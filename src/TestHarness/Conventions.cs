﻿using FubuMVC.Core;
using FubuMVC.Swank;

namespace TestHarness
{
    public class Conventions : FubuRegistry
    {
        public Conventions()
        {
            IncludeDiagnostics(true);

            Actions.IncludeTypesNamed(x => x.EndsWith("Handler"));
            
            Routes
                .HomeIs<IndexHandler>(x => x.ExecuteGet())
                .IgnoreNamespaceForUrlFrom<Conventions>()
                .IgnoreMethodSuffix("Execute")
                .ConstrainToHttpMethod(action => action.Method.Name.EndsWith("Get"), "GET")
                .ConstrainToHttpMethod(action => action.Method.Name.EndsWith("Post"), "POST")
                .ConstrainToHttpMethod(action => action.Method.Name.StartsWith("Put"), "PUT")
                .ConstrainToHttpMethod(action => action.Method.Name.StartsWith("Delete"), "DELETE");

            Import<Swank>(x => x
                .AtUrl("documentation")
                .AppliesTo<Conventions>()
                .Where(y => !y.HasAnyOutputBehavior()));

            Views.TryToAttachWithDefaultConventions();

            Media.ApplyContentNegotiationToActions(x => 
                !x.HasAnyOutputBehavior() && 
                x.HandlerType.Assembly == GetType().Assembly);
        }
    }
}