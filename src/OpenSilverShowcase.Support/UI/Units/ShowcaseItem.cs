using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Jamesnet.Foundation;
using OpenSilverShowcase.Support.Local.Helpers;
using OpenSilverShowcase.Support.UI.Primitives;

namespace OpenSilverShowcase.Support.UI.Units
{
    public class ShowcaseItem : ShowcaseItemElement
    {
        public static MonacoEditor MonacoEditor;

        public static readonly DependencyProperty DemoBackgroundProperty =
            DependencyProperty.Register(nameof(DemoBackground), typeof(System.Windows.Media.Brush), typeof(ShowcaseItem), new PropertyMetadata(null));

        public static readonly DependencyProperty DemoBorderBrushProperty =
            DependencyProperty.Register(nameof(DemoBorderBrush), typeof(System.Windows.Media.Brush), typeof(ShowcaseItem), new PropertyMetadata(null));

        public static readonly DependencyProperty TitleForegroundProperty =
            DependencyProperty.Register(nameof(TitleForeground), typeof(System.Windows.Media.Brush), typeof(ShowcaseItem), new PropertyMetadata(null));

        public static readonly DependencyProperty DescriptionForegroundProperty =
            DependencyProperty.Register(nameof(DescriptionForeground), typeof(System.Windows.Media.Brush), typeof(ShowcaseItem), new PropertyMetadata(null));

        public static readonly DependencyProperty CodeToolbarBorderBrushProperty =
            DependencyProperty.Register(nameof(CodeToolbarBorderBrush), typeof(System.Windows.Media.Brush), typeof(ShowcaseItem), new PropertyMetadata(null));

        public static readonly DependencyProperty CodePanelBorderBrushProperty =
            DependencyProperty.Register(nameof(CodePanelBorderBrush), typeof(System.Windows.Media.Brush), typeof(ShowcaseItem), new PropertyMetadata(null));

        public static readonly DependencyProperty MobileCodeBackgroundProperty =
            DependencyProperty.Register(nameof(MobileCodeBackground), typeof(System.Windows.Media.Brush), typeof(ShowcaseItem), new PropertyMetadata(null));

        public static readonly DependencyProperty DesktopCodeBackgroundProperty =
            DependencyProperty.Register(nameof(DesktopCodeBackground), typeof(System.Windows.Media.Brush), typeof(ShowcaseItem), new PropertyMetadata(null));

        public static readonly DependencyProperty CodeFooterBackgroundProperty =
            DependencyProperty.Register(nameof(CodeFooterBackground), typeof(System.Windows.Media.Brush), typeof(ShowcaseItem), new PropertyMetadata(null));

        public System.Windows.Media.Brush DemoBackground
        {
            get => (System.Windows.Media.Brush)GetValue(DemoBackgroundProperty);
            set => SetValue(DemoBackgroundProperty, value);
        }

        public System.Windows.Media.Brush DemoBorderBrush
        {
            get => (System.Windows.Media.Brush)GetValue(DemoBorderBrushProperty);
            set => SetValue(DemoBorderBrushProperty, value);
        }

        public System.Windows.Media.Brush TitleForeground
        {
            get => (System.Windows.Media.Brush)GetValue(TitleForegroundProperty);
            set => SetValue(TitleForegroundProperty, value);
        }

        public System.Windows.Media.Brush DescriptionForeground
        {
            get => (System.Windows.Media.Brush)GetValue(DescriptionForegroundProperty);
            set => SetValue(DescriptionForegroundProperty, value);
        }

        public System.Windows.Media.Brush CodeToolbarBorderBrush
        {
            get => (System.Windows.Media.Brush)GetValue(CodeToolbarBorderBrushProperty);
            set => SetValue(CodeToolbarBorderBrushProperty, value);
        }

        public System.Windows.Media.Brush CodePanelBorderBrush
        {
            get => (System.Windows.Media.Brush)GetValue(CodePanelBorderBrushProperty);
            set => SetValue(CodePanelBorderBrushProperty, value);
        }

        public System.Windows.Media.Brush MobileCodeBackground
        {
            get => (System.Windows.Media.Brush)GetValue(MobileCodeBackgroundProperty);
            set => SetValue(MobileCodeBackgroundProperty, value);
        }

        public System.Windows.Media.Brush DesktopCodeBackground
        {
            get => (System.Windows.Media.Brush)GetValue(DesktopCodeBackgroundProperty);
            set => SetValue(DesktopCodeBackgroundProperty, value);
        }

        public System.Windows.Media.Brush CodeFooterBackground
        {
            get => (System.Windows.Media.Brush)GetValue(CodeFooterBackgroundProperty);
            set => SetValue(CodeFooterBackgroundProperty, value);
        }

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
            Loaded += ShowcaseItem_Loaded;
        }

        private void ShowcaseItem_Loaded(object sender, RoutedEventArgs e)
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
            _copyButton = GetTemplateChild("PART_CopyButton") as CopyIconButton;

            if (_codeItems != null)
            {
                _codeItems.ItemsSource = CodeSources;
                _codeItems.SelectionChanged += OnCodeItemSelected;
                _codeItems.SelectedItem = CodeSources?.FirstOrDefault();
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
                    if (_simpleCode != null)
                        _simpleCode.Text = code ?? "// Unable to load code.";
                }
                catch (Exception ex)
                {
                    MonacoEditor.Code = $"// Failed to load code: {ex.Message}";
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
                string ns = GetType().Namespace;
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
