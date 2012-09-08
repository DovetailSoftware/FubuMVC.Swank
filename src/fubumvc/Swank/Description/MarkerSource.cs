﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MarkdownSharp;

namespace Swank.Description
{
    public class MarkerSource<TMarker> where TMarker : Description
    {
        private readonly static Func<Assembly, IList<TMarker>> GetCachedDescriptions =
            Func.Memoize<Assembly, IList<TMarker>>(a =>
                a.GetTypes().Where(x => typeof(TMarker).IsAssignableFrom(x) && x != typeof(TMarker)).Select(CreateDescription)
                    .OrderByDescending(x => x.GetType().Namespace).ThenBy(x => x.Name).Cast<TMarker>().ToList());

        private readonly static Func<Assembly, string[]> GetEmbeddedResources =
            Func.Memoize<Assembly, string[]>(a => a.GetManifestResourceNames());

        public IList<TMarker> GetDescriptions(Assembly assembly)
        {
            return GetCachedDescriptions(assembly);
        }

        private static Description CreateDescription(Type type)
        {
            var description = (Description) Activator.CreateInstance(type);
            description.Namespace = type.Namespace;
            description.AppliesTo = type.BaseType.GetGenericArguments().FirstOrDefault();
            if (string.IsNullOrEmpty(description.Comments))
            {
                var resourceName = GetEmbeddedResources(type.Assembly).FirstOrDefault(
                    x => new[] {".txt", ".html", ".md"}.Any(y => type.FullName + y == x));
                if (resourceName != null)
                {
                    var comments = type.Assembly.GetManifestResourceStream(resourceName).ReadToEnd();
                    if (resourceName.EndsWith(".txt") || resourceName.EndsWith(".html"))
                        description.Comments = comments;
                    else if (resourceName.EndsWith(".md"))
                        description.Comments = new Markdown().Transform(comments).Trim();
                } 
            }
            return description;
        }
    }
}