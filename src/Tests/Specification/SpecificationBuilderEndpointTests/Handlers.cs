﻿using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using FubuMVC.Core;
using FubuMVC.Swank.Description;

namespace Tests.Specification.SpecificationBuilderEndpointTests
{
    namespace EndpointDescriptions
    {
        public class NoDescriptionGetHandler { public object Execute_NoDescription(object request) { return null; } }

        namespace HandlerDescription
        {
            public class EmbeddedDescriptionGetHandler { public object Execute_EmbeddedDescription(object request) { return null; } }

            public class Request { public Guid Id { get; set; } }

            [Description("Some get handler name", "Some get handler description")]
            public class GetHandler { public object Execute_Get_Id(Request request) { return null; } }

            [Description("Some post handler name", "Some post handler description")]
            public class PostHandler { public object Execute_Post(Request request) { return null; } }

            [Description("Some put handler name", "Some put handler description")]
            public class PutHandler { public object Execute_Put_Id(Request request) { return null; } }

            [Description("Some delete handler name", "Some delete handler description")]
            public class DeleteHandler { public object Execute_Delete_Id(Request request) { return null; } }
        }

        namespace ActionDescription
        {
            public class EmbeddedDescriptionGetHandler { public object Execute_EmbeddedDescription(object request) { return null; } }

            public class Request { public Guid Id { get; set; } }

            public class GetHandler
            {
                [Description("Some get action name", "Some get action description")]
                public object Execute_Get_Id(Request request) { return null; }
            }

            public class PostHandler
            {
                [Description("Some post action name", "Some post action description")]
                public object Execute_Post(Request request) { return null; }
            }

            public class PutHandler
            {
                [Description("Some put action name", "Some put action description")]
                public object Execute_Put_Id(Request request) { return null; }
            }

            public class DeleteHandler
            {
                [Description("Some delete action name", "Some delete action description")]
                public object Execute_Delete_Id(Request request) { return null; }
            }
        }
    }

    namespace HiddenEndpointAttributes
    {
        public class HiddenActionGetHandler
        {
            [Hide]
            public object Execute_HiddenAction(object request) { return null; } 
        }

        [Hide]
        public class HiddenGetHandler { public object Execute_Hidden(object request) { return null; } }
        public class VisibleGetHandler { public object Execute_Visible(object request) { return null; } }
    }

    namespace ControllerResource
    {
        [Resource("Some Controller")]
        public class Controller
        {
            public object Execute(object request) { return null; }
        }
    }

    namespace InputTypeDescriptions
    {
        [Comments("Some get request description")]
        public class GetRequest { }
        public class GetHandler { public object Execute_Get(GetRequest request) { return null; } }

        [Comments("Some post request description")]
        public class PostRequest {}
        public class PostHandler { public object Execute_Post(PostRequest request) { return null; } }

        [Comments("Some put request description")]
        public class PutRequest { }
        public class PutHandler { public object Execute_Put(PutRequest request) { return null; } }

        [Comments("Some delete request description")]
        public class DeleteRequest { }
        public class DeleteHandler { public object Execute_Delete(DeleteRequest request) { return null; } }

        public class RequestItem {}

        public class CollectionPostHandler { public object Execute_Collection(List<RequestItem> request) { return null; } }

        public class RequestItems : List<RequestItem> { }
        public class InheritedCollectionPostHandler { public object Execute_InheritedCollection(RequestItems request) { return null; } }

        [XmlType("NewItemName")]
        public class OverridenRequestItem { }

        public class OverridenRequestPostHandler { public object Execute_OverridenRequest(OverridenRequestItem request) { return null; } }

        [XmlType("NewCollectionName")]
        public class OverridenRequestItems : List<OverridenRequestItem> { }
        public class OverridenCollectionPostHandler { public object Execute_OverridenCollection(OverridenRequestItems request) { return null; } }
    }

    namespace OutputTypeDescriptions
    {
        [Comments("Some get response description")]
        public class GetResponse { }
        public class GetHandler { public GetResponse Execute_Get(object request) { return null; } }

        [Comments("Some post response description")]
        public class PostResponse { }
        public class PostHandler { public PostResponse Execute_Post(object request) { return null; } }

        [Comments("Some put response description")]
        public class PutResponse { }
        public class PutHandler { public PutResponse Execute_Put(object request) { return null; } }

        [Comments("Some delete response description")]
        public class DeleteResponse { }
        public class DeleteHandler { public DeleteResponse Execute_Delete(object request) { return null; } }

        public class ResponseItem { }

        public class CollectionPostHandler { public List<ResponseItem> Execute_Collection(object request) { return null; } }

        public class ResponseItems : List<ResponseItem> { }
        public class InheritedCollectionPostHandler { public ResponseItems Execute_InheritedCollection(object request) { return null; } }

        [XmlType("NewItemName")]
        public class OverridenResponseItem { }

        public class OverridenRequestPostHandler { public OverridenResponseItem Execute_OverridenRequest(object request) { return null; } }

        [XmlType("NewCollectionName")]
        public class OverridenResponseItems : List<OverridenResponseItem> { }
        public class OverridenCollectionPostHandler { public OverridenResponseItems Execute_OverridenCollection(object request) { return null; } }
    }

    namespace ErrorDescriptions
    {
        [ErrorDescription(411, "411 error on handler")]
        [ErrorDescription(410, "410 error on handler", "410 error on action description")]
        public class ErrorsGetHandler
        {
            [ErrorDescription(413, "413 error on action")]
            [ErrorDescription(412, "412 error on action", "412 error on action description")]
            public object Execute_Errors(object request) { return null; }
        }

        public class NoErrorsGetHandler
        {
            public object Execute_NoErrors(object request) { return null; }
        }
    }

    namespace Querystrings
    {
        public class Request
        {
            public Guid Id { get; set; }
            [QueryString]
            public string Sort { get; set; }
            [Comments("These are the revision numbers.")]
            public List<int> Revision { get; set; }
            [QueryString, Hide]
            public string HiddenParameter { get; set; }
            [QueryString, Required]
            public string RequiredParameter { get; set; }
            public string ContentType { get; set; }
        }

        public class ImplicitGetHandler { public object Execute_ImplicitGet_Id(Request request) { return null; } }
        public class ImplicitDeleteHandler { public object Execute_ImplicitDelete_Id(Request request) { return null; } }

        public class ExplicitPostHandler { public object Execute_ExplicitPost_Id(Request request) { return null; } }
        public class ExplicitPutHandler { public object Execute_ExplicitPut_Id(Request request) { return null; } }

        public enum Options
        {
            [Hide]
            Option2, 
            [Description("Option 1", "Option 1 description.")]
            Option1, 
            Option3
        }

        public class OptionRequest { public Options Options { get; set; } }
        public class OptionGetHandler { public object Execute_Option(OptionRequest request) { return null; } }
    }

    namespace UrlParameters
    {
        public class Request
        {
            [Comments("This the revision number.")]
            public int Revision { get; set; }
            public Guid WidgetId { get; set; }
            public string Sort { get; set; }
        }

        public class GetHandler { public object Execute_WidgetId_Revision(Request request) { return null; } }

        public enum Options
        {
            [Hide]
            Option2,
            [Description("Option 1", "Option 1 description.")]
            Option1,
            Option3
        }

        public class OptionRequest { public Options Options { get; set; } }
        public class OptionGetHandler { public object Execute_Option_Options(OptionRequest request) { return null; } }
    }
}
