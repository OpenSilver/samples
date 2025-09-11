using System;
using System.Windows.Controls;
using OpenSilverShowcase.Support.Local.Helpers;
using System.Linq;
using System.Windows.Controls.Primitives;
using System.Threading.Tasks;
using Jamesnet.Foundation;
using System.Windows.Browser;
using System.Windows;
using OpenSilverShowcase.Support.UI.Primitives;

namespace OpenSilverShowcase.Support.UI.Units
{
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
                    MonacoEditor.Theme = "vs-dark";
                    MonacoEditor.IsReadOnly = true;
                    MonacoEditor.Code = code ?? "// Unable to load code.";
                    MonacoEditor.Language = selectedCodeSource.Language;
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
