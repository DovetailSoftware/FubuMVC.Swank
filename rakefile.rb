COMPILE_TARGET = ENV['config'].nil? ? "debug" : ENV['config']
BUILD_VERSION = '100.0.0'
tc_build_number = ENV["BUILD_NUMBER"]
build_revision = tc_build_number || Time.new.strftime('5%H%M')
build_number = "#{BUILD_VERSION}.#{build_revision}"
BUILD_NUMBER = build_number

def self.findMsbuild()
  begin
    file = `vswhere -latest -find **/Bin/MSBuild.exe`
    file = file.delete!("\r\n")
    puts "\nvswhere found #{file}'.\n\n"
    return "\"#{file}\""
  rescue
    puts "\nvswhere is not installed. Install it using 'choco install vswhere'.\n\n"
    return "failed"
  end
end

def self.getMsbuildToolsPath()
  # BuildTools is where TeamCity has MSBuild installed
  # Community is where most developers will have MSBuild installed, however some have Professional edition of Visual Studio.
  location = [
    "C:\\Program Files\\Microsoft Visual Studio\\2022\\Community\\MSBuild\\Current\\Bin\\",
    "C:\\Program Files\\Microsoft Visual Studio\\2022\\Professional\\MSBuild\\Current\\Bin\\",
    "C:\\Program Files (x86)\\Microsoft Visual Studio\\2022\\BuildTools\\MSBuild\\Current\\Bin\\",
    # The TeamCity agents in Blue-agent-pool have VS 2019 Build Tools.
    "C:\\Program Files (x86)\\Microsoft Visual Studio\\2019\\BuildTools\\MSBuild\\Current\\Bin\\",
  ]
  .find { |path| File.exist?(path) }

  if(location == nil)
    raise "Couldn't find MSBuild!"
  end

  puts "Found MSBuild at #{location}"
  return location;
end

msbuildFilePath = findMsbuild()
if(msbuildFilePath == "failed")
  msbuildFilePath = "\"#{getMsbuildToolsPath()}msbuild.exe\""
end
puts "Compiling with #{msbuildFilePath}"

desc 'Compile the code'
task :compile => [:clean, :version, :pack_bottle] do
  sh "#{msbuildFilePath} src/FubuMVC.Swank.sln /property:Configuration=#{COMPILE_TARGET} /v:m /t:rebuild /nr:False /maxcpucount:8"
end

desc "Create the assembly-pak bottles file"
task :pack_bottle do
  toolPath = get_nuget_tool_path("Bottles", "BottleRunner.exe")
  fullCmd = "#{toolPath} assembly-pak src/Swank -p Swank.csproj"
  sh fullCmd
end

desc "Update the version information for the build"
task :version do
  asm_version = build_number

  begin
    commit = `git log -1 --pretty=format:%H`
  rescue
    commit = "git unavailable"
  end
  puts "##teamcity[buildNumber '#{build_number}']" unless tc_build_number.nil?
  puts "Version: #{build_number}" if tc_build_number.nil?

  options = {
    :description => 'FubuMVC.Swank',
    :product_name => 'FubuMVC.Swank',
    :copyright => "Copyright (c) 2013-#{Time.now.year} Ultraviolet Catastrophe",
    :company => 'Ultraviolet Catastrophe',
    :trademark => commit,
    :version => asm_version,
    :file_version => build_number,
    :informational_version => asm_version
  }

  puts "Writing src/CommonAssemblyInfo.cs..."
  File.open('src/CommonAssemblyInfo.cs', 'w') do |file|
    file.write "using System.Reflection;\n"
    file.write "using System.Runtime.InteropServices;\n"
    file.write "[assembly: AssemblyDescription(\"#{options[:description]}\")]\n"
    file.write "[assembly: AssemblyProduct(\"#{options[:product_name]}\")]\n"
    file.write "[assembly: AssemblyCopyright(\"#{options[:copyright]}\")]\n"
    file.write "[assembly: AssemblyTrademark(\"#{options[:trademark]}\")]\n"
    file.write "[assembly: AssemblyVersion(\"#{options[:version]}\")]\n"
    file.write "[assembly: AssemblyCompany(\"#{options[:company]}\")]\n"
    file.write "[assembly: AssemblyFileVersion(\"#{options[:file_version]}\")]\n"
    file.write "[assembly: AssemblyInformationalVersion(\"#{options[:informational_version]}\")]\n"
  end
end

desc "Prepares the working directory for a new build"
task :clean do
  FileUtils.rm_rf 'artifacts'
  Dir.mkdir 'artifacts'
end

desc 'Run the unit tests'
task :test => [:compile, :fast_test] do
end

desc 'Run the unit tests without compile'
task :fast_test do
  sh "src/packages/NUnit.ConsoleRunner.3.6.1/tools/nunit3-console.exe src/Tests/bin/#{COMPILE_TARGET}/Tests.dll"
end

def get_nuget_tool_path(name, tool)
    path = Dir.glob("**/packages/#{name}**/tools/#{tool}")
    fail "#{name} nuget tool #{tool} could not be found under #{Dir.getwd}." unless !path.empty?
    return path.sort {|x,y| y <=> x } [0]
end