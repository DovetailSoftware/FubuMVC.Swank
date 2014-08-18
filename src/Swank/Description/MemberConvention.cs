﻿using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Description
{
    public class MemberConvention : IDescriptionConvention<PropertyInfo, MemberDescription>
    {
        public virtual MemberDescription GetDescription(PropertyInfo property)
        {
            return new MemberDescription {
                Name = property.GetCustomAttribute<XmlElementAttribute>()
                            .WhenNotNull(x => x.ElementName)
                            .Otherwise(property.GetCustomAttribute<DataMemberAttribute>()
                                .WhenNotNull(x => x.Name)
                                .Otherwise(property.Name)),
                Comments = property.GetCustomAttribute<CommentsAttribute>()
                                   .WhenNotNull(x => x.Comments).OtherwiseDefault(),
                DefaultValue = property.GetCustomAttribute<DefaultValueAttribute>()
                                       .WhenNotNull(x => x.Value).OtherwiseDefault(),
                Required = !property.HasAttribute<OptionalAttribute>() && !property.PropertyType.IsNullable(),
                ArrayItem = new Description
                {
                    Name = property.GetAttribute<XmlArrayItemAttribute>()
                                   .WhenNotNull(x => x.ElementName).OtherwiseDefault(),
                    Comments = 
                },
                DictionaryEntry = new DictionaryDescription
                {
                    Key = new Description
                    {
                        Name = ,
                        Comments = 
                    },
                    Value = new Description
                    {
                        Name = ,
                        Comments = 
                    }
                }
            };
        }
    }
}