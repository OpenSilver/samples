using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OpenSilverShowcase.Support.Local.Utils;

public static class EmbeddedCodeLoader
{
    public static string Load(string fileName, Assembly? sourceAssembly = null)
    {
        var asm = sourceAssembly ?? Assembly.GetCallingAssembly();
        var resource = asm.GetManifestResourceNames()
                          .FirstOrDefault(n => n.EndsWith(fileName, StringComparison.OrdinalIgnoreCase));

        if (resource == null) return $"[Not found: {fileName}]";

        using var stream = asm.GetManifestResourceStream(resource);
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

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


    public static string NormalizeIndentation(string input)
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

public static class EmbeddedUriResolver
{
    private static string BaseNamespace = "OpenSilverShowcase.Buttons.ShowcaseCodes";

    public static string ToEmbeddedPath(Uri source, Assembly assem)
    {
        BaseNamespace = $"{assem.GetName().Name}.ShowcaseCodes";

        if (source == null || (!source.IsAbsoluteUri && string.IsNullOrEmpty(source.OriginalString)))
            return "";

        var uri = source.OriginalString.TrimStart('/');
        uri = uri.Replace($"{assem.GetName().Name};component", "").TrimStart('/');

        var parts = uri.Split('/');
        if (parts.Length == 0) return "";

        var fileName = parts[parts.Length - 1]; 

        string fileNameWithoutExtension;
        string suffix;

        if (fileName.EndsWith(".xaml.cs", StringComparison.OrdinalIgnoreCase))
        {
            fileNameWithoutExtension = Path.GetFileNameWithoutExtension(
                Path.GetFileNameWithoutExtension(fileName)
            );
            suffix = ".xaml_XamlCs.txt";
        }
        else if (fileName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
        {
            fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            suffix = "_cs.txt";
        }
        else if (fileName.EndsWith(".xaml", StringComparison.OrdinalIgnoreCase))
        {
            fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            suffix = "_Xaml.txt";
        }
        else if (fileName.EndsWith(".viewmodel", StringComparison.OrdinalIgnoreCase) || fileName.EndsWith(".vm", StringComparison.OrdinalIgnoreCase))
        {
            fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            suffix = "_ViewModel.txt";
        }
        else
        {
            fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            suffix = ".txt";
        }

        var folderParts = parts.Take(parts.Length - 1).ToArray();
        var folder = string.Join(".", folderParts);

        return $"{BaseNamespace}.{folder}.{fileNameWithoutExtension}{suffix}";
    }
}

