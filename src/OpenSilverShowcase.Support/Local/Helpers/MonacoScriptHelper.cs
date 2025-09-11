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
                div.style.overflow = 'hidden';
                div.innerHTML = '<div style=""padding:20px;color:#cccccc;font-family:monospace;height:100%;width:100%;display:flex;align-items:center;justify-content:center;overflow:hidden;"">Loading Monaco Editor...</div>';

                function getCookieValue(name) {{
                    const cookies=document.cookie.split(';');
                    for(let cookie of cookies){{
                        cookie=cookie.trim();
                        if(cookie.startsWith(`${{name}}=`)) return cookie.substring(`${{name}}=`.length);
                    }}
                    return null;
                }}

                async function uploadImage(blob) {{
                    const jwtToken=getCookieValue('JwtToken');
                    if(!jwtToken) throw new Error('No JWT');
                    const formData=new FormData();
                    formData.append('image',blob);
                    if(window.currentPageId) formData.append('pageId',window.currentPageId);
                    const res=await fetch('https://opensilverserver.azurewebsites.net/api/docs/contents/upload-image',{{
                        method:'POST',
                        body:formData,
                        headers:{{Authorization:`Bearer ${{jwtToken}}`}}
                    }});
                    if(!res.ok) throw new Error(`Upload failed: ${{res.statusText}}`);
                    return await res.text();
                }}

                function insertAtCursor(editor,text){{
                    const sel=editor.getSelection();
                    editor.executeEdits('image-upload',[{{range:sel,text,forceMoveMarkers:true}}]);
                    editor.focus();
                }}

                async function handleFiles(editor,files){{
                    for(const f of files){{
                        if(!f.type||!f.type.startsWith('image/')) continue;
                        try {{
                            const url=await uploadImage(f);
                            insertAtCursor(editor,`![](${{url}})`);
                        }} catch(e){{
                            insertAtCursor(editor,'');
                        }}
                    }}
                }}

                function attachMonacoImageUpload(editor,{{pageIdProvider}}={{}}){{
                    const node=editor.getDomNode();
                    node.addEventListener('paste',e=>{{
                        const items=e.clipboardData&&e.clipboardData.items?Array.from(e.clipboardData.items):[];
                        const files=items.map(i=>i.getAsFile&&i.getAsFile()).filter(f=>f&&f.type&&f.type.startsWith('image/'));
                        if(files.length){{
                            e.preventDefault();
                            if(typeof pageIdProvider==='function') window.currentPageId=pageIdProvider();
                            handleFiles(editor,files);
                        }}
                    }});
                    node.addEventListener('drop',e=>{{
                        const files=Array.from(e.dataTransfer.files||[]).filter(f=>f.type&&f.type.startsWith('image/'));
                        if(files.length){{
                            e.preventDefault();
                            if(typeof pageIdProvider==='function') window.currentPageId=pageIdProvider();
                            handleFiles(editor,files);
                        }}
                    }});
                }}

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

                    attachMonacoImageUpload(editor, {{ pageIdProvider: () => window.currentPageId }});

                    div._monacoEditor = editor;
                    div._editorId = '{editorId}';
            
                    editor.onDidChangeModelContent(function(e) {{
                        try {{
                            const newCode = editor.getValue();
                            const callbackName = 'MonacoCallback_{editorId}';
                            if (window[callbackName] && typeof window[callbackName].OnCodeChanged === 'function') {{
                                window[callbackName].OnCodeChanged(newCode);
                            }}
                        }} catch (error) {{
                            console.error('[Monaco] Error in code change callback:', error);
                        }}
                    }});
            
                    const editorWidth = {width - 2};
                    const editorHeight = {height - 2};
                    editor.layout({{ width: editorWidth, height: editorHeight }});
                }}
            ", div);
        }

        public static void UpdateEditorCode(object div, string newCode)
        {
            Interop.ExecuteJavaScriptVoid($@"
                const div = $0;
                if (div._monacoEditor) {{
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
    }
}