using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Sets
{
    public interface ISetElement
    {
        public ISetElement this[int index] { get; }

        public bool Equals(ISetElement elem);

        public string ToString();

        public ISetElement Dupplicate();
    }

    public static class Consts
    {
        public const string StrSetUnorderedBegin = "{";
        public const string StrSetUnorderedEnd = "}";
        public const string StrSetOrderedBegin = "<";
        public const string StrSetOrderedEnd = ">";
        public const string StrSetUnorderedEmpty = "{}";
        public const string StrSetOrderedEmpty = "<>";

        public const string StrSetUnorderedSep = ", ";
        public const string StrSetOrderedSep = ", ";
    }
}
