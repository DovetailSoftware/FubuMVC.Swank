﻿namespace FubuMVC.Swank.Description
{
    public class ModuleDescription : DescriptionBase
    {
        public ModuleDescription() {}
        public ModuleDescription(string name, string comments = null) : base(name, comments) { }
    }

    public class ResourceDescription<THandler> : ResourceDescription
    {
        public ResourceDescription() { }
        public ResourceDescription(string name, string comments = null) : base(name, comments) { }
    }

    public class ResourceDescription : DescriptionBase
    {
        public ResourceDescription() {}
        public ResourceDescription(string name, string comments = null) : base(name, comments) { }

        public System.Type Handler { get; set; }
    }

    public class EndpointDescription : DescriptionBase
    {
        public string RequestComments { get; set; }
        public string ResponseComments { get; set; }
    }

    public class MemberDescription : DescriptionBase
    {
        public object DefaultValue { get; set; }
        public bool Required { get; set; }
        public System.Type Type { get; set; }
    }

    public class OptionDescription : DescriptionBase { }

    public class ErrorDescription : DescriptionBase
    {
        public int Status { get; set; }
    }

    public class TypeDescription : DescriptionBase
    {
        public System.Type Type { get; set; }
    }
}