﻿using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Description
{
    public class DescriptionBase
    {
        public DescriptionBase() {}

        public DescriptionBase(string name, string comments = null)
        {
            Name = name;
            Comments = comments;
        }

        public string Name { get; set; }
        public string Comments { get; set; }

        public override bool Equals(object obj)
        {
            return obj is DescriptionBase && ((DescriptionBase)obj).Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.WhenNotNull(x => x.GetHashCode()).Otherwise(0);
        }
    }
}
