using OpenSilver;

namespace OpenSilverShowcase.Support.Local.Helpers
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
}