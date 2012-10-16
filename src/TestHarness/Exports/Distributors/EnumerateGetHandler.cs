﻿using System.Collections.Generic;
using System.Net;
using System.Xml.Serialization;
using FubuMVC.Swank.Description;

namespace TestHarness.Exports.Distributors
{
    public class EnumerateDistributorsRequest
    {
        public enum Order
        {
            [Description("Ascending", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
            Asc,
            [Description("Descending", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
            Desc
        }

        [Comments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public Order Sort { get; set; }
    }

    // Applying the XmlType attribute to the output model allows you 
    // to name the root xml element which in this case would default
    // to ArrayOfUserModel, not pretty. Also applying this attribute 
    // to the output type will not affect json output as objects 
    // aren't named like they are in xml.
    [XmlType("Users")]
    public class DistributorModels : List<Distributor> { }

    public class EnumerateGetHandler
    {
        [Description("Get Distributors")]
        [ErrorDescription(HttpStatusCode.MultipleChoices, "Fail Whale", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        [ResponseComments("Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut leo est, molestie eget laoreet eu, tincidunt sed nibh.")]
        public DistributorModels Execute(EnumerateDistributorsRequest request)
        {
            return null;
        } 
    }
}