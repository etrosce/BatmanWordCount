using Microsoft.VisualStudio.TestTools.UnitTesting;
using BatmanWordCount;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BatmanWordCount;
using System.IO;

namespace BatmanWordCount.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public void ParseWordsTest()
        {
            string test = "Hi, this is a #1 test! (hopefully)";
            List<string> expected = new List<string> { "Hi", "this", "is", "a", "test", "hopefully"};

            List<string> parsed = test.ParseWords().ToList();
            Assert.AreEqual(expected.Count, parsed.Count);

            for (int i = 0; i < parsed.Count; i++)
            {
                Assert.AreEqual(expected[i], parsed[i]);
            }
        }

        [TestMethod()]
        public void ParseLinksTest()
        {
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.Load("Test.html");

            List<string> expected = new List<string> { "/wiki/page", "/wiki/page2"};
            List<string> parsed = htmlDoc.DocumentNode.ParseLinks().ToList();

            Assert.AreEqual(expected.Count, parsed.Count);

            for (int i = 0; i < parsed.Count; i++)
            {
                Assert.AreEqual(expected[i], parsed[i]);
            }
        }

        [TestMethod()]
        public void ToClearTextTest()
        {
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.Load("Test.html");

            string expected = "Test \r\n    Some text\r\n     \r\n    Some more text\r\n     \r\nText here and there ";
            string converted = htmlDoc.DocumentNode.ToClearText();

            Assert.AreEqual(expected, converted);
        }

        [TestMethod()]
        public void CrawlTest()
        {
            Program.Crawl("Test.html", 0, 1, uri =>
            {
                if (File.Exists(uri))
                    return File.ReadAllText(uri);
                else
                    return String.Empty;
            });

            List<KeyValuePair<string, int>> expected = new List<KeyValuePair<string, int>>
            {   new KeyValuePair<string, int>("text", 3),
                new KeyValuePair<string, int>("some", 2),
                new KeyValuePair<string, int>("more", 1),
            };

            List<KeyValuePair<string, int>> topWords = Program.GetWordCount(3).ToList();

            Assert.AreEqual(expected.Count, topWords.Count);

            for (int i = 0; i < topWords.Count; i++)
            {
                Assert.AreEqual(expected[i].Key, topWords[i].Key);
                Assert.AreEqual(expected[i].Value, topWords[i].Value);
            }
        }
    }
}