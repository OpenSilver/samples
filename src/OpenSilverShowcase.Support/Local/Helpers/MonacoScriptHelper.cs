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
            var jsCode = $@"
    (function(){{
        const div = $0;
        const width = {width};
        const height = {height};
        const initialCode = {System.Text.Json.JsonSerializer.Serialize(code ?? "")};
        const initialLang  = {System.Text.Json.JsonSerializer.Serialize(language ?? "javascript")};
        const isReadOnly   = {(isReadOnly ? "true" : "false")};
        const editorId     = {System.Text.Json.JsonSerializer.Serialize(editorId)};
        const baseUrl      = {System.Text.Json.JsonSerializer.Serialize(_useCdn ? _assetBase : "/assets/monaco/min")};
        const vsPath       = baseUrl + '/vs';

        // ---------- Theme Bridge ----------
        // AppTheme(Light/Night) -> Monaco 테마명 매핑
        function mapAppThemeToMonaco(appTheme) {{
            if (!appTheme) return 'vs-dark';
            const v = String(appTheme).toLowerCase();
            if (v === 'light') return 'vs';
            if (v === 'night' || v === 'dark') return 'vs-dark';
            return 'vs-dark';
        }}
        function readAppTheme() {{
            try {{ return localStorage.getItem('AppTheme') || 'Night'; }} catch {{ return 'Night'; }}
        }}
        function getMonacoThemeFromLocalStorage() {{
            return mapAppThemeToMonaco(readAppTheme());
        }}
        function applyThemeForAllEditors(themeName) {{
            if (!window.monaco) return;
            window.monaco.editor.setTheme(themeName);
            if (Array.isArray(window.__monacoEditors)) {{
                for (const ed of window.__monacoEditors) {{
                    try {{ ed.updateOptions({{}}); }} catch {{}}
                }}
            }}
        }}
        function applyThemeFromLocalStorage() {{
            applyThemeForAllEditors(getMonacoThemeFromLocalStorage());
        }}
        function ensureThemeBridgeInstalled() {{
            if (window.__monacoThemeBridgeInstalled) return;
            window.__monacoThemeBridgeInstalled = true;

            // 같은 탭에서도 setItem('AppTheme', ...) 시 테마 반영되도록 훅
            try {{
                const _setItem = localStorage.setItem.bind(localStorage);
                localStorage.setItem = function(k, v) {{
                    _setItem(k, v);
                    if (k === 'AppTheme') {{
                        window.dispatchEvent(new CustomEvent('AppThemeChanged', {{ detail: {{ value: v }} }}));
                    }}
                }};
            }} catch {{}}

            // 같은 탭: 커스텀 이벤트
            window.addEventListener('AppThemeChanged', function() {{
                applyThemeFromLocalStorage();
            }});
            // 다른 탭/창: storage 이벤트
            window.addEventListener('storage', function(e) {{
                if (e && e.key === 'AppTheme') applyThemeFromLocalStorage();
            }});
        }}
        // ----------------------------------

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
                const loaderUrl = baseUrl + '/vs/loader.js';
                const s = document.createElement('script');
                s.src = loaderUrl;
                s.async = true;
                s.onload = function(){{
                    try {{
                        window.require.config({{ paths: {{ vs: vsPath }} }});
                        window.MonacoEnvironment = {{
                            getWorkerUrl: function (moduleId, label) {{
                                const vs = vsPath;
                                const workerModule =
                                    label === 'json'        ? 'vs/language/json/json.worker' :
                                    label === 'css'         ? 'vs/language/css/css.worker'   :
                                    label === 'html'        ? 'vs/language/html/html.worker' :
                                    (label === 'typescript' || label === 'javascript')
                                                            ? 'vs/language/typescript/ts.worker'
                                                            : 'vs/editor/editor.worker';
                                const code = `
                                    self.MonacoEnvironment = {{ baseUrl: '${{vs}}/' }};
                                    importScripts('${{vs}}/loader.js');
                                    require.config({{ paths: {{ vs: '${{vs}}' }} }});
                                    require(['${{workerModule}}'], function(){{}});
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

        function getOrCreateModel(value, language, uriStr){{
            const monaco = window.monaco;
            const uri = monaco.Uri.parse(uriStr);
            let model = monaco.editor.getModel(uri);
            if (!model) {{
                model = monaco.editor.createModel(value, language, uri);
            }} else {{
                if (model.getValue() !== value) model.setValue(value);
                if (model.getLanguageId && model.getLanguageId() !== language) {{
                    monaco.editor.setModelLanguage(model, language);
                }}
            }}
            return model;
        }}

        (async function bootstrap(){{
            try {{
                ensureThemeBridgeInstalled();
                await ensureMonacoReady();

                // 초기 테마는 반드시 'AppTheme' 기준으로
                applyThemeFromLocalStorage();

                if (!Array.isArray(window.__monacoEditors)) window.__monacoEditors = [];

                if (div._monacoEditor) {{
                    const ed = div._monacoEditor;
                    const uriStr = 'inmemory://model/' + editorId;
                    const model = getOrCreateModel(initialCode, initialLang, uriStr);
                    ed.setModel(model);
                    ed.updateOptions({{ readOnly: isReadOnly, padding: {{ top: 16, bottom: 16 }}, minimap: {{ enabled: false }}, scrollBeyondLastLine: false, lineNumbers: 'on' }});
                    ed.layout({{ width: Math.max(0, width-2), height: Math.max(0, height-2) }});
                    return;
                }}

                // 최초 생성
                div.innerHTML = '';
                const uriStr = 'inmemory://model/' + editorId;
                const model = getOrCreateModel(initialCode, initialLang, uriStr);

                const editor = window.monaco.editor.create(div, {{
                    model: model,
                    readOnly: isReadOnly,
                    padding: {{ top: 16, bottom: 16 }},
                    automaticLayout: true,
                    minimap: {{ enabled: false }},
                    scrollBeyondLastLine: false,
                    lineNumbers: 'on'
                }});

                // 에디터 등록 (전체 테마 일괄 적용 대상)
                window.__monacoEditors.push(editor);
                div._monacoEditor = editor;
                div._editorId = editorId;

                editor.onDidDispose(function(){{
                    const arr = window.__monacoEditors;
                    if (Array.isArray(arr)) {{
                        const i = arr.indexOf(editor);
                        if (i >= 0) arr.splice(i,1);
                    }}
                }});

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
    }})();";

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
