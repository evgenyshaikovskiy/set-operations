using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sets;

namespace SetOperations
{
    public static class ParsingConsts
    {
        public const char ParserSpcharEquals = '=';
        public const char ParserSpcharSetUnorderedBegin = '{';
        public const char ParserSpcharSetUnorderedEnd = '}';
        public const char ParserSpcharSetOrderedBegin = '<';
        public const char ParserSpcharSetOrderedEnd = '>';
        public const char ParserSpcharSetSep = ',';

        public const byte ParserTokenNone = 0;
        public const byte ParserTokenName = 1;
        public const byte ParserTokenEquals = 2;
        public const byte ParserTokenSetSep = 3;
        public const byte ParserTokenSetUnorderedBegin = 11;
        public const byte ParserTokenSetUnorderedEnd = 12;
        public const byte ParserTokenSetOrderedBegin = 13;
        public const byte ParserTokenSetOrderedEnd = 14;

        public const string FilePathInput = "..\\..\\..\\input.txt";

        public static readonly (char start, char end)[] ParserAllowedRanges = { ('0', '9'), ('A', 'Z'), ('a', 'z') };
        public static readonly char[] ParserAllowedSingles = "_={}<>,".ToCharArray();
    }
}
