﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Description
{
    public class MarkerConvention<TMarker> where TMarker : Description
    {
        private readonly static Func<Assembly, IList<TMarker>> GetCachedDescriptions =
            Func.Memoize<Assembly, IList<TMarker>>(a =>
                a.GetTypes().Where(x => typeof(TMarker).IsAssignableFrom(x) && x != typeof(TMarker)).Select(CreateDescription)
                    .OrderByDescending(x => x.GetType().Namespace).ThenBy(x => x.Name).Cast<TMarker>().ToList());

        public virtual IList<TMarker> GetDescriptions(Assembly assembly)
        {
            return GetCachedDescriptions(assembly);
        }

        private static Description CreateDescription(Type type)
        {
            var description = (Description) Activator.CreateInstance(type);
            if (string.IsNullOrEmpty(description.Comments)) 
                description.Comments = type.Assembly.FindTextResourceNamed(type.FullName);
            return description;
        }
    }
}