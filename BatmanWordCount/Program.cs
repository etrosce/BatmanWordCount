using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.IO;

namespace BatmanWordCount
{
    public class Program
    {
        private static IDictionary<string, int> wordCountMap = new Dictionary<string, int>();
        private static HashSet<string> links = new HashSet<string>() { "/wiki/Main_Page", "/wiki/Batman" };

        static void Main(string[] args)
        {
            int topN = 0;
            if (!args.Any() || !int.TryParse(args[0], out topN))
            {
                Console.WriteLine("Please specify a number of top words to list");
                Environment.Exit(-1);
            }

            Crawl("/wiki/Batman", 0, 2, uri =>
            {
                Task<string> htmlTask = GetHtml("https://en.wikipedia.org" + uri);
                Task.WaitAll(htmlTask);

                return htmlTask.Result;
            });

            foreach (var item in GetWordCount(topN))
            {
                Console.WriteLine($"{item.Key} - {item.Value}");
            }

            Console.ReadKey();
        }

        public static void Crawl(string uri, int currentDepth, int maxDepth, Func<string, string> htmlProvider)
        {
            string html = htmlProvider(uri);

            if (String.IsNullOrEmpty(html))
                return;

            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(html);

            HtmlNode content = htmlDoc.GetElementbyId("mw-content-text");

            if (content == null)
                return;

            string clearText = content.ToClearText();
            foreach (string word in clearText.ParseWords())
            {
                string lowerWord = word.ToLower();
                if (!wordCountMap.ContainsKey(lowerWord))
                    wordCountMap.Add(lowerWord, 1);
                else
                    wordCountMap[lowerWord]++;
            };

            if (currentDepth < maxDepth)
            {
                currentDepth++;
                foreach (var link in content.ParseLinks())
                {
                    if (!links.Contains(link))
                    {
                        links.Add(link);
                        Crawl(link, currentDepth, maxDepth, htmlProvider);
                    }
                }
            }
        }

        public static IEnumerable<KeyValuePair<string, int>>  GetWordCount(int topN)
        {
            return wordCountMap.OrderByDescending(w => w.Value).Take(topN);
        }

        private async static Task<string> GetHtml(string uri)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    return await httpClient.GetStringAsync(uri);
                }
                catch
                {
                    return String.Empty;
                }
            }
        }
    }
}
