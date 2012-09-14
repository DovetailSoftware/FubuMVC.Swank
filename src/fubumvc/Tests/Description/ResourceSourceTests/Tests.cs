﻿using System.Linq;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using NUnit.Framework;
using Should;
using Swank;
using Swank.Description;
using Tests.Description.ResourceSourceTests.Administration.Users;
using Tests.Description.ResourceSourceTests.Templates;
using ActionSource = Swank.ActionSource;

namespace Tests.Description.ResourceSourceTests
{
    [TestFixture]
    public class Tests
    {
        private IDescriptionSource<ActionCall, ResourceDescription> _resourceSource;
        private BehaviorGraph _graph;

        [SetUp]
        public void Setup()
        {
            _graph = Behaviors.BuildGraph().AddActionsInThisNamespace();
            _resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new ActionSource(_graph, ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly()
                    .Where(y => y.HandlerType.Namespace.StartsWith(GetType().Namespace)))), 
                new ResourceSourceConfig());
        }

        [Test]
        public void should_find_resource_markdown_description()
        {
            var resourceDescription = new AdminAccountResource();
            var action = _graph.GetAction<AdminAccountAllGetHandler>();
            _resourceSource.HasDescription(action).ShouldBeTrue();
            var resource = _resourceSource.GetDescription(action);
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual(resourceDescription.Name);
            resource.Comments.ShouldEqual("<p><strong>These are accounts yo!</strong></p>");
        }

        [Test]
        public void should_find_resource_description_when_an_applies_to_type_is_not_specified()
        {
            var resourceDescription = new AdminAddressResource();
            var action = _graph.GetAction<AdminAddressAllGetHandler>();
            _resourceSource.HasDescription(action).ShouldBeTrue();
            var resource = _resourceSource.GetDescription(action);
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual(resourceDescription.Name);
            resource.Comments.ShouldEqual(resourceDescription.Comments);
        }

        [Test]
        public void should_find_resource_description_when_an_applies_to_type_is_specified()
        {
            var resourceDescription = new AdminUserResource();
            var action = _graph.GetAction<AdminUserAllGetHandler>();
            _resourceSource.HasDescription(action).ShouldBeTrue();
            var resource = _resourceSource.GetDescription(action);
            resource.ShouldNotBeNull();
            resource.Name.ShouldEqual(resourceDescription.Name);
            resource.Comments.ShouldEqual(resourceDescription.Comments);
        }

        [Test]
        public void should_not_find_resource_description_when_none_is_specified_in_the_same_namespaces()
        {
            var action = _graph.GetAction<TemplateAllGetHandler>();
            _resourceSource.HasDescription(action).ShouldBeFalse();
            _resourceSource.GetDescription(action).ShouldBeNull();
        }

        [Test]
        public void should_enumerate_resources_using_default_grouping()
        {
            var endpoints = _graph.Actions().ToDictionary(x => x.HandlerType, _resourceSource.GetDescription);

            endpoints[typeof(AdminAccountAllGetHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAccountPostHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAccountGetHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAccountPutHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAccountDeleteHandler)].ShouldBeType<AdminAccountResource>();

            endpoints[typeof(AdminUserAllGetHandler)].ShouldBeType<AdminUserResource>();
            endpoints[typeof(AdminUserPostHandler)].ShouldBeType<AdminUserResource>();
            endpoints[typeof(AdminUserGetHandler)].ShouldBeType<AdminUserResource>();
            endpoints[typeof(AdminUserPutHandler)].ShouldBeType<AdminUserResource>();
            endpoints[typeof(AdminUserDeleteHandler)].ShouldBeType<AdminUserResource>();

            endpoints[typeof(AdminAddressAllGetHandler)].ShouldBeType<AdminAddressResource>();
            endpoints[typeof(AdminAddressPostHandler)].ShouldBeType<AdminAddressResource>();
            endpoints[typeof(AdminAddressGetHandler)].ShouldBeType<AdminAddressResource>();
            endpoints[typeof(AdminAddressPutHandler)].ShouldBeType<AdminAddressResource>();
            endpoints[typeof(AdminAddressDeleteHandler)].ShouldBeType<AdminAddressResource>();
        }

        [Test]
        public void should_enumerate_resources_using_custom_grouping()
        {
            var resourceSource = new ResourceSource(
                new MarkerSource<ResourceDescription>(),
                new ActionSource(_graph, ConfigurationDsl.CreateConfig(x => x.AppliesToThisAssembly())),
                new ResourceSourceConfig().GroupBy(x => x.ParentChain().Route.FirstPatternSegment()));
            var endpoints = _graph.Actions().ToDictionary(x => x.HandlerType, resourceSource.GetDescription);

            endpoints[typeof(AdminAccountAllGetHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAccountPostHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAccountGetHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAccountPutHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAccountDeleteHandler)].ShouldBeType<AdminAccountResource>();

            endpoints[typeof(AdminUserAllGetHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminUserPostHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminUserGetHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminUserPutHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminUserDeleteHandler)].ShouldBeType<AdminAccountResource>();

            endpoints[typeof(AdminAddressAllGetHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAddressPostHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAddressGetHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAddressPutHandler)].ShouldBeType<AdminAccountResource>();
            endpoints[typeof(AdminAddressDeleteHandler)].ShouldBeType<AdminAccountResource>();
        }
    }
}