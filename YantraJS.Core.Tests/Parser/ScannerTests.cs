using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        public static string CSV<T>(this IList<T> list) => string.Join(",", list.Select(x => x.ToString()));

        public static void AssertSequenceEqual<T>(this IList<T> list, params T[] items) {
            if (!Enumerable.SequenceEqual(list, items))
                Assert.Fail($"{list.CSV()} does not match {items.CSV()}");
        }

        public static void AssertSequenceEqual(
            this IList<TokenTypes> list, 
            params TokenTypes[] items) {
            var compare = new List<TokenTypes>(items);
            compare.Add(TokenTypes.EOF);
            if (!Enumerable.SequenceEqual(list, compare))
                Assert.Fail($"{list.CSV()} does not match {compare.CSV()}");
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
            ScanTypes("a\n").AssertSequenceEqual(TokenTypes.Identifier, TokenTypes.LineTerminator);
            ScanTypes("'a'\n").AssertSequenceEqual(TokenTypes.String, TokenTypes.LineTerminator);
            ScanTypes("'a'").AssertSequenceEqual(TokenTypes.String);
            ScanTypes("'\"a'").AssertSequenceEqual(TokenTypes.String);
            ScanTypes("'\\'a'").AssertSequenceEqual(TokenTypes.String);
            ScanTypes(" '\\'a'").AssertSequenceEqual(TokenTypes.String);
        }
        [TestMethod]
        public void Number()
        {
            ScanTypes(" 12.1_12\n").AssertSequenceEqual(TokenTypes.Number, TokenTypes.LineTerminator);
            ScanTypes("0").AssertSequenceEqual(TokenTypes.Number);
            ScanTypes("0x1a").AssertSequenceEqual(TokenTypes.Number);
            ScanTypes("0x11").AssertSequenceEqual(TokenTypes.Number);
            ScanTypes("0b11").AssertSequenceEqual(TokenTypes.Number);
            ScanTypes("12_12").AssertSequenceEqual(TokenTypes.Number);
            ScanTypes(" 12_12 ").AssertSequenceEqual(TokenTypes.Number);
            ScanTypes(" 12.1_12 ").AssertSequenceEqual(TokenTypes.Number);
            ScanTypes(" 12.1_12\n").AssertSequenceEqual(TokenTypes.Number, TokenTypes.LineTerminator);
            ScanTypes(" /* asdfd */ 12.1_12 ").AssertSequenceEqual(TokenTypes.Number);
            ScanTypes(" /* asdfd */ 12.1_12\n").AssertSequenceEqual(TokenTypes.Number, TokenTypes.LineTerminator);
            ScanTypes(" /* asdfd */ a = 12.1_12\n").AssertSequenceEqual(
                TokenTypes.Identifier, 
                TokenTypes.Assign,
                TokenTypes.Number, TokenTypes.LineTerminator);
        }

        [TestMethod]
        public void Operators()
        {
            ScanTypes("1+1").AssertSequenceEqual(TokenTypes.Number, TokenTypes.Plus, TokenTypes.Number);
            ScanTypes("1++").AssertSequenceEqual(TokenTypes.Number, TokenTypes.Increment);
        }

        [TestMethod]
        public void Template()
        {
            // ScanTypes("`a`").AssertSequenceEqual(TokenTypes.TemplateEnd);
            ScanTypes("`a${1}`").AssertSequenceEqual(
                TokenTypes.TemplateBegin, 
                TokenTypes.Number,
                TokenTypes.TemplateEnd);
        }

        [TestMethod]
        public void RegExp()
        {
            ScanTypes("/a/").AssertSequenceEqual(TokenTypes.RegExLiteral);
            ScanTypes("/=a/").AssertSequenceEqual(TokenTypes.RegExLiteral);
            ScanTypes("a/=a").AssertSequenceEqual(TokenTypes.Identifier, TokenTypes.AssignDivide ,  TokenTypes.Identifier);
            // ScanTypes("//a/").AssertSequenceEqual(TokenTypes.EOF, TokenTypes.EOF);
        }

    }
}
