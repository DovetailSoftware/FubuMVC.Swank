﻿using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Spark;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Extensions;
using FubuMVC.Swank.Specification;

namespace FubuMVC.Swank
{
    public class Conventions : FubuRegistry
    {
        public Conventions(Configuration configuration)
        {
            Actions.FindBy(x => x.IncludeTypesNamed(y => y.EndsWith("Handler")));
            
            Routes
                .HomeIs<ViewHandler>(x => x.Execute())
                .IgnoreMethodSuffix("Execute")
                .IgnoreControllerNamesEntirely()
                .IgnoreControllerNamespaceEntirely()
                .ConstrainToHttpMethod(action => action.Method.Name.EndsWith("Get"), "GET");

            Import<SparkEngine>();

            Policies.Add(x => x.Conneg.ApplyConneg());

            Services(x =>
            {
                x.AddService(configuration);
                x.AddService<ISpecificationService, CachedSpecificationService>();
                x.AddService<IDescriptionConvention<ActionCall, ModuleDescription>>(configuration.ModuleConvention.Type, configuration.ModuleConvention.Config)
                 .AddService<IDescriptionConvention<ActionCall, ResourceDescription>>(configuration.ResourceConvention.Type, configuration.ResourceConvention.Config)
                 .AddService<IDescriptionConvention<ActionCall, EndpointDescription>>(configuration.EndpointConvention.Type, configuration.EndpointConvention.Config)
                 .AddService<IDescriptionConvention<PropertyInfo, MemberDescription>>(configuration.MemberConvention.Type, configuration.MemberConvention.Config)
                 .AddService<IDescriptionConvention<FieldInfo, OptionDescription>>(configuration.OptionConvention.Type, configuration.OptionConvention.Config)
                 .AddService<IDescriptionConvention<ActionCall, List<StatusCodeDescription>>>(configuration.StatusCodeConvention.Type, configuration.StatusCodeConvention.Config)
                 .AddService<IDescriptionConvention<ActionCall, List<HeaderDescription>>>(configuration.HeaderConvention.Type, configuration.HeaderConvention.Config)
                 .AddService<IDescriptionConvention<System.Type, TypeDescription>>(configuration.TypeConvention.Type, configuration.TypeConvention.Config);
            });
        }
    }
}