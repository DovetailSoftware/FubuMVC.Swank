﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Extensions;
using FubuMVC.Swank.Specification;
using NUnit.Framework;
using Should;

namespace Tests.Specification
{
    [TestFixture]
    public class DataDescriptionFactoryTests
    {
        public List<DataDescription> BuildDescription(Type type,
            Action<Configuration> configure = null, ActionCall action = null)
        {
            var configuration = new Configuration();
            if (configure != null) configure(configuration);
            return new DataDescriptionFactory(configuration).Create(new TypeGraphFactory(
                configuration,
                new TypeDescriptorCache(),
                new TypeConvention(configuration),
                new MemberConvention(),
                new OptionFactory(configuration, new OptionConvention())).BuildGraph(type, action));
        }

        public List<DataDescription> BuildDescription<T>(
            Action<Configuration> configure = null, ActionCall action = null)
        {
            return BuildDescription(typeof(T), configure, action);
        }

        // Complex types

        [Comments("Complex type comments")]
        public class ComplexTypeWithNoMembers { }

        [Test]
        public void should_create_complex_type()
        {
            var description = BuildDescription<ComplexTypeWithNoMembers>();

            description.Count.ShouldEqual(2);

            description[0].ShouldBeComplexType("ComplexTypeWithNoMembers", 0,
                x => x.First().Opening().Comments("Complex type comments"));
            
            description[1].ShouldBeComplexType("ComplexTypeWithNoMembers", 0, x => x.Last().Closing());
        }

        public class ComplexTypeWithSimpleMembers
        {
            public string StringMember { get; set; }
            public bool BooleanMember { get; set; }
            public DateTime DateTimeMember { get; set; }
            public TimeSpan DurationMember { get; set; }
            public Guid UuidMember { get; set; }
            public int NumericMember { get; set; }
        }

        [Test]
        public void should_create_complex_type_with_simple_type_members()
        {
            var description = BuildDescription<ComplexTypeWithSimpleMembers>();

            description.Count.ShouldEqual(8);

            description[0].ShouldBeComplexType("ComplexTypeWithSimpleMembers", 0, x => x.First().Opening());

            description[1].ShouldBeSimpleTypeMember("StringMember", "string", 1, "", x => x.IsString());
            description[2].ShouldBeSimpleTypeMember("BooleanMember", "boolean", 1, "false", x => x.IsBoolean());
            description[3].ShouldBeSimpleTypeMember("DateTimeMember", "dateTime", 1, DateTime.Now.ToString("g"), x => x.IsDateTime());
            description[4].ShouldBeSimpleTypeMember("DurationMember", "duration", 1, "0:00:00", x => x.IsDuration());
            description[5].ShouldBeSimpleTypeMember("UuidMember", "uuid", 1, "00000000-0000-0000-0000-000000000000", x => x.IsGuid());
            description[6].ShouldBeSimpleTypeMember("NumericMember", "int", 1, "0", x => x.IsNumeric(), x => x.IsLastMember());

            description[7].ShouldBeComplexType("ComplexTypeWithSimpleMembers", 0, x => x.Last().Closing());
        }

        public enum Options
        {
            Option, 
            [Comments("This is an option.")]
            OptionWithComments
        }

        public class ComplexTypeWithSimpleOptionMember
        {
            public Options OptionMember { get; set; }
        }

        [Test]
        public void should_create_complex_type_with_simple_numeric_option_member()
        {
            var description = BuildDescription<ComplexTypeWithSimpleOptionMember>();

            description.Count.ShouldEqual(3);

            description[0].ShouldBeComplexType("ComplexTypeWithSimpleOptionMember", 0, x => x.First().Opening());

            description[1].ShouldBeSimpleTypeMember("OptionMember", "int", 1, "0", 
                x => x.IsNumeric()
                    .Options
                        .WithOption("Option", "0")
                        .WithOptionAndComments("OptionWithComments", "1", "This is an option."), 
                x => x.IsLastMember());

            description[2].ShouldBeComplexType("ComplexTypeWithSimpleOptionMember", 0, x => x.Last().Closing());
        }

        [Test]
        public void should_create_complex_type_with_simple_string_option_member()
        {
            var description = BuildDescription<ComplexTypeWithSimpleOptionMember>(
                x => x.EnumValue = EnumValue.AsString);

            description.Count.ShouldEqual(3);

            description[0].ShouldBeComplexType("ComplexTypeWithSimpleOptionMember", 0, x => x.First().Opening());

            description[1].ShouldBeSimpleTypeMember("OptionMember", "string", 1, "Option",
                x => x.IsString()
                    .Options
                        .WithOption("Option")
                        .WithOptionAndComments("OptionWithComments", "This is an option."),
                x => x.IsLastMember());

            description[2].ShouldBeComplexType("ComplexTypeWithSimpleOptionMember", 0, x => x.Last().Closing());
        }

        public class ComplexTypeWithOptionalMember
        {
            [Optional]
            public string OptionalMember { get; set; }
            public string RequiredMember { get; set; }
        }

        public class OptionalMemberPostHandler
        {
            public void Execute(ComplexTypeWithOptionalMember request) { }
        }

        [Test]
        public void should_create_complex_type_with_optional_members()
        {
            var action = Behavior.BuildGraph().AddAndGetAction<OptionalMemberPostHandler>();

            var description = BuildDescription<ComplexTypeWithOptionalMember>(action: action);

            description.Count.ShouldEqual(4);

            description[0].ShouldBeComplexType("ComplexTypeWithOptionalMember", 0, x => x.First().Opening());

            description[1].ShouldBeSimpleTypeMember("OptionalMember", "string", 1, "", x => x.IsString(),
                x => x.Optional());

            description[2].ShouldBeSimpleTypeMember("RequiredMember", "string", 1, "", x => x.IsString(),
                x => x.Required().IsLastMember());

            description[3].ShouldBeComplexType("ComplexTypeWithOptionalMember", 0, x => x.Last().Closing());
        }

        public class ComplexTypeWithDeprecatedMember
        {
            [Obsolete("Why u no use different one??")]
            public string DeprecatedMember { get; set; }
        }

        [Test]
        public void should_create_complex_type_with_deprecated_members()
        {
            var description = BuildDescription<ComplexTypeWithDeprecatedMember>();

            description.Count.ShouldEqual(3);

            description[0].ShouldBeComplexType("ComplexTypeWithDeprecatedMember", 0, x => x.First().Opening());

            description[1].ShouldBeSimpleTypeMember("DeprecatedMember", "string", 1, "", x => x.IsString(),
                x => x.IsDeprecated("Why u no use different one??").IsLastMember());

            description[2].ShouldBeComplexType("ComplexTypeWithDeprecatedMember", 0, x => x.Last().Closing());
        }

        public class ComplexTypeWithArrayMembers
        {
            public List<string> ArrayMember { get; set; }
        }

        public class ComplexTypeWithArrayMembersWithCustomItemName
        {
            [XmlArrayItem("Item")]
            public List<string> ArrayMember { get; set; }
        }

        [Test]
        [TestCase(typeof(ComplexTypeWithArrayMembers), "string")]
        [TestCase(typeof(ComplexTypeWithArrayMembersWithCustomItemName), "Item")]
        public void should_create_complex_type_with_array_members(
            Type type, string itemName)
        {
            var description = BuildDescription(type);

            description.Count.ShouldEqual(5);
            description[0].ShouldBeComplexType(type.Name, 0, x => x.First().Opening());

            description[1].ShouldBeArrayMember("ArrayMember", 1, x => x.Opening(), x => x.IsLastMember());

            description[2].ShouldBeSimpleType(itemName, "string", 2, "", x => x.IsString());

            description[3].ShouldBeArrayMember("ArrayMember", 1, x => x.Closing(), x => x.IsLastMember());

            description[4].ShouldBeComplexType(type.Name, 0, x => x.Last().Closing());
        }

        public class ComplexTypeWithDictionaryMember
        {
            [DictionaryDescription("Entries", "This is a dictionary.",
                "KeyName", "This is a dictionary key.",
                "This is a dictionary value.")]
            public Dictionary<string, string> DictionaryMember { get; set; }
        }

        [Test]
        public void should_create_complex_type_with_dictionary_members()
        {
            var description = BuildDescription<ComplexTypeWithDictionaryMember>();

            description.Count.ShouldEqual(5);

            description[0].ShouldBeComplexType("ComplexTypeWithDictionaryMember", 0, x => x.First().Opening());

            description[1].ShouldBeDictionaryMember("Entries", 1, x => x.Opening(),
                x => x.Comments("This is a dictionary.").IsLastMember());

            description[2].ShouldBeSimpleTypeDictionaryEntry("KeyName", "string", "string", 2, "",
               x => x.IsString().Comments("This is a dictionary value."),
               x => x.KeyComments("This is a dictionary key."));

            description[3].ShouldBeDictionaryMember("Entries", 1, x => x.Closing(), x => x.IsLastMember());

            description[4].ShouldBeComplexType("ComplexTypeWithDictionaryMember", 0, x => x.Last().Closing());
        }

        // Arrays

        [ArrayDescription("Items", "This is an array", 
            "Item", "This is an array item.")]
        public class ArrayType : List<string> { }

        [Test]
        public void should_create_an_array_with_a_description()
        {
            var description = BuildDescription<ArrayType>();

            description.Count.ShouldEqual(3);

            description[0].ShouldBeArray("Items", 0, x => x
                .Comments("This is an array").First().Opening());

            description[1].ShouldBeSimpleType("Item", "string", 1, "", x => x
                .Comments("This is an array item.").IsString());

            description[2].ShouldBeArray("Items", 0, x => x.Last().Closing());
        }

        public enum ArrayOptions { Option1, Option2 }

        [Test]
        public void should_create_an_array_of_numeric_options()
        {
            var description = BuildDescription<List<ArrayOptions>>();

            description.Count.ShouldEqual(3);

            description[0].ShouldBeArray("ArrayOfInt", 0, x => x.First().Opening());

            description[1].ShouldBeSimpleType("int", "int", 1, "0", x => x.IsNumeric()
                .Options
                    .WithOption("Option1", "0")
                    .WithOption("Option2", "1")); 

            description[2].ShouldBeArray("ArrayOfInt", 0, x => x.Last().Closing());
        }

        [Test]
        public void should_create_an_array_of_string_options()
        {
            var description = BuildDescription<List<ArrayOptions>>(x => x.EnumValue = EnumValue.AsString);

            description.Count.ShouldEqual(3);

            description[0].ShouldBeArray("ArrayOfString", 0, x => x.First().Opening());

            description[1].ShouldBeSimpleType("string", "string", 1, "Option1", x => x.IsString()
                .Options
                    .WithOption("Option1")
                    .WithOption("Option2"));

            description[2].ShouldBeArray("ArrayOfString", 0, x => x.Last().Closing());
        }

        public class ArrayComplexType { public string Member { get; set; } }

        [Test]
        public void should_create_an_array_of_complex_types()
        {
            var description = BuildDescription<List<ArrayComplexType>>();

            description.Count.ShouldEqual(5);

            description[0].ShouldBeArray("ArrayOfArrayComplexType", 0, x => x.First().Opening());

            description[1].ShouldBeComplexType("ArrayComplexType", 1, x => x.Opening());

            description[2].ShouldBeSimpleTypeMember("Member", "string", 2, "", 
                x => x.IsString(), x => x.IsLastMember());

            description[3].ShouldBeComplexType("ArrayComplexType", 1, x => x.Closing());

            description[4].ShouldBeArray("ArrayOfArrayComplexType", 0, x => x.Last().Closing());
        }

        [Test]
        public void should_create_an_array_of_arrays()
        {
            var description = BuildDescription<List<List<string>>>();

            description.Count.ShouldEqual(5);

            description[0].ShouldBeArray("ArrayOfArrayOfString", 0, x => x.First().Opening());

            description[1].ShouldBeArray("ArrayOfString", 1, x => x.Opening());

            description[2].ShouldBeSimpleType("string", "string", 2, "", x => x.IsString());

            description[3].ShouldBeArray("ArrayOfString", 1, x => x.Closing());

            description[4].ShouldBeArray("ArrayOfArrayOfString", 0, x => x.Last().Closing());
        }

        [Test]
        public void should_create_an_array_of_dictionaries()
        {
            var description = BuildDescription<List<Dictionary<string, int>>>();

            description.Count.ShouldEqual(5);

            description[0].ShouldBeArray("ArrayOfDictionaryOfInt", 0, x => x.First().Opening());

            description[1].ShouldBeDictionary("DictionaryOfInt", 1, x => x.Opening());

            description[2].ShouldBeSimpleTypeDictionaryEntry("key", "string", "int", 2, "0",
                x => x.IsNumeric());

            description[3].ShouldBeDictionary("DictionaryOfInt", 1, x => x.Closing());

            description[4].ShouldBeArray("ArrayOfDictionaryOfInt", 0, x => x.Last().Closing());
        }

        // Dictionaries

        [DictionaryDescription("Entries", "This is a dictionary.",
            "KeyName", "This is a dictionary key.", 
            "This is a dictionary value.")]
        public class DictionaryType : Dictionary<string, int> { }

        [Test]
        public void should_create_a_dictionary_with_a_description()
        {
            var description = BuildDescription<DictionaryType>();

            description.Count.ShouldEqual(3);

            description[0].ShouldBeDictionary("Entries", 0, x => x
                .Comments("This is a dictionary.").First().Opening());

            description[1].ShouldBeSimpleTypeDictionaryEntry("KeyName", "string", "int", 1, "0",
                x => x.IsNumeric().Comments("This is a dictionary value."), 
                x => x.KeyComments("This is a dictionary key."));

            description[2].ShouldBeDictionary("Entries", 0, x => x.Last().Closing());
        }

        public enum DictionaryKeyOptions { KeyOption1, KeyOption2 }
        public enum DictionaryValueOptions { ValueOption1, ValueOption2 }

        [Test]
        public void should_create_an_dictionary_of_numeric_options()
        {
            var description = BuildDescription<Dictionary<DictionaryKeyOptions, DictionaryValueOptions>>();

            description.Count.ShouldEqual(3);

            description[0].ShouldBeDictionary("DictionaryOfInt", 0, x => x.First().Opening());

            description[1].ShouldBeSimpleTypeDictionaryEntry("key", "int", "int", 1, "0",
                x => x.IsNumeric()
                    .Options
                        .WithOption("ValueOption1", "0")
                        .WithOption("ValueOption2", "1"),
                x => x.KeyOptions
                    .WithOption("KeyOption1", "0")
                    .WithOption("KeyOption2", "1"));

            description[2].ShouldBeDictionary("DictionaryOfInt", 0, x => x.Last().Closing());
        }

        [Test]
        public void should_create_an_dictionary_of_string_options()
        {
            var description = BuildDescription<Dictionary<DictionaryKeyOptions, DictionaryValueOptions>>(x => x.EnumValue = EnumValue.AsString);

            description.Count.ShouldEqual(3);

            description[0].ShouldBeDictionary("DictionaryOfString", 0, x => x.First().Opening());

            description[1].ShouldBeSimpleTypeDictionaryEntry("key", "string", "string", 1, "ValueOption1",
                x => x.IsString()
                    .Options
                        .WithOption("ValueOption1")
                        .WithOption("ValueOption2"),
                x => x.KeyOptions
                    .WithOption("KeyOption1")
                    .WithOption("KeyOption2"));

            description[2].ShouldBeDictionary("DictionaryOfString", 0, x => x.Last().Closing());
        }
        public class DictionaryComplexType { public string Member { get; set; } }

        [Test]
        public void should_create_a_dictionary_of_complex_types()
        {
            var description = BuildDescription<Dictionary<string, DictionaryComplexType>>();

            description.Count.ShouldEqual(5);

            description[0].ShouldBeDictionary("DictionaryOfDictionaryComplexType", 0, x => x.First().Opening());

            description[1].ShouldBeOpeningComplexTypeDictionaryEntry("key", "string", 1);

            description[2].ShouldBeSimpleTypeMember("Member", "string", 2, "",
                x => x.IsString(), x => x.IsLastMember());

            description[3].ShouldBeClosingComplexTypeDictionaryEntry("key", 1);

            description[4].ShouldBeDictionary("DictionaryOfDictionaryComplexType", 0, x => x.Last().Closing());
        }

        [Test]
        public void should_create_a_dictionary_of_arrays()
        {
            var description = BuildDescription<Dictionary<string, List<int>>>();

            description.Count.ShouldEqual(5);

            description[0].ShouldBeDictionary("DictionaryOfArrayOfInt", 0, x => x.First().Opening());

            description[1].ShouldBeOpeningArrayDictionaryEntry("key", "string", 1);

            description[2].ShouldBeSimpleType("int", "int", 2, "0", x => x.IsNumeric());

            description[3].ShouldBeClosingArrayDictionaryEntry("key", 1);

            description[4].ShouldBeDictionary("DictionaryOfArrayOfInt", 0, x => x.Last().Closing());
        }

        [Test]
        public void should_create_a_dictionary_of_dictionaries()
        {
            var description = BuildDescription<Dictionary<string, Dictionary<string, int>>>();

            description.Count.ShouldEqual(5);

            description[0].ShouldBeDictionary("DictionaryOfDictionaryOfInt", 0, x => x.First().Opening());

            description[1].ShouldBeOpeningDictionaryDictionaryEntry("key", "string", 1);

            description[2].ShouldBeSimpleTypeDictionaryEntry("key", "string", "int", 2, "0",
                x => x.IsNumeric());

            description[3].ShouldBeClosingDictionaryDictionaryEntry("key", 1);

            description[4].ShouldBeDictionary("DictionaryOfDictionaryOfInt", 0, x => x.Last().Closing());
        }
    }

    public static class DataDescriptionAssertions
    {
        // Simple type assertions

        public static void ShouldBeSimpleType(this DataDescription source,
            string name, string typeName, int level, string defaultValue,
            Action<SimpleTypeDsl> simpleTypeProperties)
        {
            source.ShouldMatchData(CreateSimpleType(name, typeName, 
                level, defaultValue, simpleTypeProperties));
        }

        public static void ShouldBeSimpleTypeMember(this DataDescription source,
            string name, string typeName, int level, string defaultValue,
            Action<SimpleTypeDsl> simpleTypeProperties,
            Action<MemberDsl> memberProperties = null)
        {
            var compare = CreateSimpleType(name, typeName, 
                level, defaultValue, simpleTypeProperties);
            compare.IsMember = true;
            if (memberProperties != null) memberProperties(new MemberDsl(compare));
            source.ShouldMatchData(compare);
        }

        public static void ShouldBeSimpleTypeDictionaryEntry(this DataDescription source,
            string name, string keyTypeName, string valueTypeName, int level, string defaultValue,
            Action<SimpleTypeDsl> simpleTypeProperties,
            Action<DictionaryKeyDsl> dictionaryEntryProperties = null)
        {
            var compare = CreateSimpleType(name, valueTypeName,
                level, defaultValue, simpleTypeProperties);
            compare.IsDictionaryEntry = true;
            compare.DictionaryKey = new Key { TypeName = keyTypeName };
            if (dictionaryEntryProperties != null) 
                dictionaryEntryProperties(new DictionaryKeyDsl(compare));
            source.ShouldMatchData(compare);
        }

        private static DataDescription CreateSimpleType(
            string name, string typeName, int level, string defaultValue,
            Action<SimpleTypeDsl> simpleTypeProperties)
        {
            var simpleType = new DataDescription
            {
                Name = name,
                TypeName = typeName,
                IsSimpleType = true,
                DefaultValue = defaultValue,
                Whitespace = DataDescriptionFactory.Whitespace.Repeat(level)
            };
            simpleTypeProperties(new SimpleTypeDsl(simpleType));
            return simpleType;
        }

        public class SimpleTypeDsl
        {
            private readonly DataDescription _data;
            public SimpleTypeDsl(DataDescription data) { _data = data; }
            public SimpleTypeDsl Comments(string comments) { _data.Comments = comments; return this; }
            public SimpleTypeDsl IsString() { _data.IsString = true; return this; }
            public SimpleTypeDsl IsBoolean() { _data.IsBoolean = true; return this; }
            public SimpleTypeDsl IsNumeric() { _data.IsNumeric = true; return this; }
            public SimpleTypeDsl IsDateTime() { _data.IsDateTime = true; return this; }
            public SimpleTypeDsl IsDuration() { _data.IsDuration = true; return this; }
            public SimpleTypeDsl IsGuid() { _data.IsGuid = true; return this; }

            public OptionDsl Options
            {
                get { return new OptionDsl(_data.Options = _data.Options ?? new List<Option>()); }
            }
        }

        // Array assertions

        public static void ShouldBeArray(
            this DataDescription source, string name, int level,
            Action<ArrayDsl> properties)
        {
            source.ShouldMatchData(CreateArray(name, level, properties));
        }

        public static void ShouldBeArrayMember(
            this DataDescription source, string name, int level,
            Action<ArrayDsl> arrayProperties,
            Action<MemberDsl> memberProperties = null)
        {
            var compare = CreateArray(name, level, arrayProperties);
            compare.IsMember = true;
            if (memberProperties != null) memberProperties(new MemberDsl(compare));
            source.ShouldMatchData(compare);
        }

        public static void ShouldBeOpeningArrayDictionaryEntry(
            this DataDescription source, string name, string keyTypeName, int level,
            Action<ArrayDsl> arrayProperties = null,
            Action<DictionaryKeyDsl> dictionaryKeyProperties = null)
        {
            var compare = CreateArray(name, level, arrayProperties);
            compare.IsOpening = true;
            compare.IsDictionaryEntry = true;
            compare.DictionaryKey = new Key { TypeName = keyTypeName };
            if (dictionaryKeyProperties != null)
                dictionaryKeyProperties(new DictionaryKeyDsl(compare));
            source.ShouldMatchData(compare);
        }

        public static void ShouldBeClosingArrayDictionaryEntry(
            this DataDescription source, string name, int level,
            Action<ArrayDsl> arrayProperties = null)
        {
            var compare = CreateArray(name, level, arrayProperties);
            compare.IsClosing = true;
            compare.IsDictionaryEntry = true;
            source.ShouldMatchData(compare);
        }

        private static DataDescription CreateArray(
            string name, int level, Action<ArrayDsl> properties)
        {
            var arrayType = new DataDescription
            {
                Name = name,
                IsArray = true,
                Whitespace = DataDescriptionFactory.Whitespace.Repeat(level)
            };
            if (properties != null) properties(new ArrayDsl(arrayType));
            return arrayType;
        }

        public class ArrayDsl
        {
            private readonly DataDescription _data;
            public ArrayDsl(DataDescription data) { _data = data; }
            public ArrayDsl Comments(string comments) { _data.Comments = comments; return this; }
            public ArrayDsl Opening() { _data.IsOpening = true; return this; }
            public ArrayDsl Closing() { _data.IsClosing = true; return this; }
            public ArrayDsl First() { _data.IsFirst = true; return this; }
            public ArrayDsl Last() { _data.IsLast = true; return this; }
        }

        // Dictionary assertions

        public static void ShouldBeDictionary(
            this DataDescription source, string name, int level,
            Action<DictionaryDsl> properties)
        {
            source.ShouldMatchData(CreateDictionary(name, level, properties));
        }

        public static void ShouldBeDictionaryMember(
            this DataDescription source, string name, int level,
            Action<DictionaryDsl> dictionaryProperties,
            Action<MemberDsl> memberProperties = null)
        {
            var compare = CreateDictionary(name, level, dictionaryProperties);
            compare.IsMember = true;
            if (memberProperties != null) memberProperties(new MemberDsl(compare));
            source.ShouldMatchData(compare);
        }

        public static void ShouldBeDictionaryDictionaryEntry(
            this DataDescription source, string name, string keyTypeName, int level,
            Action<DictionaryDsl> dictionaryProperties,
            Action<DictionaryKeyDsl> dictionaryKeyProperties = null)
        {
            var compare = CreateDictionary(name, level, dictionaryProperties);
            compare.IsDictionaryEntry = true;
            compare.DictionaryKey = new Key { TypeName = keyTypeName };
            if (dictionaryKeyProperties != null)
                dictionaryKeyProperties(new DictionaryKeyDsl(compare));
            source.ShouldMatchData(compare);
        }

        public static void ShouldBeOpeningDictionaryDictionaryEntry(
            this DataDescription source, string name, string keyTypeName, int level,
            Action<DictionaryDsl> dictionaryProperties = null,
            Action<DictionaryKeyDsl> dictionaryKeyProperties = null)
        {
            var compare = CreateDictionary(name, level, dictionaryProperties);
            compare.IsOpening = true;
            compare.IsDictionaryEntry = true;
            compare.DictionaryKey = new Key { TypeName = keyTypeName };
            if (dictionaryKeyProperties != null)
                dictionaryKeyProperties(new DictionaryKeyDsl(compare));
            source.ShouldMatchData(compare);
        }

        public static void ShouldBeClosingDictionaryDictionaryEntry(
            this DataDescription source, string name, int level,
            Action<DictionaryDsl> dictionaryKeyProperties = null)
        {
            var compare = CreateDictionary(name, level, dictionaryKeyProperties);
            compare.IsClosing = true;
            compare.IsDictionaryEntry = true;
            source.ShouldMatchData(compare);
        }

        private static DataDescription CreateDictionary(
            string name, int level, Action<DictionaryDsl> properties)
        {
            var dictionaryType = new DataDescription
            {
                Name = name,
                IsDictionary = true,
                Whitespace = DataDescriptionFactory.Whitespace.Repeat(level)
            };
            if (properties != null) properties(new DictionaryDsl(dictionaryType));
            return dictionaryType;
        }

        public class DictionaryDsl
        {
            private readonly DataDescription _data;
            public DictionaryDsl(DataDescription data) { _data = data; }
            public DictionaryDsl Comments(string comments) { _data.Comments = comments; return this; }
            public DictionaryDsl Opening() { _data.IsOpening = true; return this; }
            public DictionaryDsl Closing() { _data.IsClosing = true; return this; }
            public DictionaryDsl First() { _data.IsFirst = true; return this; }
            public DictionaryDsl Last() { _data.IsLast = true; return this; }
        }

        // Complex type assertions

        public static void ShouldBeComplexType(this DataDescription source,
            string name, int level, Action<ComplexTypeDsl> properties)
        {
            source.ShouldMatchData(CreateComplexType(name, level, properties));
        }

        public static void ShouldBeComplexTypeMember(
            this DataDescription source, string name, int level,
            Action<ComplexTypeDsl> complexTypeProperties = null,
            Action<MemberDsl> memberProperties = null)
        {
            var compare = CreateComplexType(name, level, complexTypeProperties);
            compare.IsMember = true;
            if (memberProperties != null) memberProperties(new MemberDsl(compare));
            source.ShouldMatchData(compare);
        }

        public static void ShouldBeOpeningComplexTypeDictionaryEntry(
            this DataDescription source, string name, string keyTypeName, int level,
            Action<ComplexTypeDsl> complexTypeProperties = null,
            Action<DictionaryKeyDsl> dictionaryKeyProperties = null)
        {
            var compare = CreateComplexType(name, level, complexTypeProperties);
            compare.IsOpening = true;
            compare.IsDictionaryEntry = true;
            compare.DictionaryKey = new Key { TypeName = keyTypeName };
            if (dictionaryKeyProperties != null)
                dictionaryKeyProperties(new DictionaryKeyDsl(compare));
            source.ShouldMatchData(compare);
        }

        public static void ShouldBeClosingComplexTypeDictionaryEntry(
            this DataDescription source, string name, int level,
            Action<ComplexTypeDsl> complexTypeProperties = null)
        {
            var compare = CreateComplexType(name, level, complexTypeProperties);
            compare.IsClosing = true;
            compare.IsDictionaryEntry = true;
            source.ShouldMatchData(compare);
        }

        private static DataDescription CreateComplexType(
            string name, int level, Action<ComplexTypeDsl> properties = null)
        {
            var complexType = new DataDescription
            {
                Name = name,
                IsComplexType = true,
                Whitespace = DataDescriptionFactory.Whitespace.Repeat(level)
            };
            if (properties != null) properties(new ComplexTypeDsl(complexType));
            return complexType;
        }

        public class ComplexTypeDsl
        {
            private readonly DataDescription _data;
            public ComplexTypeDsl(DataDescription data) { _data = data; }
            public ComplexTypeDsl Comments(string comments) { _data.Comments = comments; return this; }
            public ComplexTypeDsl Opening() { _data.IsOpening = true; return this; }
            public ComplexTypeDsl Closing() { _data.IsClosing = true; return this; }
            public ComplexTypeDsl First() { _data.IsFirst = true; return this; }
            public ComplexTypeDsl Last() { _data.IsLast = true; return this; }
        }

        // Common assertion DSL's

        public class DictionaryKeyDsl
        {
            private readonly Key _key;
            public DictionaryKeyDsl(DataDescription data) { _key = data.DictionaryKey; }
            public DictionaryKeyDsl KeyComments(string comments) { _key.Comments = comments; return this; }

            public OptionDsl KeyOptions
            {
                get { return new OptionDsl(_key.Options = _key.Options ?? new List<Option>()); }
            }
        }

        public class MemberDsl
        {
            private readonly DataDescription _data;
            public MemberDsl(DataDescription data) { _data = data; }
            public MemberDsl Comments(string comments) { _data.Comments = comments; return this; }
            public MemberDsl Required() { _data.Required = true; return this; }
            public MemberDsl Optional() { _data.Optional = true; return this; }
            public MemberDsl IsLastMember() { _data.IsLastMember = true; return this; }

            public MemberDsl IsDeprecated(string message = null)
            {
                _data.IsDeprecated = true;
                _data.DeprecationMessage = message;
                return this;
            }
        }

        public class OptionDsl
        {
            private readonly List<Option> _options;
            public OptionDsl(List<Option> options) { _options = options; }

            public OptionDsl WithOption(string value)
            {
                return WithOption(new Option { Value = value });
            }

            public OptionDsl WithOption(string name, string value)
            {
                return WithOption(new Option { Name = name, Value = value });
            }

            public OptionDsl WithOptionAndComments(string value, string comments)
            {
                return WithOption(new Option { Value = value, Comments = comments });
            }

            public OptionDsl WithOptionAndComments(string name, string value, string comments)
            {
                return WithOption(new Option { Name = name, Value = value, Comments = comments });
            }

            private OptionDsl WithOption(Option option)
            {
                _options.Add(option); return this;
            }
        }

        // Common assertions

        private static void ShouldMatchData(this DataDescription source, DataDescription compare)
        {
            source.Name.ShouldEqual(compare.Name);
            source.Comments.ShouldEqual(compare.Comments);
            source.IsFirst.ShouldEqual(compare.IsFirst);
            source.IsLast.ShouldEqual(compare.IsLast);
            source.TypeName.ShouldEqual(compare.TypeName);
            source.DefaultValue.ShouldEqual(compare.DefaultValue);
            source.Required.ShouldEqual(compare.Required);
            source.Optional.ShouldEqual(compare.Optional);
            source.Whitespace.ShouldEqual(compare.Whitespace);
            source.IsDeprecated.ShouldEqual(compare.IsDeprecated);
            source.DeprecationMessage.ShouldEqual(compare.DeprecationMessage);

            source.IsOpening.ShouldEqual(compare.IsOpening);
            source.IsClosing.ShouldEqual(compare.IsClosing);

            source.IsMember.ShouldEqual(compare.IsMember);
            source.IsLastMember.ShouldEqual(compare.IsLastMember);

            source.IsSimpleType.ShouldEqual(compare.IsSimpleType);
            source.IsString.ShouldEqual(compare.IsString);
            source.IsBoolean.ShouldEqual(compare.IsBoolean);
            source.IsNumeric.ShouldEqual(compare.IsNumeric);
            source.IsDateTime.ShouldEqual(compare.IsDateTime);
            source.IsDuration.ShouldEqual(compare.IsDuration);
            source.IsGuid.ShouldEqual(compare.IsGuid);
            compare.Options.ShouldEqualOptions(source.Options);

            source.IsComplexType.ShouldEqual(compare.IsComplexType);

            source.IsArray.ShouldEqual(compare.IsArray);

            source.IsDictionary.ShouldEqual(compare.IsDictionary);
            source.IsDictionaryEntry.ShouldEqual(compare.IsDictionaryEntry);

            if (compare.DictionaryKey == null) source.DictionaryKey.ShouldBeNull();
            else
            {
                source.DictionaryKey.TypeName.ShouldEqual(compare.DictionaryKey.TypeName);
                source.DictionaryKey.Comments.ShouldEqual(compare.DictionaryKey.Comments);
                source.DictionaryKey.Options.ShouldEqualOptions(compare.DictionaryKey.Options);
            }
        }

        private static void ShouldEqualOptions(this List<Option> source, List<Option> compare)
        {
            if (compare == null) source.ShouldBeNull();
            else
            {
                source.Count.ShouldEqual(compare.Count);
                foreach (var option in source.Zip(compare,
                    (s, c) => new { Source = s, Compare = c }))
                {
                    option.Source.Name.ShouldEqual(option.Compare.Name);
                    option.Source.Comments.ShouldEqual(option.Compare.Comments);
                    option.Source.Value.ShouldEqual(option.Compare.Value);
                }
            }
        }
    }
}
