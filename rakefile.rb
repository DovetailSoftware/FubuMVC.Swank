COMPILE_TARGET = ENV['config'].nil? ? "debug" : ENV['config']
BUILD_VERSION = '100.0.0'
tc_build_number = ENV["BUILD_NUMBER"]
build_revision = tc_build_number || Time.new.strftime('5%H%M')
build_number = "#{BUILD_VERSION}.#{build_revision}"
BUILD_NUMBER = build_number 
msbuild = '"C:\Program Files (x86)\MSBuild\14.0\Bin\msbuild.exe"'

require 'win32/registry'
# See: https://en.wikibooks.org/wiki/Ruby_Programming/Standard_Library/Win32::Registry
desc "Testing msbuild"
task :msbuild do
  puts getMsbuildToolsPath(14.0)
end

def self.getMsbuildToolsPath(msbuildToolsVersion)
    Win32::Registry::HKEY_LOCAL_MACHINE.open("Software\\Microsoft\\MSBuild\\ToolsVersions\\#{msbuildToolsVersion}") do |reg|
      #reg.each{|name, type, value| puts name, type, value}
      #reg.each_key{|k, v| puts k, v}
      return reg['MSBuildToolsPath'] + 'MSBuild.exe'
    end
end

desc 'Compile the code'
task :compile => [:clean, :version, :pack_bottle] do
  msbuild = getMsbuildToolsPath(14.0)
  msbuild = '"'+msbuild+'"'
  sh "#{msbuild} src/FubuMVC.Swank.sln /property:Configuration=#{COMPILE_TARGET} /v:m /t:rebuild /nr:False /maxcpucount:8"
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