using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// JSON 직렬화를 위해 System.Text.Json 대신 구형이지만 호환성이 높은 라이브러리를 사용합니다.
// 이 코드를 컴파일하기 위해 다음 DLL을 참조해야 합니다: System.Web.Extensions.dll
using System.Web.Script.Serialization; 

// 파일 스코프 네임스페이스를 사용하여 코드를 깔끔하게 유지합니다.
namespace ShowcaseLoader
{
    // ====================================================================
    // 1. 데이터 구조 정의 (요구사항 5, 7, 8 관련)
    // ====================================================================

    /// <summary>
    /// DLL에서 리플렉션으로 찾아 인스턴스화할 기본 타입.
    /// </summary>
    public abstract class ShowcaseContent
    {
        // 요구사항 6: 패널류는 "Layout", 나머지는 "Control"로 분류하기 위한 추상 속성
        public abstract string Category { get; }

        /// <summary>
        /// 요구사항 8: 인스턴스에서 항목 목록을 가져오는 퍼블릭 메서드 (ShowcaseItemInfo 반환)
        /// </summary>
        public abstract List<ShowcaseItemInfo> GetShowcaseItemInfos();
    }

    /// <summary>
    /// GetShowcaseItemInfos()에서 반환되는 개별 정보 단위입니다.
    /// </summary>
    public class ShowcaseItemInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ShowcaseItemInfo() { } // 직렬화를 위한 기본 생성자
        public ShowcaseItemInfo(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

    /// <summary>
    /// 최종적으로 생성할 목록의 항목 구조입니다. (요구사항 5, 7)
    /// </summary>
    public class ControlItem
    {
        public string ShowcaseType { get; set; } // 요구사항 9: Items의 첫 번째 요소로 설정됨
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconGeometry { get; set; }
        public string Category { get; set; } // 요구사항 6
        public List<string> Items { get; set; } // 요구사항 7, 8
        
        public override string ToString() => 
            $"{Category,-8} | {Name,-25} | Items: {string.Join(", ", Items.Take(3))}{(Items.Count > 3 ? "..." : "")}";
    }

    // ====================================================================
    // 3. 메인 로직
    // ====================================================================

    public class DllLoader
    {
        private static readonly string DllsRootPath = "dlls";
        private static readonly string OutputFilePath = "showcase_controls.json";
        
        // 요구사항 5: 최종적으로 생성될 목록
        private static readonly List<ControlItem> _allControlItems = new List<ControlItem>();

        public static void Main(string[] args)
        {
            // 한글 출력을 위해 인코딩을 UTF-8로 설정 (CMD에서 chcp 65001을 먼저 실행하는 것이 가장 확실합니다)
            Console.OutputEncoding = Encoding.UTF8;

            try
            {
                Console.WriteLine("=================================================");
                Console.WriteLine($"DLL 로더 시작: '{DllsRootPath}' 폴더 검색 중...");

                // 1. dlls 폴더 존재 여부 확인
                if (!Directory.Exists(DllsRootPath))
                {
                    Console.WriteLine($"오류: '{DllsRootPath}' 폴더가 존재하지 않습니다. DLL 파일을 이 폴더에 넣어주세요.");
                    return;
                }

                // .dll 확장자를 가진 모든 파일을 찾습니다. (요구사항 1, 2)
                var allDllFiles = Directory.GetFiles(DllsRootPath, "*.dll");

                // 2. 예외 처리: OpenSilverShowcase.Support.dll 제외 (요구사항 3)
                var targetDlls = allDllFiles
                    .Where(path => !Path.GetFileName(path).Equals("OpenSilverShowcase.Support.dll", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                Console.WriteLine($"총 {targetDlls.Count}개의 대상 DLL 파일 발견.");
                Console.WriteLine("-------------------------------------------------");

                // 3. 각 DLL을 로드하고 클래스 인스턴스 생성 (요구사항 4)
                foreach (var dllPath in targetDlls)
                {
                    ProcessAssembly(dllPath);
                }

                // 4. 결과 출력
                Console.WriteLine("\n=================================================");
                Console.WriteLine($"최종 Control Items 목록 (총 {_allControlItems.Count}개):");
                Console.WriteLine("=================================================");
                
                // 정렬하여 보기 좋게 출력 (Layout | Name | Items 순)
                foreach (var item in _allControlItems.OrderBy(i => i.Category).ThenBy(i => i.Name))
                {
                    Console.WriteLine(item);
                }
                Console.WriteLine("=================================================");
                
                // 5. 결과를 JSON 파일로 저장 (새로운 요구사항)
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
            
            // 프로그램이 종료되지 않도록 사용자 입력을 기다리는 코드 추가
            Console.WriteLine($"\n\n--- 결과가 '{OutputFilePath}' 파일로 저장되었습니다. 아무 키나 눌러 창을 닫아주세요. ---");
            Console.ReadKey();
        }

        /// <summary>
        /// 특정 DLL 파일을 로드하고 내부의 ShowcaseContent 타입을 처리합니다.
        /// </summary>
        private static void ProcessAssembly(string dllPath)
        {
            string assemblyName = Path.GetFileName(dllPath);
            Assembly assembly;

            try
            {
                // DLL 로드
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

            // 어셈블리 내의 모든 타입을 검색합니다.
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // 종속성 DLL이 없을 때 발생하는 오류 처리
                Console.WriteLine($"[WARNING] {assemblyName}: 일부 타입을 로드하지 못했습니다. 종속성 DLL이 누락되었을 수 있습니다.");
                foreach (var loaderEx in ex.LoaderExceptions)
                {
                    Console.WriteLine($"  - 누락: {loaderEx.Message}");
                }
                types = ex.Types.Where(t => t != null).ToArray(); // 로드된 타입만 사용
            }


            var showcaseContentTypes = types
                .Where(t => 
                    t.IsClass && 
                    !t.IsAbstract && 
                    // ShowcaseContent를 상속받는 타입 필터링
                    typeof(ShowcaseContent).IsAssignableFrom(t)
                ).ToList();

            if (showcaseContentTypes.Count == 0)
            {
                Console.WriteLine($"[INFO] {assemblyName}: ShowcaseContent 타입 클래스가 없습니다.");
                return;
            }
            
            Console.WriteLine($"[LOADED] {assemblyName}: {showcaseContentTypes.Count}개의 ShowcaseContent 타입 발견.");

            // 4. ShowcaseContent 타입 인스턴스 생성 및 목록 채우기 (요구사항 4, 8)
            foreach (var type in showcaseContentTypes)
            {
                CreateControlItemsFromInstance(assemblyName, type);
            }
        }

        /// <summary>
        /// 특정 ShowcaseContent 타입의 인스턴스를 생성하고 정보를 추출하여 _allControlItems에 추가합니다.
        /// </summary>
        private static void CreateControlItemsFromInstance(string assemblyName, Type type)
        {
            try
            {
                // 인스턴스 생성 (요구사항 4)
                if (Activator.CreateInstance(type) is not ShowcaseContent instance)
                {
                    Console.WriteLine($"[ERROR] {assemblyName} - {type.Name}: 인스턴스 생성에 실패했습니다.");
                    return;
                }

                // GetShowcaseItemInfos 메서드 호출 (요구사항 8)
                var itemInfos = instance.GetShowcaseItemInfos();
                
                if (itemInfos == null || itemInfos.Count == 0)
                {
                    Console.WriteLine($"[WARN] {assemblyName} - {type.Name}: GetShowcaseItemInfos()에서 반환된 항목이 없습니다.");
                    return;
                }
                
                // 요구사항 10: 무조건 1개 이상 있다고 가정 (예외 처리 생략)
                // Items는 생성된 모든 ShowcaseItemInfo의 Name 목록입니다. (요구사항 7)
                var itemsNames = itemInfos.Select(info => info.Name).ToList();
                
                // 요구사항 9: ShowcaseType은 Items의 가장 첫 번째 항목으로 설정
                var showcaseType = itemsNames.First();

                // 리스트를 순회하며 최종 ControlItem 목록을 생성합니다.
                foreach (var info in itemInfos)
                {
                    var newItem = new ControlItem
                    {
                        ShowcaseType = showcaseType,
                        Name = info.Name,
                        Description = info.Description,
                        IconGeometry = $"{info.Name}IconGeometry", // 플레이스홀더 값
                        Category = instance.Category, // 요구사항 6
                        Items = itemsNames // 현재 인스턴스에서 찾은 모든 Item Names 목록
                    };
                    
                    _allControlItems.Add(newItem);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {assemblyName} - {type.Name} 처리 중 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 생성된 ControlItem 목록을 JSON 파일로 저장합니다.
        /// </summary>
        private static void SaveResultsToJson(List<ControlItem> items)
        {
            try
            {
                // JavaScriptSerializer를 사용하여 JSON 직렬화
                var serializer = new JavaScriptSerializer();
                string jsonString = serializer.Serialize(items);
                
                // File.WriteAllText는 UTF-8을 기본 지원합니다.
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
