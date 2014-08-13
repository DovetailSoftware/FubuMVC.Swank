﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Specification
{
    public class SpecificationService
    {
        private class BehaviorMapping
        {
            public ActionCall FirstAction { get; set; }
            public ModuleDescription Module { get; set; }
            public ResourceDescription Resource { get; set; }

            public BehaviorChain Chain { get; set; }
        }

        public static readonly Func<string, int> HttpVerbRank = x => { switch (x.IsEmpty() ? null : x.ToLower()) 
            { case "get": return 0; case "post": return 1; case "put": return 2; 
              case "update": return 3; case "delete": return 5; default: return 4; } };

        private readonly Configuration _configuration;
        private readonly BehaviorSource _behaviors;
        private readonly ITypeDescriptorCache _typeCache;
        private readonly IDescriptionConvention<BehaviorChain, ModuleDescription> _moduleConvention;
        private readonly IDescriptionConvention<BehaviorChain, ResourceDescription> _resourceConvention;
        private readonly IDescriptionConvention<BehaviorChain, EndpointDescription> _endpointConvention;
        private readonly IDescriptionConvention<PropertyInfo, MemberDescription> _memberConvention;
        private readonly IDescriptionConvention<FieldInfo, OptionDescription> _optionConvention;
        private readonly IDescriptionConvention<BehaviorChain, List<StatusCodeDescription>> _statusCodeConvention;
        private readonly IDescriptionConvention<BehaviorChain, List<HeaderDescription>> _headerConvention;
        private readonly IDescriptionConvention<System.Type, TypeDescription> _typeConvention;

        public SpecificationService(
            Configuration configuration, 
            BehaviorSource behaviors,
            ITypeDescriptorCache typeCache,
            IDescriptionConvention<BehaviorChain, ModuleDescription> moduleConvention,
            IDescriptionConvention<BehaviorChain, ResourceDescription> resourceConvention,
            IDescriptionConvention<BehaviorChain, EndpointDescription> endpointConvention,
            IDescriptionConvention<PropertyInfo, MemberDescription> memberConvention,
            IDescriptionConvention<FieldInfo, OptionDescription> optionConvention,
            IDescriptionConvention<BehaviorChain, List<StatusCodeDescription>> statusCodeConvention,
            IDescriptionConvention<BehaviorChain, List<HeaderDescription>> headerConvention,
            IDescriptionConvention<System.Type, TypeDescription> typeConvention)
        {
            _configuration = configuration;
            _behaviors = behaviors;
            _typeCache = typeCache;
            _moduleConvention = moduleConvention;
            _resourceConvention = resourceConvention;
            _endpointConvention = endpointConvention;
            _memberConvention = memberConvention;
            _optionConvention = optionConvention;
            _statusCodeConvention = statusCodeConvention;
            _typeConvention = typeConvention;
            _headerConvention = headerConvention;
        }

        public Specification Generate()
        {
            var behaviorMappings = GetBehaviorMapping(_behaviors.GetChains());

            CheckForOrphanedChains(behaviorMappings);

            var modules = GetModules(behaviorMappings.Where(x => x.Module != null).ToList());
            var resources = GetResources(behaviorMappings.Where(x => x.Module == null).ToList());
            if (resources.Any()) modules.Add(new Module
            {
                Name = Module.DefaultName,
                Resources = resources
            });

            var specification = new Specification {
                    Name = _configuration.Name,
                    Comments = _configuration.AppliesToAssemblies
                        .Select(x => x.FindTextResourceNamed("*" + _configuration.Comments))
                        .FirstOrDefault(x => x != null),
                    Modules = modules
                };
            return specification;
        }

        private List<BehaviorMapping> GetBehaviorMapping(IEnumerable<BehaviorChain> chains)
        {
            return chains
                .Where(x => !x.Calls.Any(c => c.Method.HasAttribute<HideAttribute>() || c.HandlerType.HasAttribute<HideAttribute>()))
                .Select(c => new { Chain = c, Module = _moduleConvention.GetDescription(c), Resource = _resourceConvention.GetDescription(c) })
                .Where(x => ((_configuration.OrphanedModuleActions == OrphanedActions.Exclude && x.Module != null) ||
                              _configuration.OrphanedModuleActions != OrphanedActions.Exclude))
                .Where(x => ((_configuration.OrphanedResourceActions == OrphanedActions.Exclude && x.Resource != null) ||
                              _configuration.OrphanedResourceActions != OrphanedActions.Exclude))
                .Select(x => new BehaviorMapping {
                    Chain = x.Chain,
                    FirstAction = x.Chain.FirstCall(),
                    Module = _configuration.OrphanedModuleActions == OrphanedActions.UseDefault
                        ? x.Module ?? _configuration.DefaultModuleFactory(x.Chain)
                        : x.Module,
                    Resource = _configuration.OrphanedResourceActions == OrphanedActions.UseDefault
                        ? x.Resource ?? _configuration.DefaultResourceFactory(x.Chain)
                        : x.Resource
                }).ToList();
        }

        private void CheckForOrphanedChains(IList<BehaviorMapping> behaviorMappings)
        {
            if (_configuration.OrphanedModuleActions == OrphanedActions.Fail)
            {
                var orphanedModuleActions = behaviorMappings.Where(x => x.Module == null).ToList();
                if (orphanedModuleActions.Any()) throw new OrphanedModuleActionException(
                    orphanedModuleActions.Select(x => x.FirstAction.HandlerType.FullName + "." + x.FirstAction.Method.Name));
            }

            if (_configuration.OrphanedResourceActions == OrphanedActions.Fail)
            {
                var orphanedActions = behaviorMappings.Where(x => x.Resource == null).ToList();
                if (orphanedActions.Any()) throw new OrphanedResourceActionException(
                    orphanedActions.Select(x => x.FirstAction.HandlerType.FullName + "." + x.FirstAction.Method.Name));
            }
        }

        private List<Type> GatherInputOutputModels(IEnumerable<BehaviorChain> chains)
        {
            var rootInputOutputModels = chains.SelectMany(RootInputAndOutputModels).ToList();

            return rootInputOutputModels
                .Concat(rootInputOutputModels.SelectMany(GetTypes))
                .DistinctBy(x => x.Type, x => x.Chain)
                .Select(cxt => {
                    var description = _typeConvention.GetDescription(cxt.Type);
                    var type = description.WhenNotNull(y => y.Type).Otherwise(cxt.Type);

                    return _configuration.TypeOverrides.Apply(cxt.Type, new Type {
                        Id = cxt.Chain != null
                            ? _configuration.InputTypeIdConvention(type, cxt.Chain.FirstCall().Method)
                            : _configuration.TypeIdConvention(type),
                        Name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                        Comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                        Members = GetMembers(type, cxt.Chain)
                    });
                })
                .OrderBy(x => x.Name).ToList();
        }

        private List<TypeContext> GetTypes(TypeContext type)
        {
            var properties = type.Type.IsProjection()
                ? type.Type.GetProjectionProperties()
                : _typeCache.GetPropertiesFor(type.Type).Select(x => x.Value);

            var types = properties
                .Where(x => !x.IsHidden() &&
                            !(x.PropertyType.GetListElementType() ?? x.PropertyType).IsSystemType() && 
                            !x.PropertyType.IsEnum &&
                            !x.IsAutoBound())
                .Select(x => new TypeContext(x.PropertyType.GetListElementType() ?? x.PropertyType, type))
                .DistinctBy(x => x.Type, x => x.Chain)
                .ToList();

            return types.Concat(types
                            .Where(x => type.Traverse(y => y.Parent).All(y => y.Type != x.Type))
                            .SelectMany(GetTypes))
                        .DistinctBy(x => x.Type, x => x.Chain)
                        .ToList();
        }

        private List<Member> GetMembers(System.Type type, BehaviorChain chain)
        {
            var action = chain != null ? chain.FirstCall() : null;

            var properties = type.IsProjection()
                ? type.GetProjectionProperties()
                : _typeCache.GetPropertiesFor(type).Select(x => x.Value);

            Func<System.Type, string> getType = x =>
                    x.IsSystemType() || x.IsEnum
                        ? x.GetXmlName()
                        : _configuration.TypeIdConvention(x);

            return properties
                .Where(x => !x.IsHidden() &&
                            !x.IsAutoBound() &&
                            !x.IsQuerystring(action) &&
                            !x.IsUrlParameter(action))
                .Select(x => {
                        var description = _memberConvention.GetDescription(x);
                        return _configuration.MemberOverrides.Apply(x, 
                            new Member 
                            {
                                Name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                                Comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                                DefaultValue = description.WhenNotNull(y => y.DefaultValue).WhenNotNull(z => z.ToDefaultValueString(_configuration)).OtherwiseDefault(),
                                Required = description.WhenNotNull(y => y.Required).OtherwiseDefault(),
                                Type = getType(description.Type),
                                IsArray = description.IsArray,
                                ArrayItemName = description.WhenNotNull(y => y.ArrayItemName).OtherwiseDefault(),
                                IsDictionary = description.IsDictionary,
                                Key = description.IsDictionary ? getType(description.DictionaryKeyType) : "",
                                Options = GetOptions(description.Type)
                            });
                    })
                .ToList();
        } 

        private List<Module> GetModules(IEnumerable<BehaviorMapping> behaviorMappings)
        {
            return behaviorMappings
                .GroupBy(x => x.Module)
                .Select(x => _configuration.ModuleOverrides.Apply(new Module {
                    Name = x.Key.Name,
                    Comments = x.Key.Comments,
                    Resources = GetResources(x.Select(y => y).ToList())
                }))
                .OrderBy(x => x.Name).ToList();
        }

        private List<Resource> GetResources(IEnumerable<BehaviorMapping> behaviorMappings)
        {
            return behaviorMappings
                .GroupBy(x => x.Resource)
                .Select(x => _configuration.ResourceOverrides.Apply(new Resource {
                    Name = x.Key.Name,
                    Comments = x.Key.Comments ?? x.First().FirstAction.HandlerType.Assembly
                        .FindTextResourceNamed(x.First().FirstAction.HandlerType.Namespace + ".resource"),
                    Endpoints = GetEndpoints(x.Select(y => y.Chain))
                }))
                .OrderBy(x => x.Name).ToList();
        }

        private List<Endpoint> GetEndpoints(IEnumerable<BehaviorChain> chains)
        {
            return chains
                .Select(chain => {
                    var endpoint = _endpointConvention.GetDescription(chain);
                    var route = chain.Route;
                    var querystring = chain.FirstCall().HasInput ? GetQuerystringParameters(chain) : null;
                    return _configuration.EndpointOverrides.Apply(chain, new Endpoint {
                        Name = endpoint.WhenNotNull(y => y.Name).OtherwiseDefault(),
                        Comments = endpoint.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                        Url = route.Pattern.EnusureStartsWith("/") + querystring.Join(y => "{0}={{{0}}}".ToFormat(y.Name), "?", "&", ""),
                        Method = route.AllowedHttpMethods.FirstOrDefault(),
                        UrlParameters = chain.FirstCall().HasInput ? GetUrlParameters(chain) : null,
                        QuerystringParameters = querystring,
                        StatusCodes = GetStatusCodes(chain),
                        Headers = GetHeaders(chain),
                        Request = chain.FirstCall().HasInput && (route.AllowsPost() || route.AllowsPut())
                            ? _configuration.RequestOverrides.Apply(chain, GetRequestData(chain.InputType(), endpoint))
                            : null,
                        Response = chain.LastCall().HasOutput
                            ? _configuration.ResponseOverrides.Apply(chain, GetResponseData(chain.LastCall().OutputType(), endpoint))
                            : null
                    });
                }).OrderBy(x => x.Url.Split('?').First()).ThenBy(x => HttpVerbRank(x.Method)).ToList();
        }

        private Data GetRequestData(BehaviorChain chain, EndpointDescription endpoint)
        {
            var firstCall = chain.FirstCall();

            if (!firstCall.HasInput ||
                chain.Route.AllowsGet() ||
                chain.Route.AllowsDelete()) return null;

            var inputType = firstCall.InputType();
            return GetData(inputType, endpoint.RequestComments, chain.FirstCall().Method);
        }

        private Data GetResponseData(BehaviorChain chain, EndpointDescription endpoint)
        {
            var lastCall = chain.LastCall();

            if (!lastCall.HasOutput) return null;

            var outputType = lastCall.OutputType();
            return GetData(outputType, endpoint.ResponseComments);
        }

        private Data GetData(System.Type type, string comments, MethodInfo method = null)
        {
            var dataType = _typeConvention.GetDescription(type);
            return new Data
            {
                    Name = dataType.WhenNotNull(y => y.Name).OtherwiseDefault(),
                    Comments = comments,
                    IsArray = type.IsArray || type.IsList(),
                    //IsDictionary = ,
                    //Key = ,
                    //Schema = 
            };
        }

        private List<TypeContext> WalkTypeGraph(System.Type type)
        {
            type = type.GetListElementType() ?? type;
            var properties = type.IsProjection()
                ? type.GetProjectionProperties()
                : _typeCache.GetPropertiesFor(type).Select(x => x.Value);

            var types = properties
                .Where(x => !x.IsHidden() &&
                            !(x.PropertyType.GetListElementType() ?? x.PropertyType).IsSystemType() &&
                            !x.PropertyType.IsEnum &&
                            !x.IsAutoBound())
                .Select(x => new TypeContext(x.PropertyType.GetListElementType() ?? x.PropertyType, type))
                .DistinctBy(x => x.Type, x => x.Chain)
                .ToList();

            return types.Concat(types
                            .Where(x => type.Traverse(y => y.Parent).All(y => y.Type != x.Type))
                            .SelectMany(GetTypes))
                        .DistinctBy(x => x.Type, x => x.Chain)
                        .ToList();
        }

        private List<UrlParameter> GetUrlParameters(BehaviorChain chain)
        {
            var action = chain.FirstCall();

            var properties = _typeCache.GetPropertiesFor(action.InputType());
            return action.ParentChain().Route.Input.RouteParameters.Select(
                x => {
                    var property = properties[x.Name];
                    var description = _memberConvention.GetDescription(property);
                    return _configuration.UrlParameterOverrides.Apply(chain, property, new UrlParameter {
                            Name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                            Comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                            Type = property.PropertyType.GetXmlName(),
                            Options = GetOptions(property.PropertyType)
                        });
                }).ToList();
        }

        private List<QuerystringParameter> GetQuerystringParameters(BehaviorChain chain)
        {
            var action = chain.FirstCall();

            return _typeCache.GetPropertiesFor(action.InputType())
                .Where(x => x.Value.IsQuerystring(action) && 
                            !x.Value.HasAttribute<HideAttribute>() && 
                            !x.Value.IsAutoBound())
                .Select(x => {
                    var description = _memberConvention.GetDescription(x.Value);
                    return _configuration.QuerystringOverrides.Apply(chain, x.Value, new QuerystringParameter {
                        Name = description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                        Comments = description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                        Type = (x.Value.PropertyType.GetListElementType() ?? x.Value.PropertyType).GetXmlName(),
                        Options = GetOptions(x.Value.PropertyType),
                        DefaultValue = description.DefaultValue.WhenNotNull(y => y.ToDefaultValueString(_configuration)).OtherwiseDefault(),
                        MultipleAllowed = x.Value.PropertyType.IsArray || x.Value.PropertyType.IsList(),
                        Required = description.Required
                    });
                }).OrderBy(x => x.Name).ToList();
        }

        private List<StatusCode> GetStatusCodes(BehaviorChain chain)
        {
            return _statusCodeConvention.GetDescription(chain)
                .Select(x => _configuration.StatusCodeOverrides.Apply(chain, new StatusCode {
                    Code = x.Code,
                    Name = x.Name,
                    Comments = x.Comments
                })).OrderBy(x => x.Code).ToList();
        }

        private List<Header> GetHeaders(BehaviorChain chain)
        {
            return _headerConvention.GetDescription(chain)
                .Select(x => _configuration.HeaderOverrides.Apply(chain, new Header {
                    Type = x.Type.ToString(),
                    Name = x.Name,
                    Comments = x.Comments,
                    Optional = x.Optional
                })).OrderBy(x => x.Type).ThenBy(x => x.Name).ToList();
        }

        private List<Option> GetOptions(System.Type type)
        {
            return type.IsEnum || (type.IsNullable() && Nullable.GetUnderlyingType(type).IsEnum) ? 
                type.GetEnumOptions()
                    .Where(x => !x.HasAttribute<HideAttribute>())
                    .Select(x => {
                        var option = _optionConvention.GetDescription(x);
                        return _configuration.OptionOverrides.Apply(x, new Option {
                            Name = option.WhenNotNull(y => y.Name).OtherwiseDefault(),
                            Comments = option.WhenNotNull(y => y.Comments).OtherwiseDefault(), 
                            Value = _configuration.EnumValue == EnumValue.AsString ? x.Name : x.GetRawConstantValue().ToString()
                        });
                    }).OrderBy(x => x.Name).ToList()
             : new List<Option>();
        }
    }
}
