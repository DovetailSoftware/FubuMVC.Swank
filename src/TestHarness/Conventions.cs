﻿using FubuMVC.Core;
using FubuMVC.Swank;
using FubuMVC.Swank.Extensions;
using FubuMVC.Swank.Specification;

namespace TestHarness
{
    public class Conventions : FubuRegistry
    {
        public Conventions()
        {
            IncludeDiagnostics(true);

            Actions.IncludeTypesNamed(x => x.EndsWith("Handler"));
            
            Routes
                .HomeIs<IndexGetHandler>(x => x.Execute())
                .IgnoreNamespaceForUrlFrom<Conventions>()
                .IgnoreMethodSuffix("Execute")
                .IgnoreControllerNamesEntirely()
                .ConstrainToHttpMethod(action => action.HandlerType.Name.EndsWith("GetHandler"), "GET")
                .ConstrainToHttpMethod(action => action.HandlerType.Name.EndsWith("PostHandler"), "POST")
                .ConstrainToHttpMethod(action => action.HandlerType.Name.EndsWith("PutHandler"), "PUT")
                .ConstrainToHttpMethod(action => action.HandlerType.Name.EndsWith("DeleteHandler"), "DELETE");

            Import<Swank>(x => x
                .AppliesToThisAssembly()
                .AtUrl("documentation")
                .Named("Test Harness API")
                .WithCopyright("Copyright &copy; {year} Test Harness")
                .WithStylesheets("~/styles/style.css")
                .WithScripts("~/scripts/script.js")
                .MergeThisSpecification("~/spec.json")
                .OverrideEndpoints((action, endpoint) => endpoint
                    .Errors.Add(new Error { Status = 404, Name = "Not Found", Comments = "The item was not found!" }))
                .OverridePropertiesWhen((propertyinfo, property) => property.Comments = "This is the id of the user.",
                    (propertyinfo, property) => propertyinfo.Name == "UserId" && propertyinfo.IsGuid()));

            Views.TryToAttachWithDefaultConventions();

            Media.ApplyContentNegotiationToActions(x => x.HandlerType.Assembly == GetType().Assembly);
        }
    }
}