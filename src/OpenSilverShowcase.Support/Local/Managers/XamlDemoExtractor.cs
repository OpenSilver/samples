using System;
using System.Linq;

namespace OpenSilverShowcase.Support.Local.Managers
{
    public static class XamlDemoExtractor
    {
        public static string ExtractDemoContentOnly(string xamlText)
        {
            string[] startTags = new[]
            {
                "<DemoContent>",
                "<ShowcaseItem.DemoContent>",
                "<units:ShowcaseItem.DemoContent>"
            };
            string[] endTags = new[]
            {
                "</DemoContent>",
                "</ShowcaseItem.DemoContent>",
                "</units:ShowcaseItem.DemoContent>"
            };
            foreach (var startTag in startTags)
            {
                foreach (var endTag in endTags)
                {
                    int startIndex = xamlText.IndexOf(startTag, StringComparison.OrdinalIgnoreCase);
                    int endIndex = xamlText.IndexOf(endTag, StringComparison.OrdinalIgnoreCase);
                    if (startIndex >= 0 && endIndex > startIndex)
                    {
                        startIndex += startTag.Length;
                        string inner = xamlText.Substring(startIndex, endIndex - startIndex);
                        return NormalizeIndentation(inner).Trim();
                    }
                }
            }
            return NormalizeIndentation(xamlText).Trim();
        }

        private static string NormalizeIndentation(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            var nonEmptyLines = lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
            if (!nonEmptyLines.Any())
                return input;

            int minIndent = nonEmptyLines
                .Select(line => line.TakeWhile(char.IsWhiteSpace).Count())
                .Min();

            var normalizedLines = lines.Select(line =>
            {
                if (string.IsNullOrWhiteSpace(line))
                    return line;
                return line.Substring(Math.Min(minIndent, line.Length));
            });

            return string.Join(Environment.NewLine, normalizedLines).TrimEnd();
        }
    }
}