using CSHTML5.Native.Html.Controls;
using OpenSilver;
using System;
using System.Text.Json;
using System.Windows;
using System.Windows.Browser;

namespace OSFSample.Support.UI.Units;

public class MonacoEditor : HtmlPresenter
{
    private const string CdnUrl = "https://unpkg.com/monaco-editor@latest";
    private object _domElement;
    private bool _isLoaded = false;
    private string _uniqueIframeId;
    private bool _isCreating = false; // 중복 생성 방지 플래그 추가

    public event EventHandler<string> CodeChanged;

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
        Console.WriteLine($"[Monaco] OnCodeChanged fired. NewValue={e.NewValue?.ToString()?.Substring(0, Math.Min(80, e.NewValue?.ToString()?.Length ?? 0))}");
        editor.SyncCodeToJs();
    }

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
        Console.WriteLine($"[Monaco] OnLanguageChanged fired. NewValue={e.NewValue}");
        editor.SyncLanguageToJs();
    }

    public bool IsReadOnly
    {
        get => (bool)GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }

    public static readonly DependencyProperty IsReadOnlyProperty =
        DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(MonacoEditor),
        new PropertyMetadata(true, OnIsReadOnlyChanged));

    private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var editor = (MonacoEditor)d;
        Console.WriteLine($"[Monaco] OnIsReadOnlyChanged fired. NewValue={e.NewValue}");
        editor.SyncReadOnlyToJs();
    }

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
        Console.WriteLine($"[Monaco] OnThemeChanged fired. NewValue={e.NewValue}");
        editor.SyncThemeToJs();
    }

    public MonacoEditor()
    {
        _uniqueIframeId = $"monacoEditorIframe_{GetHashCode()}_{DateTime.Now.Ticks}";
        Console.WriteLine("[Monaco] MonacoEditor() ctor called, _uniqueIframeId = " + _uniqueIframeId);
        try
        {
            HtmlPage.RegisterScriptableObject("MonacoBridge", this);
            Console.WriteLine("[Monaco] RegisterScriptableObject('MonacoBridge', this) called OK.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("[Monaco][ERROR] RegisterScriptableObject failed: " + ex);
        }
        Loaded += MonacoEditor_Loaded;
        Unloaded += MonacoEditor_Unloaded;
        IsVisibleChanged += MonacoEditor_IsVisibleChanged;
    }

    private void MonacoEditor_Loaded(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("[Monaco] MonacoEditor_Loaded called.");
        _domElement = Interop.GetDiv(this);
        Console.WriteLine("[Monaco] Interop.GetDiv OK. domElement = " + (_domElement != null));
        RefreshMonacoEditor();
    }

    private void MonacoEditor_Unloaded(object sender, RoutedEventArgs e)
    {
        Console.WriteLine("[Monaco] MonacoEditor_Unloaded called.");
        _isLoaded = false;
        _isCreating = false; // 플래그 리셋
        CleanupMonacoEditor(); // 완전한 정리 추가
    }

    private void MonacoEditor_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        Console.WriteLine($"[Monaco] MonacoEditor_IsVisibleChanged: IsVisible={IsVisible}, _isCreating={_isCreating}");

        // 이미 생성 중이면 무시
        if (_isCreating)
        {
            Console.WriteLine("[Monaco] Already creating editor, ignoring visibility change.");
            return;
        }

        if (IsVisible && _domElement != null)
        {
            bool iframeExists = false;
            try
            {
                var jsResult = Interop.ExecuteJavaScript($"!!document.getElementById('{_uniqueIframeId}')");
                iframeExists = jsResult != null && jsResult.ToString().ToLower() == "true";
                Console.WriteLine($"[Monaco] Iframe exists check: {iframeExists} (id={_uniqueIframeId})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Monaco][ERROR] IsVisibleChanged iframe check: {ex.Message}");
            }

            if (!iframeExists && !_isLoaded)
            {
                Console.WriteLine("[Monaco] Iframe not found and not loaded, refreshing Monaco editor.");
                // 지연 시간을 더 줘서 DOM이 완전히 준비되도록 함
                Dispatcher.BeginInvoke(new Action(() => RefreshMonacoEditor()),
                    System.Windows.Threading.DispatcherPriority.Background);
            }
        }
    }

    private void RefreshMonacoEditor()
    {
        Console.WriteLine("[Monaco] RefreshMonacoEditor() called.");

        // 이미 생성 중이면 무시
        if (_isCreating)
        {
            Console.WriteLine("[Monaco] Already creating editor, skipping refresh.");
            return;
        }

        try
        {
            _isCreating = true; // 생성 시작 플래그 설정
            _isLoaded = false;

            // DOM 요소 완전 정리
            CleanupMonacoEditor();

            // 새로운 ID 생성
            _uniqueIframeId = $"monacoEditorIframe_{GetHashCode()}_{DateTime.Now.Ticks}";
            Console.WriteLine("[Monaco] New _uniqueIframeId = " + _uniqueIframeId);

            // 지연 실행으로 DOM 정리가 완료되도록 함
            Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    RecreateMonacoEditor();
                }
                finally
                {
                    _isCreating = false; // 생성 완료 후 플래그 해제
                }
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
        catch (Exception ex)
        {
            Console.WriteLine("[Monaco][ERROR] RefreshMonacoEditor: " + ex);
            _isCreating = false; // 오류 시에도 플래그 해제
        }
    }

    private void CleanupMonacoEditor()
    {
        Console.WriteLine("[Monaco] CleanupMonacoEditor() called.");
        try
        {
            // 현재 DOM 요소의 모든 자식 요소 제거
            if (_domElement != null)
            {
                Interop.ExecuteJavaScriptVoid("$0.innerHTML = '';", _domElement);
                Console.WriteLine("[Monaco] Current DOM element cleared.");
            }

            // 전체 Monaco iframe들 제거 (보험용)
            var removeAll = "Array.from(document.querySelectorAll('iframe[id^=\"monacoEditorIframe_\"]')).forEach(x=>x.remove());";
            Interop.ExecuteJavaScriptVoid(removeAll);
            Console.WriteLine("[Monaco] All monacoEditorIframe_* removed globally.");

            // 약간의 지연으로 DOM 정리가 완료되도록 함
            System.Threading.Thread.Sleep(50);
        }
        catch (Exception ex)
        {
            Console.WriteLine("[Monaco][ERROR] CleanupMonacoEditor: " + ex);
        }
    }

    private void RecreateMonacoEditor()
    {
        Console.WriteLine($"[Monaco] RecreateMonacoEditor() called, _uniqueIframeId = {_uniqueIframeId}");

        if (_domElement == null)
        {
            Console.WriteLine("[Monaco] DOM element is null, cannot create editor.");
            return;
        }

        try
        {
            var escapedCode = EscapeJavaScriptString(Code);

            Interop.ExecuteJavaScriptVoidAsync($$"""
            (function () {
                try {
                    // 현재 DOM 요소가 비어있는지 확인
                    const currentDiv = $0;
                    if (currentDiv.children.length > 0) {
                        console.log('[Monaco] Current div has children, clearing...');
                        currentDiv.innerHTML = '';
                    }
                    
                    // 같은 ID의 iframe이 이미 존재하는지 다시 한 번 확인
                    const existiniframe = document.getElementById('{{_uniqueIframeId}}');
                    if (existiniframe) {
                        console.log('[Monaco] Iframe with same ID already exists, removing...');
                        existiniframe.remove();
                    }
                    
                    const iframe = document.createElement('iframe');
                    iframe.setAttribute('id', '{{_uniqueIframeId}}');
                    iframe.style.cssText = 'border:none;width:100%;height:100%;overflow:hidden;display:block;margin:0;padding:0;';
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
                                <div class="loading">Loading Monaco Editor...</div>
                            </div>
                            <script>
                                window.addEventListener('message', function(event) {
                                    if (event.data && event.data.cmd) {
                                        if (event.data.cmd === 'setReadOnly') window.setReadOnly(event.data.value);
                                        if (event.data.cmd === 'setEditorContent') window.setEditorContent(event.data.value);
                                        if (event.data.cmd === 'setEditorLanguage') window.setEditorLanguage(event.data.value);
                                        if (event.data.cmd === 'setTheme') window.setTheme(event.data.value);
                                    }
                                });
                                setTimeout(function() {
                                    if (window.parent && window.parent.MonacoBridge && typeof window.parent.MonacoBridge.OnMonacoMessage === 'function') {
                                        window.parent.MonacoBridge.OnMonacoMessage("[iframe] MonacoBridge 연결됨 - window.parent.MonacoBridge.OnMonacoMessage OK [id={{_uniqueIframeId}}]");
                                    } else {
                                        console.error('[iframe][Monaco] window.parent.MonacoBridge.OnMonacoMessage NOT FOUND! [id={{_uniqueIframeId}}]');
                                    }
                                }, 100);
                            </script>
                        </body>
                        </html>
                    `;
                    iframeDoc.write(htmlContent);
                    iframeDoc.close();
                    
                    iframe.onload = function() {
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
                                        
                                        window.setEditorContent = function(content) {
                                            try {
                                                if (editor && editor.getValue() !== content) {
                                                    editor.getModel().setValue(content);
                                                }
                                            } catch (e) {
                                                console.error('[iframe][Monaco] setEditorContent error:', e);
                                            }
                                        };
                                        window.setEditorLanguage = function(language) {
                                            try {
                                                if (editor) {
                                                    monaco.editor.setModelLanguage(editor.getModel(), language);
                                                }
                                            } catch (e) {
                                                console.error('[iframe][Monaco] setEditorLanguage error:', e);
                                            }
                                        };
                                        window.setReadOnly = function(isReadOnly) {
                                            try {
                                                if (editor) {
                                                    editor.updateOptions({ readOnly: isReadOnly });
                                                }
                                            } catch (e) {
                                                console.error('[iframe][Monaco] setReadOnly error:', e);
                                            }
                                        };
                                        window.setTheme = function(theme) {
                                            try {
                                                if (window.monaco) {
                                                    monaco.editor.setTheme(theme);
                                                }
                                            } catch (e) {
                                                console.error('[iframe][Monaco] setTheme error:', e);
                                            }
                                        };
                                        window.getEditorContent = function() {
                                            try {
                                                return editor ? editor.getValue() : '';
                                            } catch (e) {
                                                console.error('[iframe][Monaco] getEditorContent error:', e);
                                                return '';
                                            }
                                        };
                                        
                                        editor.onDidChangeModelContent(() => {
                                            try {
                                                const content = editor.getValue();
                                                if (window.parent && window.parent.MonacoBridge && typeof window.parent.MonacoBridge.OnMonacoMessage === 'function') {
                                                    window.parent.MonacoBridge.OnMonacoMessage(JSON.stringify({
                                                        monacoContentChanged: true,
                                                        content: content,
                                                        id: '{{_uniqueIframeId}}'
                                                    }));
                                                    console.log('[iframe][Monaco] onDidChangeModelContent: 메시지 부모에 전달 [id={{_uniqueIframeId}}]');
                                                } else {
                                                    console.error('[iframe][Monaco] onDidChangeModelContent: MonacoBridge not found [id={{_uniqueIframeId}}]');
                                                }
                                            } catch (e) {
                                                console.error('[iframe][Monaco] onDidChangeModelContent error:', e);
                                            }
                                        });
                                        
                                        setTimeout(() => { editor.layout(); }, 100);
                                        setTimeout(() => {
                                            if (window.parent && window.parent.MonacoBridge && typeof window.parent.MonacoBridge.OnMonacoMessage === 'function') {
                                                window.parent.MonacoBridge.OnMonacoMessage(JSON.stringify({
                                                    monacoReady: true,
                                                    id: '{{_uniqueIframeId}}'
                                                }));
                                                console.log('[iframe][Monaco] monacoReady: 부모에 신호 보냄 [id={{_uniqueIframeId}}]');
                                            } else {
                                                console.error('[iframe][Monaco] monacoReady: MonacoBridge not found [id={{_uniqueIframeId}}]');
                                            }
                                        }, 200);
                                    } catch (error) {
                                        console.error('[iframe][Monaco] Monaco editor creation error:', error);
                                    }
                                });
                            `;
                            iframeDoc.head.appendChild(configScript);
                        };
                        iframeDoc.head.appendChild(loaderScript);
                    };
                } catch (error) {
                    console.error('[Monaco][iframe][ERROR] Iframe setup error:', error);
                }
            })();
            """, _domElement);

            Console.WriteLine("[Monaco] RecreateMonacoEditor() completed, waiting for ready signal. id = " + _uniqueIframeId);
        }
        catch (Exception ex)
        {
            Console.WriteLine("[Monaco][ERROR] RecreateMonacoEditor: " + ex);
        }
    }

    [ScriptableMember]
    public void OnMonacoMessage(string json)
    {
        Console.WriteLine("[Monaco][Bridge] OnMonacoMessage called! JSON = " + json);

        try
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                Console.WriteLine("[Monaco][Bridge] OnMonacoMessage: json null/empty");
                return;
            }

            // 디버깅: 연결 확인 로그
            if (json.StartsWith("[iframe]"))
            {
                Console.WriteLine("[Monaco][Bridge] [iframe 브리지 연결 확인] " + json);
                return;
            }

            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var id = root.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
            Console.WriteLine($"[Monaco][Bridge] Incoming id={id}, My id={_uniqueIframeId}");
            if (id != _uniqueIframeId)
            {
                Console.WriteLine("[Monaco][Bridge] ID mismatch. (message ignored)");
                return;
            }

            if (root.TryGetProperty("monacoReady", out var ready) && ready.GetBoolean())
            {
                Console.WriteLine("[Monaco][Bridge] Monaco iframe signaled READY.");
                _isLoaded = true;
                _isCreating = false; // 완전히 준비되었으므로 플래그 해제
                SetMonacoState();
            }
            else if (root.TryGetProperty("monacoContentChanged", out var changed) && changed.GetBoolean())
            {
                var newCode = root.TryGetProperty("content", out var contentProp) ? contentProp.GetString() : "";
                Console.WriteLine($"[Monaco][Bridge] onDidChangeModelContent: NewCode={newCode?.Substring(0, Math.Min(80, newCode.Length))}");
                if (!IsReadOnly && Code != newCode)
                {
                    Code = newCode;
                    CodeChanged?.Invoke(this, newCode);
                    Console.WriteLine("[Monaco][Bridge] Code updated, CodeChanged event fired.");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("[Monaco][Bridge][ERROR] OnMonacoMessage: " + ex);
        }
    }

    private void SetMonacoState()
    {
        Console.WriteLine("[Monaco] SetMonacoState() called.");
        SyncReadOnlyToJs();
        SyncCodeToJs();
        SyncLanguageToJs();
        SyncThemeToJs();
    }

    private void PostToIframe(string cmd, object value)
    {
        if (_isLoaded && _domElement != null && !_isCreating)
        {
            var jsValue = value is bool b ? b.ToString().ToLower() : value is string s ? $"`{EscapeJavaScriptString(s)}`" : value;
            var js = $"document.getElementById('{_uniqueIframeId}').contentWindow.postMessage({{cmd:'{cmd}', value:{jsValue}}}, '*');";
            Console.WriteLine($"[Monaco] PostToIframe: {js.Substring(0, Math.Min(120, js.Length))}");
            Interop.ExecuteJavaScriptVoid(js);
        }
        else
        {
            Console.WriteLine($"[Monaco] PostToIframe skipped: Editor not loaded, dom null, or creating. cmd={cmd}, _isLoaded={_isLoaded}, _isCreating={_isCreating}");
        }
    }

    private void SyncReadOnlyToJs() => PostToIframe("setReadOnly", IsReadOnly);
    private void SyncCodeToJs() => PostToIframe("setEditorContent", Code);
    private void SyncLanguageToJs() => PostToIframe("setEditorLanguage", Language);
    private void SyncThemeToJs() => PostToIframe("setTheme", Theme);

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
        Console.WriteLine($"[Monaco] SetContent called. Content={content.Substring(0, Math.Min(80, content.Length))}");
        Code = content;
    }

    public string GetContent()
    {
        Console.WriteLine("[Monaco] GetContent called.");
        if (_isLoaded && _domElement != null)
        {
            try
            {
                string jsCode = $"document.getElementById('{_uniqueIframeId}').contentWindow.getEditorContent()";
                Console.WriteLine("[Monaco] GetContent: Executing " + jsCode);
                var result = Interop.ExecuteJavaScript(jsCode);
                var content = result?.ToString() ?? string.Empty;
                Console.WriteLine("[Monaco] GetContent: Returned " + content.Substring(0, Math.Min(80, content.Length)));
                return content;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Monaco][ERROR] GetContent: " + ex);
                return Code;
            }
        }
        Console.WriteLine("[Monaco] GetContent: Returning Code property.");
        return Code;
    }

    public void Focus()
    {
        Console.WriteLine("[Monaco] Focus called.");
        if (_isLoaded && _domElement != null)
        {
            try
            {
                string jsCode = $"document.getElementById('{_uniqueIframeId}').contentWindow.monacoEditor?.focus()";
                Console.WriteLine("[Monaco] Focus: Executing " + jsCode);
                Interop.ExecuteJavaScriptVoid(jsCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Monaco][ERROR] Focus: " + ex);
            }
        }
        else
        {
            Console.WriteLine("[Monaco] Focus skipped: Editor not loaded.");
        }
    }
}