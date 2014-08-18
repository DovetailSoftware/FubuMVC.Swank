﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using FubuMVC.Swank.Description;
using NUnit.Framework;
using Should;

namespace Tests.Description
{
    [TestFixture]
    public class TypeConventionTests
    {
        public class SomeType { }

        [Test]
        public void should_return_default_description_of_datatype()
        {
            var type = typeof(SomeType);
            var description = new TypeConvention().GetDescription(type);
            description.Name.ShouldEqual("SomeType");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_return_default_description_of_list_datatype()
        {
            var description = new TypeConvention().GetDescription(typeof(List<SomeType>));
            description.Name.ShouldEqual("ArrayOfSomeType");
            description.Comments.ShouldBeNull();
        }

        [Comments("This is a type with comments.")]
        public class SomeTypeWithComments { }

        [Test]
        public void should_return_attribute_description_of_datatype()
        {
            var type = typeof(SomeTypeWithComments);
            var description = new TypeConvention().GetDescription(type);
            description.Name.ShouldEqual("SomeTypeWithComments");
            description.Comments.ShouldEqual("This is a type with comments.");
        }

        [XmlType("SomeType")]
        public class SomeTypeWithXmlName { }

        [Test]
        public void should_return_attribute_description_of_datatype_and_xml_type_attribute()
        {
            var type = typeof(SomeTypeWithXmlName);
            var description = new TypeConvention().GetDescription(type);
            description.Name.ShouldEqual("SomeType");
            description.Comments.ShouldBeNull();
        }

        [XmlRoot("SomeRoot")]
        public class SomeTypeWithXmlRootName { }

        [Test]
        public void should_return_attribute_description_of_datatype_and_xml_root_attribute()
        {
            var type = typeof(SomeTypeWithXmlRootName);
            var description = new TypeConvention().GetDescription(type);
            description.Name.ShouldEqual("SomeRoot");
            description.Comments.ShouldBeNull();
        }

        [DataContract(Name = "SomeType")]
        public class SomeTypeWithDataContractName { }

        [Test]
        public void should_return_data_contract_attribute_name()
        {
            var type = typeof(SomeTypeWithDataContractName);
            var description = new TypeConvention().GetDescription(type);
            description.Name.ShouldEqual("SomeType");
            description.Comments.ShouldBeNull();
        }

        [Comments("These are some types.")]
        public class SomeTypes : List<SomeType> { }

        [Test]
        public void should_return_attribute_description_of_inherited_list_datatype()
        {
            var description = new TypeConvention().GetDescription(typeof(SomeTypes));
            description.Name.ShouldEqual("ArrayOfSomeType");
            description.Comments.ShouldEqual("These are some types.");
        }

        [Comments("These are some moar types."), XmlType("SomeTypes")]
        public class SomeMoarTypes : List<SomeType> { }

        [CollectionDataContract(Name = "SomeTypes")]
        public class SomeCollectionWithDataContractName : List<SomeType> { }

        [Test]
        public void should_return_attribute_description_of_inherited_list_datatype_with_xml_type_attribute()
        {
            var description = new TypeConvention().GetDescription(typeof(SomeMoarTypes));
            description.Name.ShouldEqual("SomeTypes");
            description.Comments.ShouldEqual("These are some moar types.");
        }

        [Test]
        public void should_return_name_of_inherited_list_datatype_with_collection_data_contract_attribute()
        {
            var description = new TypeConvention().GetDescription(typeof(SomeCollectionWithDataContractName));
            description.Name.ShouldEqual("SomeTypes");
            description.Comments.ShouldBeNull();
        }

        [Test]
        public void should_initial_cap_list_primitive_type_name()
        {
            var description = new TypeConvention().GetDescription(typeof(List<Int64>));
            description.Name.ShouldEqual("ArrayOfLong");
        }
    }
}