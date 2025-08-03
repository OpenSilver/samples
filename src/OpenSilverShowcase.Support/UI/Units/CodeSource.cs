using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace OpenSilverShowcase.Support.UI.Units;

public class CodeSource : INotifyPropertyChanged
{
    private bool _isSelected;
    private string _language;

    public string Key { get; set; } = "";

    public Uri Source { get; set; } = null!;
    public string ProjectName { get; set; }

    public string Code { get; set; } = "";

    public string Language
    {
        get => _language ?? DetectLanguageFromSource();
        set
        {
            _language = value;
            OnPropertyChanged();
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    private string DetectLanguageFromSource()
    {
        string sourceString = Source?.ToString() ?? "";
        string keyString = Key?.ToLowerInvariant() ?? "";

        if (!string.IsNullOrEmpty(sourceString))
        {
            try
            {
                var extension = Path.GetExtension(sourceString)?.ToLowerInvariant();
                var detectedLang = extension switch
                {
                    ".cs" => "csharp",
                    ".xaml" => "xml",
                    ".xml" => "xml",
                    ".html" => "html",
                    ".css" => "css",
                    ".js" => "javascript",
                    ".ts" => "typescript",
                    ".json" => "json",
                    ".sql" => "sql",
                    ".py" => "python",
                    ".java" => "java",
                    ".cpp" => "cpp",
                    ".c" => "c",
                    ".h" => "c",
                    _ => null
                };

                if (detectedLang != null)
                    return detectedLang;
            }
            catch
            {
            }
        }

        if (!string.IsNullOrEmpty(keyString))
        {
            if (keyString.Contains("xaml.cs")) return "csharp";
            if (keyString.Contains("xaml")) return "xml";
            if (keyString.Contains("cs") || keyString.Contains("c#")) return "csharp";
            if (keyString.Contains("js") || keyString.Contains("javascript")) return "javascript";
            if (keyString.Contains("ts") || keyString.Contains("typescript")) return "typescript";
            if (keyString.Contains("json")) return "json";
            if (keyString.Contains("css")) return "css";
            if (keyString.Contains("html")) return "html";
            if (keyString.Contains("sql")) return "sql";
            if (keyString.Contains("python") || keyString.Contains("py")) return "python";
        }

        return "plaintext";
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}