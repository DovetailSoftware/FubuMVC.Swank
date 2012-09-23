﻿using System;
using System.Linq;

namespace FubuMVC.Swank.Extensions
{
    public static class ObjectExtensions
    {
        public class OtherwiseOptions<TResult>
        {
            private readonly object _value;
            private readonly Func<TResult> _returnThis;

            public OtherwiseOptions(object value, Func<TResult> returnThis)
            {
                _value = value;
                _returnThis = returnThis;
            }

            public TResult OtherwiseDefault()
            {
                return Otherwise(default(TResult));
            }

            public TResult Otherwise(params TResult[] values)
            {
                return _value != null ? _returnThis() : values.FirstOrDefault(x => x != null);
            }

            public OtherwiseOptions<TNextResult> WhenNotNull<TNextResult>(Func<TResult, TNextResult> returnThis)
            {
                return OtherwiseDefault().WhenNotNull(returnThis);
            }
        }

        public static OtherwiseOptions<TResult> WhenNotNull<TSource, TResult>(this TSource value, Func<TSource, TResult> returnThis)
        {
            return new OtherwiseOptions<TResult>(value, () => returnThis(value));
        }
    }
}