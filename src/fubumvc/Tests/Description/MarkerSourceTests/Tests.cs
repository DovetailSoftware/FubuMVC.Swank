﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Should;
using Swank.Description;
using Tests.Description.MarkerSourceTests.Administration;
using Tests.Description.MarkerSourceTests.Administration.Users;
using Tests.Description.MarkerSourceTests.Batches;
using Tests.Description.MarkerSourceTests.Batches.Cells;
using Tests.Description.MarkerSourceTests.Batches.Schedules;

namespace Tests.Description.MarkerSourceTests
{
    [TestFixture]
    public class Tests
    {
        public const string SchedulesModuleComments = "<p><strong>These are schedules yo!</strong></p>";
        public const string BatchesModuleComments = "<b>These are batches yo!</b>";

        private static readonly AdministrationModule AdministrationModule = new AdministrationModule();
        private static readonly BatchesModule BatchesModule = new BatchesModule();
        private static readonly SchedulesModule SchedulesModule = new SchedulesModule();

        private static readonly BatchCellResource BatchCellResource = new BatchCellResource();
        private static readonly AdminAccountResource AdminAccountResource = new AdminAccountResource();
        private static readonly AdminAddressResource AdminAddressResource = new AdminAddressResource();
        private static readonly AdminUserResource AdminUserResource = new AdminUserResource();

        private IList<ModuleDescription> _modules;
        private IList<ResourceDescription> _resources;

        [SetUp]
        public void Setup()
        {
            var handlersNamespace = GetType().Namespace;
            _modules = new MarkerSource<ModuleDescription>().GetDescriptions(Assembly.GetExecutingAssembly())
                .Where(x => x.GetType().Namespace.StartsWith(handlersNamespace)).ToList();
            _resources = new MarkerSource<ResourceDescription>().GetDescriptions(Assembly.GetExecutingAssembly())
                .Where(x => x.GetType().Namespace.StartsWith(handlersNamespace)).ToList();
        }

        [Test]
        public void should_enumerate_descriptions()
        {
            _modules.Count.ShouldEqual(3);
            _resources.Count.ShouldEqual(4);
        }

        [Test]
        public void should_be_ordered_by_descending_namespace_and_ascending_name()
        {
            _modules[0].Name.ShouldEqual(SchedulesModule.Name);
            _modules[1].Name.ShouldEqual(BatchesModule.Name);
            _modules[2].Name.ShouldEqual(AdministrationModule.Name);

            _resources[0].Name.ShouldEqual(BatchCellResource.Name);
            _resources[1].Name.ShouldEqual(AdminAccountResource.Name);
            _resources[2].Name.ShouldEqual(AdminAddressResource.Name);
            _resources[3].Name.ShouldEqual(AdminUserResource.Name);
        }

        [Test]
        public void should_set_description_comments_from_code()
        {
            _modules[2].Comments.ShouldEqual(AdministrationModule.Comments);
        }

        [Test]
        public void should_set_description_comments_from_embedded_text()
        {
            _modules[1].Comments.ShouldEqual(BatchesModuleComments);
        }

        [Test]
        public void should_set_description_comments_from_embedded_markdown()
        {
            _modules[0].Comments.ShouldEqual(SchedulesModuleComments);
        }
    }
}