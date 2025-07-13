using CSHTML5.Native.Html.Controls;
using OpenSilver;
using System;
using System.Windows;

namespace OSFSample.Support.UI.Units;

public class MonacoEditor : HtmlPresenter
{
    private const string CdnUrl = "https://unpkg.com/monaco-editor@latest";

    private object _domElement;
    private bool _isLoaded = false;
    private string _uniqueIframeId;

    #region Code Property
    public string Code
    {
        get => (string)GetValue(CodeProperty);
        set => SetValue(CodeProperty, value);
    }

    public static readonly DependencyProperty CodeProperty =
        DependencyProperty.Register(nameof(Code), typeof(string), typeof(MonacoEditor),
        new PropertyMetadata(string.Empty, OnCodeChanged));

    private static void OnCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (MonacoEditor)d;
        if (editor._isLoaded && editor._domElement != null)
        {
            var newCode = e.NewValue?.ToString() ?? string.Empty;
            var escapedCode = EscapeJavaScriptString(newCode);
            string jsCode = "document.getElementById('" + editor._uniqueIframeId + "').contentWindow.setEditorContent(`" + escapedCode + "`)";
            Interop.ExecuteJavaScriptVoid(jsCode);
        }
    }
    #endregion

    #region Language Property
    public new string Language
    {
        get => (string)GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    public static new readonly DependencyProperty LanguageProperty =
        DependencyProperty.Register(nameof(Language), typeof(string), typeof(MonacoEditor),
        new PropertyMetadata("plaintext", OnLanguageChanged));

    private static void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (MonacoEditor)d;
        if (editor._isLoaded && editor._domElement != null)
        {
            var newLanguage = e.NewValue?.ToString() ?? "plaintext";
            string jsCode = "document.getElementById('" + editor._uniqueIframeId + "').contentWindow.setEditorLanguage('" + newLanguage + "')";
            Interop.ExecuteJavaScriptVoid(jsCode);
        }
    }
    #endregion

    #region IsReadOnly Property
    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public static readonly DependencyProperty IsReadOnlyProperty =
        DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(MonacoEditor),
        new PropertyMetadata(false, OnIsReadOnlyChanged));

    private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (MonacoEditor)d;
        if (editor._isLoaded && editor._domElement != null)
        {
            string jsValue = e.NewValue.ToString().ToLower();
            string jsCode = "document.getElementById('" + editor._uniqueIframeId + "').contentWindow.setReadOnly(" + jsValue + ")";
            Interop.ExecuteJavaScriptVoid(jsCode);
        }
    }
    #endregion

    #region Theme Property
    public string Theme
    {
        get => (string)GetValue(ThemeProperty);
        set => SetValue(ThemeProperty, value);
    }

    public static readonly DependencyProperty ThemeProperty =
        DependencyProperty.Register(nameof(Theme), typeof(string), typeof(MonacoEditor),
        new PropertyMetadata("vs-dark", OnThemeChanged));

    private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (MonacoEditor)d;
        if (editor._isLoaded && editor._domElement != null)
        {
            var newTheme = e.NewValue?.ToString() ?? "vs-dark";
            string jsCode = "document.getElementById('" + editor._uniqueIframeId + "').contentWindow.setTheme('" + newTheme + "')";
            Interop.ExecuteJavaScriptVoid(jsCode);
        }
    }
    #endregion

    public MonacoEditor()
    {
        _uniqueIframeId = $"monacoEditorIframe_{GetHashCode()}";
        Loaded += MonacoEditor_Loaded;
        Unloaded += MonacoEditor_Unloaded;
    }

    private void MonacoEditor_Loaded(object sender, RoutedEventArgs e)
    {
        _domElement = Interop.GetDiv(this); // ALWAYS update
        RefreshMonacoEditor();
    }

    private void MonacoEditor_Unloaded(object sender, RoutedEventArgs e)
    {
        _isLoaded = false; // Optionally clear any other state
    }

    private void OnVisibilityChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (IsVisible)
        {
            bool iframeExists = false;
            try
            {
                var jsResult = Interop.ExecuteJavaScript($"!!document.getElementById('{_uniqueIframeId}')");
                iframeExists = jsResult != null && jsResult.ToString().ToLower() == "true";
            }
            catch { }

            if (!iframeExists)
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    RefreshMonacoEditor();
                }), System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }
    }



    private void RefreshMonacoEditor()
    {
        try
        {
            // 기존 iframe 제거하고 완전히 재생성
            string jsCode = @"
                var existingIframe = document.getElementById('" + _uniqueIframeId + @"');
                if (existingIframe) {
                    existingIframe.remove();
                    console.log('Existing iframe removed');
                }
            ";
            Interop.ExecuteJavaScriptVoid(jsCode);

            // 200ms 후 새로운 Monaco Editor 생성
            Dispatcher.BeginInvoke(new Action(() =>
            {
                RecreateMonacoEditor();
            }), System.Windows.Threading.DispatcherPriority.Loaded);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Monaco refresh error: {ex.Message}");
        }
    }

    private void RecreateMonacoEditor()
    {
        try
        {
            // 새로운 고유 ID 생성
            _uniqueIframeId = $"monacoEditorIframe_{GetHashCode()}_{DateTime.Now.Ticks}";

            // 기존 OnLoaded 메서드와 동일한 로직 재사용
            var escapedCode = EscapeJavaScriptString(Code);

            Interop.ExecuteJavaScriptVoidAsync($$"""
            (function () {
                try {
                    const iframe = document.createElement('iframe');
                    iframe.setAttribute('id','{{_uniqueIframeId}}');
                    iframe.style.cssText = 'border:none;width:100%;height:100%;overflow:hidden;display:block;margin:0;padding:0;';

                    const currentDiv = $0;
                    currentDiv.appendChild(iframe);

                    const iframeDoc = iframe.contentDocument || iframe.contentWindow.document;
                    iframeDoc.open();
                    
                    const htmlContent = `
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset="utf-8">
                            <style>
                                * { margin: 0; padding: 0; box-sizing: border-box; }
                                body, html { width: 100%; height: 100%; overflow: hidden; font-family: 'Segoe UI', sans-serif; }
                                #monacoContainer { width: 100%; height: 100%; }
                                .loading { display: flex; justify-content: center; align-items: center; height: 100%; background: #1e1e1e; color: #cccccc; font-size: 14px; }
                            </style>
                        </head>
                        <body>
                            <div id="monacoContainer">
                                <div class="loading">Reloading Monaco Editor...</div>
                            </div>
                        </body>
                        </html>
                    `;
                    
                    iframeDoc.write(htmlContent);
                    iframeDoc.close();

                    const loaderScript = iframeDoc.createElement('script');
                    loaderScript.src = '{{CdnUrl}}/min/vs/loader.js';
                    loaderScript.onload = function() {
                        const configScript = iframeDoc.createElement('script');
                        configScript.textContent = `
                            require.config({ paths: { 'vs': '{{CdnUrl}}/min/vs' } });
                            require(['vs/editor/editor.main'], function() {
                                try {
                                    const container = document.getElementById('monacoContainer');
                                    container.innerHTML = '';
                                    
                                    const editor = monaco.editor.create(container, {
                                        value: \`{{escapedCode}}\`,
                                        language: '{{Language}}',
                                        theme: '{{Theme}}',
                                        automaticLayout: true,
                                        readOnly: {{IsReadOnly.ToString().ToLower()}},
                                        padding: { top: 16, bottom: 16 },
                                        minimap: { enabled: true },
                                        scrollBeyondLastLine: false,
                                        wordWrap: 'on',
                                        lineNumbers: 'on',
                                        folding: true
                                    });

                                    container.editor = editor;
                                    window.monacoEditor = editor;
                                    
                                    setTimeout(() => {
                                        editor.layout();
                                        console.log('Monaco Editor recreated successfully');
                                    }, 100);
                                    
                                } catch (error) {
                                    console.error('Error creating Monaco Editor:', error);
                                }
                            });
                        `;
                        iframeDoc.head.appendChild(configScript);
                    };
                    
                    iframeDoc.head.appendChild(loaderScript);

                    const helperScript = iframeDoc.createElement('script');
                    helperScript.textContent = `
                        window.setEditorContent = function(content) {
                            try {
                                const editor = window.monacoEditor;
                                if (editor && editor.getValue() !== content) {
                                    editor.getModel().setValue(content);
                                }
                            } catch (e) {
                                console.error('Error setting editor content:', e);
                            }
                        };

                        window.setEditorLanguage = function(language) {
                            try {
                                const editor = window.monacoEditor;
                                if (editor) {
                                    monaco.editor.setModelLanguage(editor.getModel(), language);
                                }
                            } catch (e) {
                                console.error('Error setting editor language:', e);
                            }
                        };
                    `;
                    iframeDoc.head.appendChild(helperScript);

                } catch (error) {
                    console.error('Error initializing Monaco Editor iframe:', error);
                }
            })();
        """, _domElement, Language, (Action<string>)OnCodeChangedInEditor);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Monaco recreation error: {ex.Message}");
        }
    }


    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;
        _domElement = Interop.GetDiv(this);

        var escapedCode = EscapeJavaScriptString(Code);

        Interop.ExecuteJavaScriptVoidAsync($$"""
            (function () {
                try {
                    const iframe = document.createElement('iframe');
                    iframe.setAttribute('id','{{_uniqueIframeId}}');
                    iframe.style.cssText = 'border:none;width:100%;height:100%;overflow:hidden;display:block;margin:0;padding:0;';

                    const currentDiv = $0;
                    currentDiv.appendChild(iframe);

                    const iframeDoc = iframe.contentDocument || iframe.contentWindow.document;
                    iframeDoc.open();
                    
                    const htmlContent = `
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset="utf-8">
                            <style>
                                * { margin: 0; padding: 0; box-sizing: border-box; }
                                body, html {
                                    width: 100%;
                                    height: 100%;
                                    overflow: hidden;
                                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                }
                                #monacoContainer {
                                    width: 100%;
                                    height: 100%;
                                }
                                .loading {
                                    display: flex;
                                    justify-content: center;
                                    align-items: center;
                                    height: 100%;
                                    background: #1e1e1e;
                                    color: #cccccc;
                                    font-size: 14px;
                                }
                            </style>
                        </head>
                        <body>
                            <div id="monacoContainer">
                                <div class="loading">Loading Monaco Editor...</div>
                            </div>
                        </body>
                        </html>
                    `;
                    
                    iframeDoc.write(htmlContent);
                    iframeDoc.close();

                    // Monaco Loader 스크립트 로드
                    const loaderScript = iframeDoc.createElement('script');
                    loaderScript.src = '{{CdnUrl}}/min/vs/loader.js';
                    loaderScript.onload = function() {
                        const configScript = iframeDoc.createElement('script');
                        configScript.textContent = `
                            require.config({ 
                                paths: { 'vs': '{{CdnUrl}}/min/vs' },
                                'vs/nls': { availableLanguages: { '*': 'en' } }
                            });
                            
                            require(['vs/editor/editor.main'], function() {
                                try {
                                    const container = document.getElementById('monacoContainer');
                                    container.innerHTML = '';
                                    
                                    const editor = monaco.editor.create(container, {
                                        value: \`{{escapedCode}}\`,
                                        language: '{{Language}}',
                                        theme: '{{Theme}}',
                                        automaticLayout: true,
                                        readOnly: {{IsReadOnly.ToString().ToLower()}},
                                        domReadOnly: {{IsReadOnly.ToString().ToLower()}},
                                        padding: { top: 16, bottom: 16 },
                                        minimap: { enabled: true },
                                        scrollBeyondLastLine: false,
                                        wordWrap: 'on',
                                        lineNumbers: 'on',
                                        glyphMargin: false,
                                        folding: true,
                                        lineDecorationsWidth: 10,
                                        lineNumbersMinChars: 3,
                                        renderLineHighlight: 'line',
                                        contextmenu: {{(!IsReadOnly).ToString().ToLower()}},
                                        selectOnLineNumbers: true,
                                        roundedSelection: false,
                                        scrollbar: {
                                            useShadows: false,
                                            verticalHasArrows: false,
                                            horizontalHasArrows: false,
                                            vertical: 'visible',
                                            horizontal: 'visible'
                                        }
                                    });

                                    // 전역 참조 저장
                                    container.editor = editor;
                                    window.monacoEditor = editor;

                                    // 콘텐츠 변경 이벤트 (읽기 전용이 아닐 때만)
                                    if (!{{IsReadOnly.ToString().ToLower()}}) {
                                        editor.onDidChangeModelContent((event) => {
                                            try {
                                                const content = editor.getValue();
                                                if (window.updateContentInParent) {
                                                    window.updateContentInParent(content);
                                                }
                                            } catch (e) {
                                                console.error('Error in content change handler:', e);
                                            }
                                        });
                                    }

                                    // 에디터 준비 완료
                                    console.log('Monaco Editor initialized successfully');
                                    
                                    // 지연 후 강제 리사이즈 및 레이아웃 갱신
                                    setTimeout(() => {
                                        try {
                                            editor.layout();
                                            container.style.height = '100%';
                                            container.style.width = '100%';
                                            editor.layout();
                                            console.log('Monaco Editor layout refreshed');
                                        } catch (e) {
                                            console.error('Layout refresh error:', e);
                                        }
                                    }, 100);
                                    
                                    // 추가 안전장치: 1초 후 한 번 더 리사이즈
                                    setTimeout(() => {
                                        try {
                                            editor.layout();
                                        } catch (e) {
                                            console.error('Secondary layout error:', e);
                                        }
                                    }, 1000);
                                    
                                } catch (error) {
                                    console.error('Error creating Monaco Editor:', error);
                                    document.getElementById('monacoContainer').innerHTML = 
                                        '<div class="loading">Error loading Monaco Editor</div>';
                                }
                            });
                        `;
                        iframeDoc.head.appendChild(configScript);
                    };
                    
                    loaderScript.onerror = function() {
                        console.error('Failed to load Monaco Editor');
                        iframeDoc.getElementById('monacoContainer').innerHTML = 
                            '<div class="loading">Failed to load Monaco Editor</div>';
                    };
                    
                    iframeDoc.head.appendChild(loaderScript);

                    // 헬퍼 함수들 정의
                    const helperScript = iframeDoc.createElement('script');
                    helperScript.textContent = `
                        window.setEditorContent = function(content) {
                            try {
                                const editor = window.monacoEditor;
                                if (editor && editor.getValue() !== content) {
                                    editor.getModel().setValue(content);
                                }
                            } catch (e) {
                                console.error('Error setting editor content:', e);
                            }
                        };

                        window.setEditorLanguage = function(language) {
                            try {
                                const editor = window.monacoEditor;
                                if (editor) {
                                    monaco.editor.setModelLanguage(editor.getModel(), language);
                                }
                            } catch (e) {
                                console.error('Error setting editor language:', e);
                            }
                        };

                        window.setReadOnly = function(isReadOnly) {
                            try {
                                const editor = window.monacoEditor;
                                if (editor) {
                                    editor.updateOptions({ 
                                        readOnly: isReadOnly,
                                        domReadOnly: isReadOnly,
                                        contextmenu: !isReadOnly
                                    });
                                }
                            } catch (e) {
                                console.error('Error setting read-only mode:', e);
                            }
                        };

                        window.setTheme = function(theme) {
                            try {
                                if (window.monaco) {
                                    monaco.editor.setTheme(theme);
                                }
                            } catch (e) {
                                console.error('Error setting theme:', e);
                            }
                        };

                        window.getEditorContent = function() {
                            try {
                                const editor = window.monacoEditor;
                                return editor ? editor.getValue() : '';
                            } catch (e) {
                                console.error('Error getting editor content:', e);
                                return '';
                            }
                        };
                    `;
                    iframeDoc.head.appendChild(helperScript);

                    // 부모 창과의 통신 설정
                    iframe.contentWindow.updateContentInParent = function(content) {
                        try {
                            $2(content);
                        } catch (e) {
                            console.error('Error communicating with parent:', e);
                        }
                    };

                } catch (error) {
                    console.error('Error initializing Monaco Editor iframe:', error);
                }
            })();
        """, _domElement, Language, (Action<string>)OnCodeChangedInEditor);

        _isLoaded = true;
    }

    private void OnCodeChangedInEditor(string newCode)
    {
        if (!IsReadOnly && Code != newCode)
        {
            Code = newCode;
        }
    }

    private static string EscapeJavaScriptString(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        return input
            .Replace("\\", "\\\\")
            .Replace("`", "\\`")
            .Replace("$", "\\$")
            .Replace("\r\n", "\\n")
            .Replace("\r", "\\n")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t")
            .Replace("\"", "\\\"")
            .Replace("'", "\\'");
    }

    public void SetContent(string content)
    {
        Code = content;
    }

    public string GetContent()
    {
        if (_isLoaded && _domElement != null)
        {
            try
            {
                string jsCode = "document.getElementById('" + _uniqueIframeId + "').contentWindow.getEditorContent()";
                var result = Interop.ExecuteJavaScript(jsCode);
                return result?.ToString() ?? string.Empty;
            }
            catch
            {
                return Code;
            }
        }
        return Code;
    }

    public void Focus()
    {
        if (_isLoaded && _domElement != null)
        {
            try
            {
                string jsCode = "document.getElementById('" + _uniqueIframeId + "').contentWindow.monacoEditor?.focus()";
                Interop.ExecuteJavaScriptVoid(jsCode);
            }
            catch
            {
                // Ignore focus errors
            }
        }
    }
}