﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using YantraJS.Core;
using YantraJS.Core.FastParser;

namespace YantraJS.Core.Tests.Parser
{
    internal static class ListExtensions
    {
        public static bool SequenceEqual<T>(this IList<T> list, params T[] items)
        {
            return Enumerable.SequenceEqual(list, items);
        }
    }

    [TestClass]
    public class ScannerTests
    {
        public IList<TokenTypes> ScanTypes(string text)
        {
            var pool = new FastPool();
            var s = new FastScanner(pool, text);
            var list = new SparseList<TokenTypes>();
            FastToken token;
            do {
                token = s.Token;
                list.Add(token.Type);
                s.ConsumeToken();
            } while (token.Type != TokenTypes.EOF);
            return list;
        }

        [TestMethod]
        public void String()
        {
            ScanTypes("'a'").SequenceEqual(TokenTypes.String);
            ScanTypes("'\"a'").SequenceEqual(TokenTypes.String);
            ScanTypes("'''a'").SequenceEqual(TokenTypes.String);
            ScanTypes(" '''a'").SequenceEqual(TokenTypes.String);
        }
        [TestMethod]
        public void Number()
        {
            ScanTypes("0").SequenceEqual(TokenTypes.Number);
            ScanTypes("0x1a").SequenceEqual(TokenTypes.Number);
            ScanTypes("0x11").SequenceEqual(TokenTypes.Number);
            ScanTypes("0b11").SequenceEqual(TokenTypes.Number);
            ScanTypes("12_12").SequenceEqual(TokenTypes.Number);
            ScanTypes(" 12_12 ").SequenceEqual(TokenTypes.Number);
            ScanTypes(" 12.1_12 ").SequenceEqual(TokenTypes.Number);
            ScanTypes(" /* asdfd */ 12.1_12 ").SequenceEqual(TokenTypes.Number);
        }

        [TestMethod]
        public void Operators()
        {
            ScanTypes("1+1").SequenceEqual(TokenTypes.Number, TokenTypes.Plus, TokenTypes.Number);
            ScanTypes("1++").SequenceEqual(TokenTypes.Number, TokenTypes.Increment);
        }

        [TestMethod]
        public void Template()
        {
            ScanTypes("`a`").SequenceEqual(TokenTypes.TemplateEnd);
            ScanTypes("`a${1}`").SequenceEqual(TokenTypes.TemplateBegin, TokenTypes.Number,  TokenTypes.TemplateEnd);
        }

        [TestMethod]
        public void RegExp()
        {
            ScanTypes("/a/").SequenceEqual(TokenTypes.RegExLiteral);
            ScanTypes("/=a/").SequenceEqual(TokenTypes.RegExLiteral);
            ScanTypes("a/=a").SequenceEqual(TokenTypes.Identifier, TokenTypes.AssignDivide ,  TokenTypes.Identifier);
            ScanTypes("//a/").SequenceEqual(TokenTypes.EOF);
        }

    }
}