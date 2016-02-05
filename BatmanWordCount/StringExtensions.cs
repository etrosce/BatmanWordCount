using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BatmanWordCount
{
    public static class StringExtensions
    {
        public static IEnumerable<string> ParseWords(this string text)
        {
            Regex regex = new Regex(@"([a-zA-Z]+)");

            foreach (Match match in regex.Matches(text))
            {
                yield return match.Value;
            }
        }
    }
}
