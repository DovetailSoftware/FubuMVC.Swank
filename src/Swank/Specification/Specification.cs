﻿using System.Collections.Generic;

namespace FubuMVC.Swank.Specification
{
    public class Specification : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public List<Type> Types { get; set; }
        public List<Module> Modules { get; set; }
    }

    public class Module : IDescription
    {
        public const string DefaultName = "Resources";

        public string Name { get; set; }
        public string Comments { get; set; }
        public List<Resource> Resources { get; set; }
    }

    public class Resource : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public List<Endpoint> Endpoints { get; set; }
    }

    public class Endpoint : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Url { get; set; }
        public string Method { get; set; }
        public List<UrlParameter> UrlParameters { get; set; }
        public List<QuerystringParameter> QuerystringParameters { get; set; }
        public List<StatusCode> StatusCodes { get; set; }
        public List<Header> Headers { get; set; }
        public Data Request { get; set; }
        public Data Response { get; set; }
    }

    public class UrlParameter : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Type { get; set; }
        public List<Option> Options { get; set; }
    }

    public class QuerystringParameter : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Type { get; set; }
        public string DefaultValue { get; set; }
        public bool MultipleAllowed { get; set; }
        public bool Required { get; set; }
        public List<Option> Options { get; set; }
    }

    public class StatusCode : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public int Code { get; set; }
    }

    public class Header : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Type { get; set; }
        public bool Optional { get; set; }
    }

    public class Data : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public List<Schema> Schema { get; set; }
    }

    public class Schema
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string TypeName { get; set; }
        public string DefaultValue { get; set; }
        public bool? Required { get; set; }
        public bool? Optional { get; set; }
        public string Whitespace { get; set; }
        public bool? IsDeprecated { get; set; }
        public string DeprecationMessage { get; set; }

        public bool? IsOpening { get; set; }
        public bool? IsClosing { get; set; }

        public bool? IsMember { get; set; }
        public bool? IsLastMember { get; set; }

        public bool? IsSimpleType { get; set; }
        public bool? IsString { get; set; }
        public bool? IsBoolean { get; set; }
        public bool? IsNumeric { get; set; }
        public bool? IsDateTime { get; set; }
        public bool? IsDuration { get; set; }
        public bool? IsGuid { get; set; }
        public List<Option> Options { get; set; }

        public bool? IsComplexType { get; set; }

        public bool? IsArray { get; set; }

        public bool? IsDictionary { get; set; }
        public bool? IsDictionaryEntry { get; set; }
        public Key DictionaryKey { get; set; }
    }

    public class Key
    {
        public string Comments { get; set; }
        public string Type { get; set; }
        public List<Option> Options { get; set; }
    }

    public class Type : IDescription
    {
        public int Id { get; set; }
        public Type Ancestor { get; set; }

        public string Name { get; set; }
        public string Comments { get; set; }

        public bool IsSimple { get; set; }
        public List<Option> Options { get; set; }

        public bool IsComplex { get; set; }
        public List<Member> Members { get; set; }

        public bool IsArray { get; set; }
        public Type ArrayItemType { get; set; }

        public bool IsDictionary { get; set; }
        public Type DictionaryKeyType { get; set; }
        public Type DictionaryValueType { get; set; }
    }

    public class Member : IDescription
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public bool Required { get; set; }
        public string DefaultValue { get; set; }
        public Type Type { get; set; }
    }

    public class Option
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Comments { get; set; }
    }
}