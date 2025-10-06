using OpenSilverShowcase.Support.UI.Units;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OpenSilverSample
{
    public static class ShowcaseExtractor
    {
        public static void ExtractAndSaveToFile()
        {
            var code = ExtractAllControlItemsAsCode();

            var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "ExtractedShowcaseItems.txt");
            outputPath = Path.GetFullPath(outputPath);

            try
            {
                File.WriteAllText(outputPath, code, Encoding.UTF8);
                Debug.WriteLine($"[Extractor] 파일 저장 완료: {outputPath}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Extractor] 파일 저장 실패: {ex.Message}");
            }
        }

        public static string ExtractAllControlItemsAsCode()
        {
            var items = ExtractAllControlItems();
            var sb = new StringBuilder();

            sb.AppendLine("        _allControlItems =");
            sb.AppendLine("        [");
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();

            foreach (var item in items)
            {
                sb.AppendLine($"            {item},");
            }

            if (sb.Length > 0)
            {
                sb.Length -= 3;
                sb.AppendLine();
            }

            sb.AppendLine("        ];");

            return sb.ToString();
        }

        public static List<string> ExtractAllControlItems()
        {
            var allControlItemStrings = new List<string>();

            try
            {
                Debug.WriteLine($"[Extractor] 현재 로드된 어셈블리에서 ShowcaseContent 검색 시작...");

                var assemblies = AppDomain.CurrentDomain.GetAssemblies();

                Debug.WriteLine($"[Extractor] 총 {assemblies.Length}개의 어셈블리 검색 중...");

                foreach (var assembly in assemblies)
                {
                    if (assembly.GetName().Name == "OpenSilverShowcase.Support")
                        continue;

                    ProcessAssembly(assembly, allControlItemStrings);
                }

                Debug.WriteLine($"[Extractor] 최종 Control Items 문자열 추출 완료. 총 {allControlItemStrings.Count}개.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Extractor] 치명적인 오류 발생: {ex.Message}\n{ex.ToString()}");
            }

            return allControlItemStrings;
        }

        private static void ProcessAssembly(Assembly assembly, List<string> allControlItemStrings)
        {
            string assemblyName = assembly.GetName().Name;

            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                Debug.WriteLine($"[Extractor][WARNING] {assemblyName}: 일부 타입을 로드하지 못했습니다.");
                types = ex.Types.Where(t => t != null).ToArray();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Extractor][ERROR] {assemblyName} 타입 로드 중 오류: {ex.Message}");
                return;
            }

            var showcaseContentTypes = types
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    typeof(ShowcaseContent).IsAssignableFrom(t)
                ).ToList();

            if (showcaseContentTypes.Count == 0)
            {
                return;
            }

            Debug.WriteLine($"[Extractor][LOADED] {assemblyName}: {showcaseContentTypes.Count}개의 ShowcaseContent 타입 발견.");

            foreach (var type in showcaseContentTypes)
            {
                CreateControlItemsFromInstance(assemblyName, type, allControlItemStrings);
            }
        }

        private static void CreateControlItemsFromInstance(string assemblyName, Type type, List<string> allControlItemStrings)
        {
            try
            {
                if (Activator.CreateInstance(type) is not ShowcaseContent instance)
                {
                    Debug.WriteLine($"[Extractor][ERROR] {assemblyName} - {type.Name}: 인스턴스 생성에 실패했습니다.");
                    return;
                }

                var itemInfos = instance.GetShowcaseItemInfos();

                if (itemInfos == null || itemInfos.Count == 0)
                {
                    Debug.WriteLine($"[Extractor][WARN] {assemblyName} - {type.Name}: GetShowcaseItemInfos()에서 반환된 항목이 없습니다.");
                    return;
                }

                var itemsNames = itemInfos.Select(info => $"\"{info.Name}\"").ToList();
                var itemsString = string.Join(", ", itemsNames);

                var showcaseType = itemInfos.First().Name;
                var name = type.Name.Replace("Content", "");
                var iconGeometryName = GetIconGeometry(name);

                string controlItemString =
                    $"new() {{ ShowcaseType = \"{showcaseType}\", " +
                    $"Name = \"{name}\", " +
                    $"Description = \"{name}\", " +
                    $"IconGeometry = {iconGeometryName}, " +
                    $"Category = \"{instance.Category}\", " +
                    $"Items = new List<string>{{ {itemsString} }} }}";

                allControlItemStrings.Add(controlItemString);

                Debug.WriteLine($"[Extractor][SUCCESS] {assemblyName} - {type.Name}: 1개 항목 추가 (Items: {itemInfos.Count}개)");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Extractor][ERROR] {assemblyName} - {type.Name} 처리 중 오류: {ex.Message}");
            }
        }

        private static string GetIconGeometry(string name)
        {
            var iconMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "Button", "ButtonIconGeometry" },
                { "TextBox", "TextBoxIconGeometry" },
                { "CheckBox", "CheckBoxIconGeometry" },
                { "ComboBox", "ComboBoxIconGeometry" },
                { "Slider", "SliderIconGeometry" },
                { "RadioButton", "RadioButtonIconGeometry" },
                { "ListBox", "ListBoxIconGeometry" },
                { "ToggleButton", "ToggleButtonIconGeometry" },
                { "ProgressBar", "ProgressBarIconGeometry" },
                { "Data", "DataIconGeometry" },
                { "Calendar", "CalendarIconGeometry" },
                { "Border", "DockIconGeometry" },
                { "DockPanel", "DockIconGeometry" },
                { "Grid", "GridIconGeometry" },
                { "StackPanel", "StackIconGeometry" },
                { "WrapPanel", "WrapIconGeometry" },
                { "Canvas", "WrapIconGeometry" },
                { "UniformGrid", "GridIconGeometry" },
                { "GridSplitter", "GridIconGeometry" },
                { "AdaptiveColumnsPanel", "GridIconGeometry" },
                { "TableControl", "GridIconGeometry" },
                { "StaggeredPanel", "StackIconGeometry" },
                { "Binding", "DataIconGeometry" },
                { "DataContext", "DataIconGeometry" },
                { "Mvvm", "DataIconGeometry" }
            };

            return iconMapping.TryGetValue(name, out var geometry) ? geometry : "StackIconGeometry";
        }
    }
}