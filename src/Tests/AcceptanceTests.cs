using System;
using System.IO;
using FubuMVC.Swank;
using FubuMVC.Swank.Extensions;
using NUnit.Framework;
using Should;

namespace Tests
{
    [TestFixture, Ignore("These don't work on the build server and I don't have time to fix them now (doing the big fubu consolidation for Dovetail")]
    public class AcceptanceTests
    {
        private Website _testWebsite;

        [OneTimeSetUp]
        public void Setup()
        {
            var dir = Path.GetDirectoryName(typeof(Specification.SpecificationService.MergeTests.Tests).Assembly.CodeBase);
            dir = new Uri(dir).LocalPath;
            Directory.SetCurrentDirectory(dir);

            _testWebsite = new Website();
            _testWebsite.Create(typeof(Swank).Assembly.GetName().Name, Paths.TestHarness);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _testWebsite.Remove();
        }

        public class IndexModel { public string Message { get; set; } }

        [Test]
        public void should_have_connectivity_to_the_test_harness_site()
        {
            _testWebsite.DownloadString("", "application/json").DeserializeJson<IndexModel>()
                .Message.ShouldEqual("oh hai");
        }

        [Test]
        public void should_return_specification_page()
        {
            _testWebsite.DownloadString("documentation", "text/html")
                .ShouldContain("<title>Test Harness API</title>");
        }

        [Test]
        public void should_return_specification_data()
        {
            var spec = _testWebsite.DownloadString("documentation/data", "application/json")
                .DeserializeJson<FubuMVC.Swank.Specification.Specification>();
            spec.Types.ShouldNotBeNull();
            spec.Modules.ShouldNotBeNull();
            spec.Resources.ShouldNotBeNull();
        }

        [Test]
        public void should_return_style_content()
        {
            _testWebsite.DownloadString("_content/swank/swank.css").ShouldNotBeEmpty();
        }

        [Test]
        public void should_return_js_content()
        {
            _testWebsite.DownloadString("_content/swank/swank.js").ShouldNotBeEmpty();
        }
    }
}