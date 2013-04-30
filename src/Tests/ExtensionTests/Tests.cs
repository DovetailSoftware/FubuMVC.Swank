﻿using System.Collections.Generic;
using System.Reflection;
using FubuMVC.Core.Registration.Routes;
using FubuMVC.Swank.Extensions;
using NSubstitute;
using NUnit.Framework;
using Should;
using System.Linq;

namespace Tests.ExtensionTests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void should_get_resource_from_pattern()
        {
            var route = Substitute.For<IRouteDefinition>();
            route.Pattern.Returns("{Yada}/books/{Id}/categories/{CategoryId}/classification/{ClassId}");
            route.GetRouteResource().ShouldEqual("/books/categories/classification");

            route.Pattern.Returns("{Yada}");
            route.GetRouteResource().ShouldEqual("/");
        }

        [Test]
        public void should_return_null_when_no_resource_found()
        {
            Assembly.GetExecutingAssembly().FindTextResourceNamed("Tests.ExtensionTests.EmbeddedResource").ShouldBeNull();
        }

        [Test]
        public void should_return_text_resource()
        {
            Assembly.GetExecutingAssembly().FindTextResourceNamed("Tests.ExtensionTests.EmbeddedTextResource").ShouldEqual("Some text yo!");
        }

        [Test]
        public void should_return_html_resource()
        {
            Assembly.GetExecutingAssembly().FindTextResourceNamed("Tests.ExtensionTests.EmbeddedHtmlResource").ShouldEqual("<b>Some html yo!</b>");
        }

        [Test]
        public void should_return_markdown_resource()
        {
            Assembly.GetExecutingAssembly().FindTextResourceNamed("Tests.ExtensionTests.EmbeddedMarkdownResource").ShouldEqual("<p><strong>Some markdown yo!</strong></p>");
        }

        private class Widgets : List<string> { }
        private class Widgets<T> : List<string> { }

        [Test]
        public void should_determine_if_an_type_is_a_list_type()
        {
            typeof(List<string>).IsListType().ShouldBeTrue();
            typeof(IList<string>).IsListType().ShouldBeTrue();
            typeof(Widgets).IsListType().ShouldBeFalse();
            typeof(Widgets<string>).IsListType().ShouldBeFalse();
        }

        [Test]
        public void should_determine_if_an_type_implements_a_list_type()
        {
            typeof(List<string>).ImplementsListType().ShouldBeTrue();
            typeof(IList<string>).ImplementsListType().ShouldBeFalse();
            typeof(Widgets).ImplementsListType().ShouldBeTrue();
            typeof(Widgets<string>).ImplementsListType().ShouldBeTrue();
        }

        [Test]
        public void should_determine_if_an_type_inherits_from_a_list()
        {
            typeof(List<string>).InheritsFromListType().ShouldBeFalse();
            typeof(IList<string>).InheritsFromListType().ShouldBeFalse();
            typeof(Widgets).InheritsFromListType().ShouldBeTrue();
            typeof(Widgets<string>).InheritsFromListType().ShouldBeTrue();
        }

        [Test]
        public void should_determine_if_a_type_is_a_list()
        {
            typeof(List<string>).IsList().ShouldBeTrue();
            typeof(IList<string>).IsList().ShouldBeTrue();
            typeof(Widgets).IsList().ShouldBeTrue();
            typeof(Widgets<string>).IsList().ShouldBeTrue();
        }

        [Test]
        public void should_return_types_list_interface()
        {
            typeof(List<string>).GetListInterface().ShouldEqual(typeof(IList<string>));
            typeof(IList<string>).GetListInterface().ShouldEqual(typeof(IList<string>));
            typeof(Widgets).GetListInterface().ShouldEqual(typeof(IList<string>));
            typeof(Widgets<string>).GetListInterface().ShouldEqual(typeof(IList<string>));
        }

        [Test]
        public void should_return_list_element_type()
        {
            typeof(List<string>).GetListElementType().ShouldEqual(typeof(string));
            typeof(IList<string>).GetListElementType().ShouldEqual(typeof(string));
            typeof(Widgets).GetListElementType().ShouldEqual(typeof(string));
            typeof(Widgets<string>).GetListElementType().ShouldEqual(typeof(string));
            typeof(string[]).GetListElementType().ShouldEqual(typeof(string));
            typeof(string).GetListElementType().ShouldBeNull();
        }

        [Test]
        public void should_outer_join()
        {
            var result = new[] {1, 2, 3}.OuterJoin(new[] {2, 3, 4}, x => x, (k, i) => i).ToList();

            result.Count.ShouldEqual(4);

            result[0].Count().ShouldEqual(1);
            result[0].First().ShouldEqual(1);

            result[1].Count().ShouldEqual(2);
            result[1].All(x => x == 2).ShouldBeTrue();

            result[2].Count().ShouldEqual(2);
            result[2].All(x => x == 3).ShouldBeTrue();

            result[3].Count().ShouldEqual(1);
            result[3].First().ShouldEqual(4);
        }

        class Node { 
            public Node Parent { get; set; } 
            public int Index { get; set; } 
        }

        [Test]
        public void should_traverse()
        {
            var tip = new Node { Index = 2, Parent = new Node { Index = 1, Parent = new Node { Index = 0 }}};
            var traversal = tip.Traverse(x => x.Parent).ToList();
            traversal.Count.ShouldEqual(3);
            traversal[0].Index.ShouldEqual(2);
            traversal[1].Index.ShouldEqual(1);
            traversal[2].Index.ShouldEqual(0);
        }

        enum SomeEnum { Oh, Hai }

        [Test]
        public void should_get_enum_name()
        {
            typeof(SomeEnum).GetXmlName().ShouldEqual("SomeEnum");    
        }

        [Test]
        public void should_get_nullable_enum_name()
        {
            typeof(SomeEnum?).GetXmlName().ShouldEqual("SomeEnum");
        }

        [Test]
        public void should_get_enum_values()
        {
            var values = typeof(SomeEnum).GetEnumOptions();
            values.Count().ShouldEqual(2);
            values[0].Name.ShouldEqual("Oh");
            values[1].Name.ShouldEqual("Hai");
        }

        [Test]
        public void should_get_nullable_enum_values()
        {
            var values = typeof(SomeEnum?).GetEnumOptions();
            values.Count().ShouldEqual(2);
            values[0].Name.ShouldEqual("Oh");
            values[1].Name.ShouldEqual("Hai");
        }
    }
}