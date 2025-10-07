using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenSilverShowcase.Support.UI.Units;

namespace DemoLoader
{
    public class ControlItem
    {
        public string ShowcaseType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconGeometry { get; set; }
        public string Category { get; set; }
        public List<string> Items { get; set; }

        public override string ToString() =>
            $"{Category,-8} | {Name,-25} | Items: {string.Join(", ", Items.Take(3))}{(Items.Count > 3 ? "..." : "")}";
    }

    public class DllLoader
    {
        private static readonly string DllsRootPath = "dlls";
        private static readonly string OutputFilePath = "showcase_controls.json";

        private static readonly List<ControlItem> _allControlItems = new List<ControlItem>();

        public static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            try
            {
                Console.WriteLine("=================================================");
                Console.WriteLine($"DLL 로더 시작: '{DllsRootPath}' 폴더 검색 중...");

                if (!Directory.Exists(DllsRootPath))
                {
                    Console.WriteLine($"오류: '{DllsRootPath}' 폴더가 존재하지 않습니다. DLL 파일을 이 폴더에 넣어주세요.");
                    return;
                }

                var allDllFiles = Directory.GetFiles(DllsRootPath, "*.dll");

                var targetDlls = allDllFiles
                    .Where(path => !Path.GetFileName(path).Equals("OpenSilverShowcase.Support.dll", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                Console.WriteLine($"총 {targetDlls.Count}개의 대상 DLL 파일 발견.");
                Console.WriteLine("-------------------------------------------------");

                foreach (var dllPath in targetDlls)
                {
                    ProcessAssembly(dllPath);
                }

                Console.WriteLine("\n=================================================");
                Console.WriteLine($"최종 Control Items 목록 (총 {_allControlItems.Count}개):");
                Console.WriteLine("=================================================");

                foreach (var item in _allControlItems.OrderBy(i => i.Category).ThenBy(i => i.Name))
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("=================================================");

                SaveResultsToJson(_allControlItems);
            }
            catch (ReflectionTypeLoadException rtle)
            {
                Console.WriteLine($"\n[치명적 오류: DLL 종속성 누락] 일부 DLL을 로드하지 못했습니다. 누락된 참조 목록:");
                foreach (var ex in rtle.LoaderExceptions)
                {
                    Console.WriteLine($"  - {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"치명적인 오류 발생: {ex.Message}");
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine($"\n\n--- 결과가 '{OutputFilePath}' 파일로 저장되었습니다. 아무 키나 눌러 창을 닫아주세요. ---");
            Console.ReadKey();
        }

        private static void ProcessAssembly(string dllPath)
        {
            string assemblyName = Path.GetFileName(dllPath);
            Assembly assembly;

            try
            {
                assembly = Assembly.LoadFrom(dllPath);
            }
            catch (BadImageFormatException)
            {
                Console.WriteLine($"[SKIP] {assemblyName}: 유효한 .NET 어셈블리가 아닙니다.");
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {assemblyName} 로드 중 오류: {ex.Message}");
                return;
            }

            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                Console.WriteLine($"[WARNING] {assemblyName}: 일부 타입을 로드하지 못했습니다. 종속성 DLL이 누락되었을 수 있습니다.");
                foreach (var loaderEx in ex.LoaderExceptions)
                {
                    Console.WriteLine($"  - 누락: {loaderEx.Message}");
                }
                types = ex.Types.Where(t => t != null).ToArray();
            }


            var showcaseContentTypes = types
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    typeof(ShowcaseContent).IsAssignableFrom(t)
                ).ToList();

            if (showcaseContentTypes.Count == 0)
            {
                Console.WriteLine($"[INFO] {assemblyName}: ShowcaseContent 타입 클래스가 없습니다.");
                return;
            }

            Console.WriteLine($"[LOADED] {assemblyName}: {showcaseContentTypes.Count}개의 ShowcaseContent 타입 발견.");

            foreach (var type in showcaseContentTypes)
            {
                CreateControlItemsFromInstance(assemblyName, type);
            }
        }

        private static void CreateControlItemsFromInstance(string assemblyName, Type type)
        {
            try
            {
                if (Activator.CreateInstance(type) is not ShowcaseContent instance)
                {
                    Console.WriteLine($"[ERROR] {assemblyName} - {type.Name}: 인스턴스 생성에 실패했습니다.");
                    return;
                }

                var itemInfos = instance.GetShowcaseItemInfos();

                if (itemInfos == null || itemInfos.Count == 0)
                {
                    Console.WriteLine($"[WARN] {assemblyName} - {type.Name}: GetShowcaseItemInfos()에서 반환된 항목이 없습니다.");
                    return;
                }

                var itemsNames = itemInfos.Select(info => info.Name).ToList();

                var showcaseType = itemsNames.First();

                foreach (var info in itemInfos)
                {
                    var newItem = new ControlItem
                    {
                        ShowcaseType = showcaseType,
                        Name = info.Name,
                        Description = info.Description,
                        IconGeometry = $"{info.Name}IconGeometry",
                        Category = instance.Category,
                        Items = itemsNames
                    };

                    _allControlItems.Add(newItem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {assemblyName} - {type.Name} 처리 중 오류: {ex.Message}");
            }
        }

        private static void SaveResultsToJson(List<ControlItem> items)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                string jsonString = serializer.Serialize(items);

                File.WriteAllText(OutputFilePath, jsonString, Encoding.UTF8);
                Console.WriteLine($"\n[성공] 최종 목록이 '{OutputFilePath}'에 저장되었습니다.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[JSON 저장 오류] 파일을 저장하는 동안 오류가 발생했습니다: {ex.Message}");
            }
        }
    }
}
