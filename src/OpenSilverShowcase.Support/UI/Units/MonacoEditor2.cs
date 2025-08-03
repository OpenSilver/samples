using System;
using System.Windows;
using CSHTML5.Native.Html.Controls;
using OpenSilver;
using System.Windows.Browser;

namespace OpenSilverShowcase.Support.UI.Units
{
    public static class MonacoScriptHelper
    {
        public static void LoadAndCreateEditor(object div, string code, string language, string theme, bool isReadOnly, double width, double height, string editorId)
        {
            Interop.ExecuteJavaScriptVoid($@"
                const div = $0;
                div.style.width = '{width}px';
                div.style.height = '{height}px';
                div.style.display = 'block';
                div.style.position = 'relative';
                div.style.backgroundColor = '#1e1e1e';
                div.style.border = '0px solid #007acc';
                div.style.boxSizing = 'border-box';
                div.innerHTML = '<div style=""padding:20px;color:#cccccc;font-family:monospace;height:100%;display:flex;align-items:center;justify-content:center;"">Loading Monaco Editor...</div>';

                if (window.monaco) {{
                    createEditor();
                }} else {{
                    loadMonaco();
                }}

                function loadMonaco() {{
                    if (window.monacoLoading) return;
                    window.monacoLoading = true;
                    const script = document.createElement('script');
                    script.src = 'https://unpkg.com/monaco-editor@latest/min/vs/loader.js';
                    script.onload = function() {{
                        require.config({{ paths: {{ vs: 'https://unpkg.com/monaco-editor@latest/min/vs' }} }});
                        require(['vs/editor/editor.main'], createEditor);
                    }};
                    document.head.appendChild(script);
                }}

                function createEditor() {{
                    div.innerHTML = '';
                    const editor = monaco.editor.create(div, {{
                        value: {System.Text.Json.JsonSerializer.Serialize(code ?? "")},
                        language: {System.Text.Json.JsonSerializer.Serialize(language ?? "javascript")},
                        theme: {System.Text.Json.JsonSerializer.Serialize(theme ?? "vs-dark")},
                        readOnly: {isReadOnly.ToString().ToLower()},
                        padding: {{ top: 16, bottom: 16 }},
                        automaticLayout: true,
                        minimap: {{ enabled: false }},
                        scrollBeyondLastLine: false,
                        lineNumbers: 'on'
                    }});
                    
                    div._monacoEditor = editor;
                    div._editorId = '{editorId}';
                    
                    // 코드 변경 이벤트 리스너 추가 (ReadOnly가 false일 때만)
                    if (!{isReadOnly.ToString().ToLower()}) {{
                        editor.onDidChangeModelContent(function(e) {{
                            try {{
                                const newCode = editor.getValue();
                                // C# 콜백 호출
                                if (window.MonacoCallbacks && window.MonacoCallbacks['{editorId}']) {{
                                    window.MonacoCallbacks['{editorId}'].OnCodeChanged(newCode);
                                    console.log('[Monaco] Code changed event fired for editor: {editorId}');
                                }}
                            }} catch (error) {{
                                console.error('[Monaco] Error in code change callback:', error);
                            }}
                        }});
                        console.log('[Monaco] Code change listener added for editor: {editorId}');
                    }}
                    
                    const editorWidth = {width - 2};
                    const editorHeight = {height - 2};
                    editor.layout({{ width: editorWidth, height: editorHeight }});
                    console.log('[Monaco] Editor created with readOnly:', {isReadOnly.ToString().ToLower()});
                }}
            ", div);
        }

        public static void UpdateEditorCode(object div, string newCode)
        {
            Interop.ExecuteJavaScriptVoid($@"
                const div = $0;
                if (div._monacoEditor) {{
                    // 무한 루프 방지: 현재 값과 다를 때만 설정
                    const currentCode = div._monacoEditor.getValue();
                    if (currentCode !== {System.Text.Json.JsonSerializer.Serialize(newCode)}) {{
                        div._monacoEditor.setValue({System.Text.Json.JsonSerializer.Serialize(newCode)});
                        console.log('[Monaco] Code updated programmatically');
                    }}
                }}
            ", div);
        }

        public static void UpdateEditorReadOnly(object div, bool isReadOnly, string editorId)
        {
            Interop.ExecuteJavaScriptVoid($@"
                const div = $0;
                if (div._monacoEditor) {{
                    div._monacoEditor.updateOptions({{ readOnly: {isReadOnly.ToString().ToLower()} }});
                    
                    // ReadOnly 변경에 따라 이벤트 리스너 추가/제거
                    if (!{isReadOnly.ToString().ToLower()} && !div._hasChangeListener) {{
                        // ReadOnly가 false가 되면 이벤트 리스너 추가
                        div._monacoEditor.onDidChangeModelContent(function(e) {{
                            try {{
                                const newCode = div._monacoEditor.getValue();
                                if (window.MonacoCallbacks && window.MonacoCallbacks['{editorId}']) {{
                                    window.MonacoCallbacks['{editorId}'].OnCodeChanged(newCode);
                                }}
                            }} catch (error) {{
                                console.error('[Monaco] Error in code change callback:', error);
                            }}
                        }});
                        div._hasChangeListener = true;
                        console.log('[Monaco] Code change listener added');
                    }}
                    
                    console.log('[Monaco] ReadOnly updated to:', {isReadOnly.ToString().ToLower()});
                }}
            ", div);
        }

        public static void UpdateEditorLanguage(object div, string language)
        {
            Interop.ExecuteJavaScriptVoid($@"
                const div = $0;
                if (div._monacoEditor && window.monaco) {{
                    monaco.editor.setModelLanguage(div._monacoEditor.getModel(), {System.Text.Json.JsonSerializer.Serialize(language)});
                    console.log('[Monaco] Language updated to:', {System.Text.Json.JsonSerializer.Serialize(language)});
                }}
            ", div);
        }

        public static void UpdateEditorTheme(object div, string theme)
        {
            Interop.ExecuteJavaScriptVoid($@"
                const div = $0;
                if (window.monaco) {{
                    monaco.editor.setTheme({System.Text.Json.JsonSerializer.Serialize(theme)});
                    console.log('[Monaco] Theme updated to:', {System.Text.Json.JsonSerializer.Serialize(theme)});
                }}
            ", div);
        }

        public static void RegisterCallback(string editorId, object callbackObject)
        {
            Interop.ExecuteJavaScriptVoid($@"
                if (!window.MonacoCallbacks) {{
                    window.MonacoCallbacks = {{}};
                }}
                window.MonacoCallbacks['{editorId}'] = $0;
                console.log('[Monaco] Callback registered for editor:', '{editorId}');
            ", callbackObject);
        }
    }

    public class CodeEditorBase : HtmlPresenter
    {
        public string Code
        {
            get => (string)GetValue(CodeProperty);
            set => SetValue(CodeProperty, value);
        }
        public static readonly DependencyProperty CodeProperty =
            DependencyProperty.Register(nameof(Code), typeof(string), typeof(CodeEditorBase),
            new PropertyMetadata("console.log('Hello World');", OnCodeChanged));

        public string Theme
        {
            get => (string)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register(nameof(Theme), typeof(string), typeof(CodeEditorBase),
            new PropertyMetadata("vs-dark", OnThemeChanged));

        public string Language
        {
            get => (string)GetValue(LanguageProperty);
            set => SetValue(LanguageProperty, value);
        }
        public static readonly DependencyProperty LanguageProperty =
            DependencyProperty.Register(nameof(Language), typeof(string), typeof(CodeEditorBase),
            new PropertyMetadata("javascript", OnLanguageChanged));

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(CodeEditorBase),
            new PropertyMetadata(false, OnIsReadOnlyChanged));

        public event EventHandler<string> CodeChanged;

        protected virtual void OnCodeChanged(string newCode)
        {
            CodeChanged?.Invoke(this, newCode);
        }

        private static void OnCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MonacoEditor editor)
            {
                editor.UpdateCode(e.NewValue as string);
            }
        }
        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MonacoEditor editor)
            {
                editor.UpdateTheme(e.NewValue as string);
            }
        }
        private static void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MonacoEditor editor)
            {
                editor.UpdateLanguage(e.NewValue as string);
            }
        }
        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MonacoEditor editor)
            {
                editor.UpdateReadOnly((bool)e.NewValue);
            }
        }
    }

    public class ScriptedCodeEditor : CodeEditorBase
    {
    }

    public class MonacoEditor : ScriptedCodeEditor
    {
        private object _div;
        private bool _editorCreated = false;
        private string _editorId;
        private bool _isUpdatingFromCallback = false;

        public MonacoEditor()
        {
            this.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.VerticalAlignment = VerticalAlignment.Stretch;
            _editorId = $"monaco_{GetHashCode()}_{DateTime.Now.Ticks}";
            Loaded += OnLoaded;

            try
            {
                HtmlPage.RegisterScriptableObject($"MonacoCallback_{_editorId}", this);
                Console.WriteLine($"[MonacoEditor2] Registered scriptable object: MonacoCallback_{_editorId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MonacoEditor2] Error registering scriptable object: {ex.Message}");
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _div = Interop.GetDiv(this);
            TryGetParentSizeAndCreate();
        }

        public void TryGetParentSizeAndCreate()
        {
            double width = 600, height = 400;
            var parent = this.Parent as FrameworkElement;
            if (parent != null)
            {
                width = parent.ActualWidth > 0 ? parent.ActualWidth : 600;
                height = parent.ActualHeight > 0 ? parent.ActualHeight : 400;
            }
            this.Width = width;
            this.Height = height;
            Dispatcher.BeginInvoke(new Action(() => CreateEditor(width, height)),
                System.Windows.Threading.DispatcherPriority.Background);
        }

        private void CreateEditor(double width, double height)
        {
            if (_div == null) return;

            MonacoScriptHelper.RegisterCallback(_editorId, this);

            MonacoScriptHelper.LoadAndCreateEditor(_div, Code, Language, Theme, IsReadOnly, width, height, _editorId);
            _editorCreated = true;
        }

        [ScriptableMember]
        public void OnCodeChanged(string newCode)
        {
            Console.WriteLine($"[MonacoEditor2] User changed code: {newCode?.Length ?? 0} chars");

            if (!_isUpdatingFromCallback && !IsReadOnly)
            {
                _isUpdatingFromCallback = true;

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    Code = newCode;
                    base.OnCodeChanged(newCode); 
                    _isUpdatingFromCallback = false;
                }), System.Windows.Threading.DispatcherPriority.Normal);
            }
        }

        public void UpdateCode(string newCode)
        {
            if (_editorCreated && _div != null && newCode != null && !_isUpdatingFromCallback)
            {
                MonacoScriptHelper.UpdateEditorCode(_div, newCode);
            }
        }

        public void UpdateReadOnly(bool isReadOnly)
        {
            if (_editorCreated && _div != null)
            {
                MonacoScriptHelper.UpdateEditorReadOnly(_div, isReadOnly, _editorId);
            }
        }

        public void UpdateLanguage(string language)
        {
            if (_editorCreated && _div != null && !string.IsNullOrEmpty(language))
            {
                MonacoScriptHelper.UpdateEditorLanguage(_div, language);
            }
        }

        public void UpdateTheme(string theme)
        {
            if (_editorCreated && _div != null && !string.IsNullOrEmpty(theme))
            {
                MonacoScriptHelper.UpdateEditorTheme(_div, theme);
            }
        }
    }
}


