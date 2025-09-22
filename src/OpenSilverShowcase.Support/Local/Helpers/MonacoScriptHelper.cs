using OpenSilver;

namespace OpenSilverShowcase.Support.Local.Helpers
{
    public static class MonacoScriptHelper
    {
        private static string _assetBase = "https://unpkg.com/monaco-editor@0.50.0/min"; // 고정 버전 권장
        private static bool _useCdn = true;

        public static void ConfigureAssets(string assetBase, bool useCdn)
        {
            _assetBase = string.IsNullOrWhiteSpace(assetBase) ? _assetBase : assetBase.TrimEnd('/');
            _useCdn = useCdn;
        }

        public static void LoadAndCreateEditor(object div, string code, string language, string theme, bool isReadOnly, double width, double height, string editorId)
        {
            string vsPath = "/assets/monaco/min";
            var jsCode = $@"
                (function(){{
                    const div = $0;
                    const width = {width};
                    const height = {height};
                    const initialCode = {System.Text.Json.JsonSerializer.Serialize(code ?? "")};
                    const initialLang = {System.Text.Json.JsonSerializer.Serialize(language ?? "javascript")};
                    const initialTheme = {System.Text.Json.JsonSerializer.Serialize(theme ?? "vs-dark")};
                    const isReadOnly = {(isReadOnly ? "true" : "false")};
                    const editorId = {System.Text.Json.JsonSerializer.Serialize(editorId)};
                    const baseUrl = {System.Text.Json.JsonSerializer.Serialize(_assetBase)};
                    const vsPath = baseUrl + '/vs';

                    // 초기 스타일
                    div.style.width = width + 'px';
                    div.style.height = height + 'px';
                    div.style.display = 'block';
                    div.style.position = 'relative';
                    div.style.backgroundColor = '#1e1e1e';
                    div.style.boxSizing = 'border-box';
                    if (!div.innerHTML) {{
                        div.innerHTML = '<div style=""padding:20px;color:#ccc;font-family:monospace;height:100%;display:flex;align-items:center;justify-content:center;"">Loading Monaco Editor...</div>';
                    }}

                    function ensureMonacoReady() {{
                        if (window.__monacoReadyPromise) return window.__monacoReadyPromise;

                        window.__monacoReadyPromise = new Promise(function(resolve, reject){{
                            if (window.monaco && window.require) {{
                                return resolve(window.monaco);
                            }}

                            // AMD 로더 스크립트 로드
                            const loaderUrl = baseUrl + '/vs/loader.js';
                            const s = document.createElement('script');
                            s.src = loaderUrl;
                            s.async = true;
                            s.onload = function(){{
                                try {{
                                    // require 경로 설정
                                    window.require.config({{ paths: {{ vs: vsPath }} }});

                                    // 워커 경로 명시 (로컬/CDN 모두에서 안전하게 동작하도록 Blob 워커 사용)
                                    window.MonacoEnvironment = {{
                                        getWorkerUrl: function(moduleId, label){{
                                            const code = `
                                                self.MonacoEnvironment = {{ baseUrl: '{vsPath}/' }};
                                                importScripts('{vsPath}/base/worker/workerMain.js');
                                            `;
                                            return URL.createObjectURL(new Blob([code], {{ type: 'text/javascript' }}));
                                        }}
                                    }};

                                    window.require(['vs/editor/editor.main'], function(){{
                                        resolve(window.monaco);
                                    }}, reject);
                                }} catch(e) {{
                                    reject(e);
                                }}
                            }};
                            s.onerror = reject;
                            document.head.appendChild(s);
                        }});

                        return window.__monacoReadyPromise;
                    }}

                    function attachMonacoImageUpload(editor, opts = {{}}) {{
                        const node = editor.getDomNode();
                        function getCookieValue(name){{
                            const cookies = document.cookie.split(';');
                            for (let c of cookies){{
                                c = c.trim();
                                if (c.startsWith(name + '=')) return c.substring((name + '=').length);
                            }}
                            return null;
                        }}
                        async function uploadImage(blob){{
                            const jwtToken = getCookieValue('JwtToken');
                            if (!jwtToken) throw new Error('No JWT');
                            const formData = new FormData();
                            formData.append('image', blob);
                            if (window.currentPageId) formData.append('pageId', window.currentPageId);
                            const res = await fetch('https://opensilverserver.azurewebsites.net/api/docs/contents/upload-image', {{
                                method: 'POST',
                                body: formData,
                                headers: {{ Authorization: `Bearer ${{jwtToken}}` }}
                            }});
                            if (!res.ok) throw new Error('Upload failed: ' + res.statusText);
                            return await res.text();
                        }}
                        function insertAtCursor(ed, text){{
                            const sel = ed.getSelection();
                            ed.executeEdits('image-upload', [{{ range: sel, text, forceMoveMarkers: true }}]);
                            ed.focus();
                        }}
                        async function handleFiles(ed, files){{
                            for (const f of files){{
                                if (!f || !f.type || !f.type.startsWith('image/')) continue;
                                try {{
                                    const url = await uploadImage(f);
                                    insertAtCursor(ed, `![](${{url}})`);
                                }} catch (e) {{
                                    insertAtCursor(ed, '');
                                }}
                            }}
                        }}
                        node.addEventListener('paste', e => {{
                            const items = (e.clipboardData && e.clipboardData.items) ? Array.from(e.clipboardData.items) : [];
                            const files = items.map(i => i.getAsFile && i.getAsFile()).filter(f => f && f.type && f.type.startsWith('image/'));
                            if (files.length){{
                                e.preventDefault();
                                handleFiles(editor, files);
                            }}
                        }});
                        node.addEventListener('drop', e => {{
                            const files = Array.from(e.dataTransfer?.files || []).filter(f => f.type && f.type.startsWith('image/'));
                            if (files.length){{
                                e.preventDefault();
                                handleFiles(editor, files);
                            }}
                        }});
                    }}

                    function getOrCreateModel(value, language, uriStr){{
                      const monaco = window.monaco;
                      const uri = monaco.Uri.parse(uriStr);
                      let model = monaco.editor.getModel(uri);
                      if (!model) {{
                        model = monaco.editor.createModel(value, language, uri);
                      }} else {{
                        if (model.getValue() !== value) model.setValue(value);
                        // ▼ 여기!
                        if (model.getLanguageId && model.getLanguageId() !== language) {{
                          monaco.editor.setModelLanguage(model, language);
                        }}
                      }}
                      return model;
                    }}


                    (async function bootstrap(){{
                        try {{
                            await ensureMonacoReady();

                            // 이미 생성된 편집기가 있으면 재사용(모델/옵션만 갱신)
                            if (div._monacoEditor) {{
                                const ed = div._monacoEditor;
                                const uriStr = 'inmemory://model/' + editorId;
                                const model = getOrCreateModel(initialCode, initialLang, uriStr);
                                ed.setModel(model);
                                ed.updateOptions({{ readOnly: isReadOnly, padding: {{ top: 16, bottom: 16 }}, minimap: {{ enabled: false }}, scrollBeyondLastLine: false, lineNumbers: 'on' }});
                                if (window.monaco) window.monaco.editor.setTheme(initialTheme);
                                ed.layout({{ width: Math.max(0, width-2), height: Math.max(0, height-2) }});
                                return;
                            }}

                            // 최초 생성
                            div.innerHTML = '';
                            const uriStr = 'inmemory://model/' + editorId;
                            const model = getOrCreateModel(initialCode, initialLang, uriStr);

                            const editor = window.monaco.editor.create(div, {{
                                model: model,
                                theme: initialTheme,
                                readOnly: isReadOnly,
                                padding: {{ top: 16, bottom: 16 }},
                                automaticLayout: true,
                                minimap: {{ enabled: false }},
                                scrollBeyondLastLine: false,
                                lineNumbers: 'on'
                            }});

                            attachMonacoImageUpload(editor);
                            div._monacoEditor = editor;
                            div._editorId = editorId;

                            editor.onDidChangeModelContent(function(){{
                                try {{
                                    const newCode = editor.getValue();
                                    const callbackName = 'MonacoCallback_' + editorId;
                                    if (window[callbackName] && typeof window[callbackName].OnCodeChanged === 'function') {{
                                        window[callbackName].OnCodeChanged(newCode);
                                    }}
                                }} catch (err) {{
                                    console.error('[Monaco] Callback error:', err);
                                }}
                            }});

                            editor.layout({{ width: Math.max(0, width-2), height: Math.max(0, height-2) }});
                        }} catch (e) {{
                            console.error('[Monaco] bootstrap error', e);
                        }}
                    }})();
                }})();
            ";

            Interop.ExecuteJavaScriptVoid(jsCode, div);
        }

        public static void UpdateEditorCode(object div, string newCode)
        {
            Interop.ExecuteJavaScriptVoid($@"
        (function(){{
            const div = $0;
            if (!div || !div._monacoEditor) return;
            const ed = div._monacoEditor;
            const current = ed.getValue();
            const next = {System.Text.Json.JsonSerializer.Serialize(newCode ?? "")};
            if (current !== next) ed.setValue(next);
        }})();
    ", div);
        }


        public static void UpdateEditorReadOnly(object div, bool isReadOnly, string editorId)
        {
            Interop.ExecuteJavaScriptVoid($@"
                (function(){{
                    const div = $0;
                    if (!div || !div._monacoEditor) return;
                    div._monacoEditor.updateOptions({{ readOnly: {(isReadOnly ? "true" : "false")} }});
                }})();
            ", div);
        }

        public static void UpdateEditorLanguage(object div, string language)
        {
            Interop.ExecuteJavaScriptVoid($@"
                (function(){{
                    const div = $0;
                    if (!div || !div._monacoEditor || !window.monaco) return;
                    const model = div._monacoEditor.getModel();
                    if (!model) return;
                    const lang = {System.Text.Json.JsonSerializer.Serialize(language ?? "javascript")};
                    if (model.getLanguageId && model.getLanguageId() !== lang) {{
                        window.monaco.editor.setModelLanguage(model, lang);
                    }}

                }})();
            ", div);
        }

        public static void UpdateEditorTheme(object div, string theme)
        {
            Interop.ExecuteJavaScriptVoid($@"
                (function(){{
                    if (!window.monaco) return;
                    window.monaco.editor.setTheme({System.Text.Json.JsonSerializer.Serialize(theme ?? "vs-dark")});
                }})();
            ");
        }
    }
}
