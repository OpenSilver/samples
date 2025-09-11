using System;
using System.Windows;
using System.Windows.Browser;
using OpenSilver;
using OpenSilverShowcase.Support.UI.Primitives;
using OpenSilverShowcase.Support.Local.Helpers;

namespace OpenSilverShowcase.Support.UI.Units
{
    public class MonacoEditor : CodeEditorBase
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
                var callbackName = $"MonacoCallback_{_editorId}";
                HtmlPage.RegisterScriptableObject(callbackName, this);
                Console.WriteLine($"[MonacoEditor] Registered scriptable object: {callbackName}");

                Interop.ExecuteJavaScriptVoid($@"
                    console.log('[MonacoEditor] Checking if callback is registered:', '{callbackName}');
                    if (window['{callbackName}']) {{
                        console.log('[MonacoEditor] Callback found in window:', typeof window['{callbackName}']);
                        console.log('[MonacoEditor] OnCodeChanged method:', typeof window['{callbackName}'].OnCodeChanged);
                    }} else {{
                        console.error('[MonacoEditor] Callback NOT found in window');
                    }}
                ");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[MonacoEditor] Error registering scriptable object: {ex.Message}");
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

            MonacoScriptHelper.LoadAndCreateEditor(_div, Code, Language, Theme, IsReadOnly, width, height, _editorId);
            _editorCreated = true;
        }

        [ScriptableMember]
        public void OnCodeChanged(string newCode)
        {
            Console.WriteLine($"[MonacoEditor] OnCodeChanged called with {newCode?.Length ?? 0} chars");

            if (!_isUpdatingFromCallback && !IsReadOnly)
            {
                Console.WriteLine($"[MonacoEditor] Processing code change - ReadOnly: {IsReadOnly}, Updating: {_isUpdatingFromCallback}");
                _isUpdatingFromCallback = true;

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        Code = newCode;
                        base.OnCodeChanged(newCode);
                        Console.WriteLine($"[MonacoEditor] Code updated successfully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[MonacoEditor] Error updating code: {ex.Message}");
                    }
                    finally
                    {
                        _isUpdatingFromCallback = false;
                    }
                }), System.Windows.Threading.DispatcherPriority.Normal);
            }
            else
            {
                Console.WriteLine($"[MonacoEditor] Code change ignored - ReadOnly: {IsReadOnly}, Updating: {_isUpdatingFromCallback}");
            }
        }

        protected override void UpdateCodeImplementation(string newCode)
        {
            if (_editorCreated && _div != null && newCode != null && !_isUpdatingFromCallback)
            {
                Console.WriteLine($"[MonacoEditor] Updating editor code programmatically: {newCode?.Length ?? 0} chars");
                MonacoScriptHelper.UpdateEditorCode(_div, newCode);
            }
        }

        protected override void UpdateReadOnlyImplementation(bool isReadOnly)
        {
            if (_editorCreated && _div != null)
            {
                Console.WriteLine($"[MonacoEditor] Updating ReadOnly to: {isReadOnly}");
                MonacoScriptHelper.UpdateEditorReadOnly(_div, isReadOnly, _editorId);
            }
        }

        protected override void UpdateLanguageImplementation(string language)
        {
            if (_editorCreated && _div != null && !string.IsNullOrEmpty(language))
            {
                Console.WriteLine($"[MonacoEditor] Updating language to: {language}");
                MonacoScriptHelper.UpdateEditorLanguage(_div, language);
            }
        }

        protected override void UpdateThemeImplementation(string theme)
        {
            if (_editorCreated && _div != null && !string.IsNullOrEmpty(theme))
            {
                Console.WriteLine($"[MonacoEditor] Updating theme to: {theme}");
                MonacoScriptHelper.UpdateEditorTheme(_div, theme);
            }
        }
    }
}