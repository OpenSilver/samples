using CSHTML5.Native.Html.Controls;
using OpenSilver;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace OpenSilverShowcase.ToastUIEditor.Examples
{
    public class ToastUiEditor : HtmlPresenter
    {
        private const string CdnUrl = "https://uicdn.toast.com/editor/latest";
        private object _domElement;
        private bool _isInitialized = false;
        private int _loadCheckAttempts = 0;
        private const int MaxLoadCheckAttempts = 50;

        #region Content
        private string _contentInEditor = string.Empty;

        public string Content
        {
            get { return (string)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(string), typeof(ToastUiEditor), new PropertyMetadata(string.Empty, OnContentChanged));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var editor = (ToastUiEditor)d;
            var newContent = (string)e.NewValue;

            if (editor._domElement != null && newContent != editor._contentInEditor)
            {
                editor._contentInEditor = newContent;
                Interop.ExecuteJavaScriptVoidAsync(
                    "if($0.editor) { $0.editor.setMarkdown($1); }",
                    editor._domElement,
                    newContent
                );
            }
        }
        #endregion

        public ToastUiEditor()
        {
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            LoadResources();
        }

        private void LoadResources()
        {
            LoadResourcesAsync();
        }

        private void LoadResourcesAsync()
        {
            Task.Run(async () =>
            {
                try
                {
                    // CSS 파일 먼저 로드
                    if (!await FileLoader.TryLoadCssFile(CdnUrl + "/toastui-editor.min.css"))
                    {
                        Console.WriteLine("Failed to load Toast UI Editor CSS");
                        return;
                    }

                    // JavaScript 파일 로드
                    if (!await FileLoader.TryLoadJavaScriptFile(CdnUrl + "/toastui-editor-all.min.js"))
                    {
                        Console.WriteLine("Failed to load Toast UI Editor JS");
                        return;
                    }

                    // UI 스레드에서 실행
                    Dispatcher.BeginInvoke(new Action(() =>
                    {
                        _domElement = Interop.GetDiv(this);
                        CheckToastUiLoaded();
                    }));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading resources: " + ex.Message);
                }
            });
        }

        private void CheckToastUiLoaded()
        {
            _loadCheckAttempts++;

            if (_loadCheckAttempts > MaxLoadCheckAttempts)
            {
                Console.WriteLine("Toast UI Editor library failed to load within timeout period");
                return;
            }

            // toastui 객체가 로드되었는지 확인
            Interop.ExecuteJavaScriptAsync(
                "return typeof toastui !== 'undefined' && typeof toastui.Editor !== 'undefined';",
                OnToastUiCheckResult
            );
        }

        private void OnToastUiCheckResult(object result)
        {
            bool isLoaded = result != null && (bool)result;

            if (isLoaded)
            {
                InitializeEditor();
            }
            else
            {
                // 100ms 후 다시 확인
                var timer = new System.Windows.Threading.DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(100);
                timer.Tick += (sender, e) =>
                {
                    timer.Stop();
                    CheckToastUiLoaded();
                };
                timer.Start();
            }
        }

        private void InitializeEditor()
        {
            if (_isInitialized) return;

            try
            {
                Interop.ExecuteJavaScriptVoidAsync(@"
                    try {
                        if (typeof toastui === 'undefined') {
                            throw new Error('toastui is not defined');
                        }
                        
                        $0.editor = new toastui.Editor({
                            el: $0.firstChild,
                            initialValue: $1,
                            previewStyle: 'vertical',
                            initialEditType: 'wysiwyg',
                            theme: 'dark'
                        });
                        
                        $0.editor.on('change', function() {
                            try {
                                var value = $0.editor.getMarkdown();
                                $2(value);
                            } catch (error) {
                                console.error('Error in change handler:', error);
                            }
                        });
                        
                        console.log('Toast UI Editor initialized successfully');
                        $3(true);
                    } catch (error) {
                        console.error('Error initializing Toast UI Editor:', error);
                        $3(false);
                    }
                ",
                _domElement,
                Content ?? string.Empty,
                (Action<string>)OnContentChangedInEditor,
                (Action<bool>)OnEditorInitialized);
            }
            catch (Exception ex)
            {
                Console.WriteLine("JavaScript error during editor initialization: " + ex.Message);
            }
        }

        private void OnEditorInitialized(bool success)
        {
            _isInitialized = success;
            if (!success)
            {
                Console.WriteLine("Failed to initialize Toast UI Editor");
            }
        }

        private void OnContentChangedInEditor(string newContent)
        {
            try
            {
                _contentInEditor = newContent;
                Content = newContent;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in OnContentChangedInEditor: " + ex.Message);
            }
        }

        private void OnResourceLoadFailed()
        {
            Console.WriteLine("Failed to load Toast UI Editor resources");
        }
    }
}