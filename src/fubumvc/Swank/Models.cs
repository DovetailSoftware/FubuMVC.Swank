﻿using System.Collections.Generic;

namespace Swank.Models
{
    public class Specification
    {
        public List<DataType> dataTypes { get; set; }
        public List<Module> modules { get; set; }
    }

    public class DataType
    {
        public string name { get; set; }
        public string alias { get; set; }
        public string description { get; set; }
        public List<Member> members { get; set; }
    }

    public class Member
    {
        public string name { get; set; }
        public string description { get; set; }
        public bool required { get; set; }
        public string defaultValue { get; set; }
        public bool collection { get; set; }
        public string dataType { get; set; }
        public List<Option> options { get; set; }
    }

    public class Option
    {
        public string name { get; set; }
        public string value { get; set; }
        public string description { get; set; }
    }

    public class Module
    {
        public string name { get; set; }
        public string description { get; set; }
        public List<Resource> resources { get; set; }
    }

    public class Resource
    {
        public string name { get; set; }
        public string description { get; set; }
        public List<Endpoint> endpoints { get; set; }
    }

    public class Endpoint
    {
        public string url { get; set; }
        public string method { get; set; }
        public string description { get; set; }
        public List<UrlParameter> urlParameters { get; set; }
        public List<QuerystringParameter> querystringParameters { get; set; }
        public List<Error> errors { get; set; }
        public Data request { get; set; }
        public Data response { get; set; }
    }

    public class UrlParameter
    {
        public string name { get; set; }
        public string description { get; set; }
        public string dataType { get; set; }
        public List<Option> options { get; set; }
    }

    public class QuerystringParameter
    {
        public string name { get; set; }
        public string description { get; set; }
        public string dataType { get; set; }
        public string defaultValue { get; set; }
        public bool multipleAllowed { get; set; }
        public List<Option> options { get; set; }
    }

    public class Error
    {
        public int status { get; set; }
        public string description { get; set; }
    }

    public class Data
    {
        public string description { get; set; }
        public string dataType { get; set; }
        public bool collection { get; set; }
    }
}