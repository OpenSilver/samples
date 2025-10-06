using System;
using System.Collections.Generic;
using OpenSilverShowcase.Support.Local.Helpers;
using System.Linq;
using System.Threading.Tasks;
using OpenSilverShowcase.Support.Local.Models;
using OpenSilverShowcase.Support.UI.Primitives;

namespace OpenSilverShowcase.Support.UI.Units
{
    public class ShowcaseContent : ShowcaseContentElement
    {
        public event Action Share;
        public event Action<string, string> FullScreenCode;
        public event Action<string> ShowcaseChanged;

        private EmbeddedResourceLoader _resourceLoader;
        private ShowcaseListBox _showcaseListBox;
        private Dictionary<Type, object> _instanceCache = new Dictionary<Type, object>();
        private string _currentShowcaseName;
        private bool _isFirst = true;


        public ShowcaseContent()
        {
            DefaultStyleKey = typeof(ShowcaseContent);
            _resourceLoader = new EmbeddedResourceLoader(this.GetType().Assembly);
            var source = _resourceLoader.GetShowcaseCodeResources();
            var types = GetShowcaseItemTypes();
            Console.WriteLine($"ShowcaseItem2 types: {types.Length}");

            Loaded += ShowcaseContent_Loaded;
        }

        private void ShowcaseContent_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_isFirst)
            {
                _isFirst = false;
            }
            else
            {
                ShowcaseChanged?.Invoke(_currentShowcaseName);
            }
        }

        public string GetCurrentShowcaseName()
        {
            return _currentShowcaseName;
        }

        public override async void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _showcaseListBox = GetTemplateChild("PART_ShowcaseItems") as ShowcaseListBox;

            if (_showcaseListBox != null)
            {
                var source = GetShowcaseItemInfos();
                _showcaseListBox.ItemsSource = source.OrderBy(x => x.Name);
                _showcaseListBox.ItemClicked += ShowcaseItemClicked;

                await Task.Delay(100);
                if (!string.IsNullOrWhiteSpace(FirstSelectedItemName))
                {
                    await _showcaseListBox.SelectItemByName(FirstSelectedItemName, true);
                }
                else if (AutoSelectFirstItem)
                {
                    await _showcaseListBox.SelectItemByDataContext(source.FirstOrDefault(), true);
                }
            }
        }

        public void SelectItemByName(string name)
        {
            FirstSelectedItemName = name;
            _showcaseListBox?.SelectItemByName(name, true);
        }

        private void ShowcaseItemClicked(object sender, ItemClickedEventArgs e)
        {
            if (e.Item is ShowcaseItemInfo itemInfo)
            {
                _currentShowcaseName = itemInfo.Name;
                ShowcaseChanged?.Invoke(itemInfo.Name);
                try
                {
                    if (_instanceCache.TryGetValue(itemInfo.Type, out var cachedInstance))
                    {
                        this.Content = cachedInstance;
                        Console.WriteLine($"Using cached instance: {itemInfo.Name}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to get cached instance: {itemInfo.Name} - {ex.Message}");
                }
            }
        }

        public List<ShowcaseItemInfo> GetShowcaseItemInfos()
        {
            var result = new List<ShowcaseItemInfo>();

            foreach (var type in GetShowcaseItemTypes())
            {
                try
                {
                    var instance = Activator.CreateInstance(type) as ShowcaseItem;
                    var title = instance?.Title ?? type.Name;

                    _instanceCache[type] = instance;

                    result.Add(new ShowcaseItemInfo
                    {
                        Name = title,
                        Type = type
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to create instance: {type.Name} - {ex.Message}");
                }
            }

            return result;
        }

        public Type[] GetShowcaseItemTypes()
        {
            return this.GetType().Assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(ShowcaseItem)))
                .ToArray();
        }public string GetFirstItemName()
{
    var items = GetShowcaseItemInfos();
    return items.OrderBy(x => x.Name).FirstOrDefault()?.Name;
}

        public void ClearInstanceCache()
        {
            _instanceCache.Clear();
            Console.WriteLine("Instance cache cleared");
        }

        public void RaiseFullScreenCode(string code, string language)
        {
            FullScreenCode?.Invoke(code, language);
        }

        internal void RaiseShare(CodeSource item)
        {
            Share?.Invoke();
        }
    }
}
