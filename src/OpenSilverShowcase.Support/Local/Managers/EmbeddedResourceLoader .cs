using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenSilverShowcase.Support.Local.Managers
{
    public class EmbeddedResourceLoader
    {
        private readonly Assembly _assembly;

        public EmbeddedResourceLoader(Assembly assembly)
        {
            _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        public string LoadText(string resourceName)
        {
            try
            {
                using var stream = _assembly.GetManifestResourceStream(resourceName);
                if (stream == null)
                {
                    LogWarning($"리소스를 찾을 수 없습니다: {resourceName}");
                    return null;
                }

                using var reader = new StreamReader(stream, Encoding.UTF8);
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                LogError($"리소스 로드 실패 - {resourceName}: {ex.Message}");
                return null;
            }
        }

        public string LoadTextByPath(Uri source)
        {
            var resourceName = ConvertPathToResourceName(source);
            return LoadText(resourceName);
        }

        public Dictionary<string, string> LoadMultipleTexts(IEnumerable<string> resourceNames)
        {
            var results = new Dictionary<string, string>();

            foreach (var resourceName in resourceNames)
            {
                var content = LoadText(resourceName);
                if (content != null)
                {
                    results[resourceName] = content;
                }
            }

            return results;
        }

        public Dictionary<string, string> LoadResourcesByPattern(string pattern)
        {
            var matchingResources = GetResourceNames()
                .Where(name => IsMatchPattern(name, pattern))
                .ToList();

            return LoadMultipleTexts(matchingResources);
        }

        public string[] GetResourceNames()
        {
            try
            {
                return _assembly.GetManifestResourceNames();
            }
            catch (Exception ex)
            {
                LogError($"리소스 목록 조회 실패: {ex.Message}");
                return Array.Empty<string>();
            }
        }

        public string[] GetResourceNamesByFolder(string folderName)
        {
            return GetResourceNames()
                .Where(name => name.Contains($".{folderName}."))
                .ToArray();
        }

        public string[] GetResourceNamesByExtension(string extension)
        {
            var suffix = extension.StartsWith(".") ? extension : $".{extension}";
            return GetResourceNames()
                .Where(name => name.EndsWith(suffix))
                .ToArray();
        }

        public bool ResourceExists(string resourceName)
        {
            return GetResourceNames().Contains(resourceName);
        }

        private string ConvertPathToResourceName(Uri source)
        {
            if (source == null || (!source.IsAbsoluteUri && string.IsNullOrEmpty(source.OriginalString)))
                return "";

            var uri = source.OriginalString.TrimStart('/');
            uri = uri.Replace($"{_assembly.GetName().Name};component", "").TrimStart('/');

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

            var baseNamespace = $"{_assembly.GetName().Name}.ShowcaseCodes";
            return string.IsNullOrEmpty(folder)
                ? $"{baseNamespace}.{fileNameWithoutExtension}{suffix}"
                : $"{baseNamespace}.{folder}.{fileNameWithoutExtension}{suffix}";
        }

        private bool IsMatchPattern(string text, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
                return true;

            if (pattern == "*")
                return true;

            if (pattern.Contains("*"))
            {
                var parts = pattern.Split('*');
                var currentIndex = 0;

                foreach (var part in parts)
                {
                    if (string.IsNullOrEmpty(part))
                        continue;

                    var index = text.IndexOf(part, currentIndex);
                    if (index == -1)
                        return false;

                    currentIndex = index + part.Length;
                }

                return true;
            }

            return text.Contains(pattern);
        }

        private void LogWarning(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[EmbeddedResourceLoader Warning] {message}");
        }

        private void LogError(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[EmbeddedResourceLoader Error] {message}");
        }
    }
}
