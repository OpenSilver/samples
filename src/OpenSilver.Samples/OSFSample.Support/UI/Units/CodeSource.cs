using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace OSFSample.Support.UI.Units;

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
        // Source URI 또는 Key로부터 언어 자동 감지
        string sourceString = Source?.ToString() ?? "";
        string keyString = Key?.ToLowerInvariant() ?? "";

        // 먼저 Source 경로의 확장자로 감지 (가장 정확)
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
                // URI 파싱 실패 시 Key로 넘어감
            }
        }

        // Source로 감지 실패 시 Key로 감지 (더 구체적인 것부터 검사)
        if (!string.IsNullOrEmpty(keyString))
        {
            // 구체적인 조합부터 먼저 검사
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