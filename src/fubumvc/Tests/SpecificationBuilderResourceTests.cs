﻿using FubuMVC.Core.Registration;
using NUnit.Framework;
using Should;
using Swank;
using Tests.Administration.Users;
using Tests.Batches.Cells;
using Tests.Batches.Schedules;
using Tests.Templates;

namespace Tests
{
    [TestFixture]
    public class SpecificationBuilderResourceTests
    {
        private static readonly BatchCellResource BatchCellResource = new BatchCellResource();
        private static readonly AdminAccountResource AdminAccountResource = new AdminAccountResource();
        private static readonly AdminAddressResource AdminAddressResource = new AdminAddressResource();
        private static readonly AdminUserResource AdminUserResource = new AdminUserResource();

        private BehaviorGraph _graph;
        private IModuleSource _moduleSource;
        private IResourceSource _resourceSource;

        [SetUp]
        public void Setup()
        {
            _graph = TestBehaviorGraph.Build();
            _moduleSource = new ModuleSource(new DescriptionSource<Module>());
            _resourceSource = new ResourceSource(
                new DescriptionSource<Resource>(),
                new Swank.ActionSource(_graph, ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly())), new ResourceSourceConfig());
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_default_resource()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.Default)
                .OnOrphanedResourceAction(OrphanedActions.Default));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), _moduleSource, _resourceSource);

            var spec = specBuilder.Build();

            var resources = spec.modules[0].resources;
            resources.Count.ShouldEqual(2);
            var resource = resources[0];
            resource.name.ShouldEqual("templates");
            resource.comments.ShouldBeNull();
            resource = resources[1];
            resource.name.ShouldEqual("templates/files");
            resource.comments.ShouldBeNull();

            resources = spec.modules[1].resources;
            resources.Count.ShouldEqual(3);
            resource = resources[0];
            resource.name.ShouldEqual(AdminAccountResource.Name);
            resource.comments.ShouldEqual(AdminAccountResource.Comments);
            resource = resources[1];
            resource.name.ShouldEqual(AdminAddressResource.Name);
            resource.comments.ShouldEqual(AdminAddressResource.Comments);
            resource = resources[2];
            resource.name.ShouldEqual(AdminUserResource.Name);
            resource.comments.ShouldEqual(AdminUserResource.Comments);

            resources = spec.modules[2].resources;
            resources.Count.ShouldEqual(1);
            resource = resources[0];
            resource.name.ShouldEqual(BatchCellResource.Name);
            resource.comments.ShouldEqual(BatchCellResource.Comments);

            resources = spec.modules[3].resources;
            resources.Count.ShouldEqual(1);
            resource = resources[0];
            resource.name.ShouldEqual("batches/schedules");
            resource.comments.ShouldBeNull();
        }

        [Test]
        public void should_automatically_add_orphaned_actions_to_specified_default_resource()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.Default)
                .OnOrphanedResourceAction(OrphanedActions.Default)
                .WithDefaultResource(y => new Resource { Name = y.ParentChain().Route.FirstPatternSegment() }));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), _moduleSource, _resourceSource);

            var spec = specBuilder.Build();

            var resources = spec.modules[0].resources;
            resources.Count.ShouldEqual(1);
            var resource = resources[0];
            resource.name.ShouldEqual("templates");
            resource.comments.ShouldBeNull();

            resources = spec.modules[1].resources;
            resources.Count.ShouldEqual(3);
            resource = resources[0];
            resource.name.ShouldEqual(AdminAccountResource.Name);
            resource.comments.ShouldEqual(AdminAccountResource.Comments);
            resource = resources[1];
            resource.name.ShouldEqual(AdminAddressResource.Name);
            resource.comments.ShouldEqual(AdminAddressResource.Comments);
            resource = resources[2];
            resource.name.ShouldEqual(AdminUserResource.Name);
            resource.comments.ShouldEqual(AdminUserResource.Comments);

            resources = spec.modules[2].resources;
            resources.Count.ShouldEqual(1);
            resource = resources[0];
            resource.name.ShouldEqual(BatchCellResource.Name);
            resource.comments.ShouldEqual(BatchCellResource.Comments);

            resources = spec.modules[3].resources;
            resources.Count.ShouldEqual(1);
            resource = resources[0];
            resource.name.ShouldEqual("batches");
            resource.comments.ShouldBeNull();
        }

        [Test]
        public void should_ignore_orphaned_actions()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.Default)
                .OnOrphanedResourceAction(OrphanedActions.Exclude)
                .WithDefaultResource(y => new Resource { Name = y.ParentChain().Route.FirstPatternSegment() }));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), _moduleSource, _resourceSource);

            var spec = specBuilder.Build();

            spec.modules[0].resources.Count.ShouldEqual(0);

            var resources = spec.modules[1].resources;
            resources.Count.ShouldEqual(3);
            var resource = resources[0];
            resource.name.ShouldEqual(AdminAccountResource.Name);
            resource.comments.ShouldEqual(AdminAccountResource.Comments);
            resource = resources[1];
            resource.name.ShouldEqual(AdminAddressResource.Name);
            resource.comments.ShouldEqual(AdminAddressResource.Comments);
            resource = resources[2];
            resource.name.ShouldEqual(AdminUserResource.Name);
            resource.comments.ShouldEqual(AdminUserResource.Comments);

            resources = spec.modules[2].resources;
            resources.Count.ShouldEqual(1);
            resource = resources[0];
            resource.name.ShouldEqual(BatchCellResource.Name);
            resource.comments.ShouldEqual(BatchCellResource.Comments);

            spec.modules[3].resources.Count.ShouldEqual(0);
        }

        [Test]
        public void should_throw_an_exception_for_orphaned_actions()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.Default)
                .OnOrphanedResourceAction(OrphanedActions.Fail));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), _moduleSource, _resourceSource);

            Assert.Throws<OrphanedResourceActionException>(() => specBuilder.Build());
        }

        [Test]
        public void should_not_throw_an_exception_when_there_are_no_orphaned_actions()
        {
            var configuration = ConfigurationDsl.CreateConfig(x => x
                .AppliesToThisAssembly()
                .OnOrphanedModuleAction(OrphanedActions.Default)
                .OnOrphanedResourceAction(OrphanedActions.Fail)
                .Where(y => y.HandlerType.Namespace != typeof(TemplateRequest).Namespace &&
                            y.HandlerType.Namespace != typeof(BatchScheduleRequest).Namespace));
            var specBuilder = new SpecificationBuilder(configuration, new Swank.ActionSource(_graph, configuration), _moduleSource, _resourceSource);

            Assert.DoesNotThrow(() => specBuilder.Build());
        }
    }
}