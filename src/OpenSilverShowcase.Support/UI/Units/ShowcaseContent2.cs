using System;
using System.Collections.Generic;
using System.Windows.Controls;
using OpenSilverShowcase.Support.Local.Managers;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Threading.Tasks;
using Jamesnet.Foundation;
using System.Windows.Browser;
using System.Windows;

namespace OpenSilverShowcase.Support.UI.Units
{
    public class ShowcaseItemInfo
    {
        public string Name { get; set; }
        public Type Type { get; set; }

        public override string ToString() => Name;
    }

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
                _showcaseListBox.ItemsSource = source.OrderBy(x=>x.Name);
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

    public class ShowcaseItem : ShowcaseItemElement
    {
        public static MonacoEditor MonacoEditor;

        private EmbeddedResourceLoader _resourceLoader;
        private Selector _codeItems;
        private ContentControl _codeContent;
        private SimpleCodeTextBox _simpleCode;
        private FullScreenButton _fullScreenButton;
        private IconButton _githubButton;
        private IconButton _shareButton;
        private CopyIconButton _copyButton;

        public ShowcaseItem()
        {
            DefaultStyleKey = typeof(ShowcaseItem);
            _resourceLoader = new EmbeddedResourceLoader(this.GetType().Assembly);
            Loaded += ShowcaseItem2_Loaded;
        }

        private void ShowcaseItem2_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ShowCode();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _codeItems = GetTemplateChild("PART_CodeItems") as Selector;
            _codeContent = GetTemplateChild("PART_CodeContent") as ContentControl;
            _simpleCode = GetTemplateChild("PART_SimpleCode") as SimpleCodeTextBox;
            _fullScreenButton = GetTemplateChild("PART_FullScreen") as FullScreenButton;
            _githubButton = GetTemplateChild("PART_GitHubButton") as IconButton;
            _shareButton = GetTemplateChild("PART_ShareButton") as IconButton;
            _shareButton = GetTemplateChild("PART_ShareButton") as IconButton;
            _copyButton = GetTemplateChild("PART_CopyButton") as CopyIconButton;

            if (_codeItems != null)
            {
                _codeItems.ItemsSource = this.CodeSources;
                _codeItems.SelectionChanged += OnCodeItemSelected;
                _codeItems.SelectedItem = this.CodeSources?.FirstOrDefault();
            }
            if (_fullScreenButton != null)
            {
                _fullScreenButton.Command = new RelayCommand(OnFullScreen);
            }
            if (_githubButton != null)
            {
                _githubButton.Click += OnGitHubClick;
            }
            if (_shareButton != null)
            {
                _shareButton.Click += OnShareClick;
            }
            if (_shareButton != null)
            {
                _shareButton.Click += OnShareClick;
            }
            if (_copyButton != null)
            {
                _copyButton.Click += OnCopyClick;
            }
        }

        private void OnFullScreen()
        {
            var parent = this.FindParent<ShowcaseContent>();
            if (parent != null && _codeItems?.SelectedItem is CodeSource selectedCodeSource)
            {
                string code = _simpleCode?.Text;
                string language = selectedCodeSource.Language;
                parent.RaiseFullScreenCode(code, language);
            }
        }

        private void OnCodeItemSelected(object sender, SelectionChangedEventArgs e)
        {
            ShowCode();
        }

        private async void ShowCode()
        {
            await Task.Delay(50);
            if (MonacoEditor == null)
            {
                MonacoEditor = new MonacoEditor();
            }

            if (_codeItems?.SelectedItem is CodeSource selectedCodeSource && _codeContent != null)
            {
                try
                {
                    string code = _resourceLoader.LoadTextByPath(selectedCodeSource.Source);
                    code = XamlDemoExtractor.ExtractDemoContentOnly(code);
                    MonacoEditor.IsReadOnly = true;
                    MonacoEditor.Code = code ?? "// Unable to load code.";
                    MonacoEditor.Language = selectedCodeSource.Language;
                    MonacoEditor.Theme = "vs-dark";
                    Console.WriteLine($"Code loaded: {selectedCodeSource.Source}");

                    if (_simpleCode != null)
                        _simpleCode.Text = code ?? "// Unable to load code.";
                }
                catch (Exception ex)
                {
                    MonacoEditor.Code = $"// Failed to load code: {ex.Message}";
                    Console.WriteLine($"Failed to load code: {selectedCodeSource.Source} - {ex.Message}");
                }

                var parent = MonacoEditor.Parent;
                if (parent is ContentControl parentContent)
                {
                    parentContent.Content = null;
                }
                _codeContent.Content = MonacoEditor;
            }
        }

        public void OnGitHubClick(object sender, RoutedEventArgs e)
        {
            var item = _codeItems?.SelectedItem as CodeSource;

            if (item != null && item.Source != null)
            {
                string ns = this.GetType().Namespace;
                string[] parts = ns?.Split('.') ?? Array.Empty<string>();
                string projectName = (parts.Length >= 2) ? $"{parts[0]}.{parts[1]}" : "Unknown";

                string path = item.Source.OriginalString.TrimStart('/');

                string prefix = $"{projectName};component/";
                if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    path = path.Substring(prefix.Length);

                if (!path.StartsWith("Examples/", StringComparison.OrdinalIgnoreCase))
                {
                    string fileName = System.IO.Path.GetFileName(path);
                    path = $"Examples/{fileName}";
                }

                string githubUrl = $"https://github.com/OpenSilver/samples/blob/master/src/OpenSilver.Samples/{projectName}/{path}";

                HtmlPage.Window.Navigate(new Uri(githubUrl, UriKind.Absolute), "_blank");
            }
        }

        public void OnShareClick(object sender, RoutedEventArgs e)
        {
            var item = _codeItems?.SelectedItem as CodeSource;
            var parent = this.FindParent<ShowcaseContent>();

            if (parent != null)
            {
                parent.RaiseShare(item);
            }
        }

        [Obsolete]
        public void OnCopyClick(object sender, RoutedEventArgs e)
        {
            var item = _codeItems?.SelectedItem as CodeSource;
            if (item != null && !string.IsNullOrEmpty(item.Code))
            {
                Clipboard.SetText(item.Code);
            }
        }
    }
}
