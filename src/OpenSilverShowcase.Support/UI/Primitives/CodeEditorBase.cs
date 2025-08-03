using System;
using System.Windows;
using CSHTML5.Native.Html.Controls;

namespace OpenSilverShowcase.Support.UI.Primitives
{
    public class CodeEditorBase : HtmlPresenter
    {
        public static readonly DependencyProperty CodeProperty = DependencyProperty.Register(nameof(Code), typeof(string), typeof(CodeEditorBase), new PropertyMetadata("console.log('Hello World');", OnCodeChanged));
        public static readonly DependencyProperty ThemeProperty = DependencyProperty.Register(nameof(Theme), typeof(string), typeof(CodeEditorBase), new PropertyMetadata("vs-dark", OnThemeChanged));
        public static readonly DependencyProperty LanguageProperty = DependencyProperty.Register(nameof(Language), typeof(string), typeof(CodeEditorBase), new PropertyMetadata("javascript", OnLanguageChanged));
        public static readonly DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(CodeEditorBase), new PropertyMetadata(false, OnIsReadOnlyChanged));

        public string Code
        {
            get => (string)GetValue(CodeProperty);
            set => SetValue(CodeProperty, value);
        }

        public string Theme
        {
            get => (string)GetValue(ThemeProperty);
            set => SetValue(ThemeProperty, value);
        }

        public string Language
        {
            get => (string)GetValue(LanguageProperty);
            set => SetValue(LanguageProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public event EventHandler<string> CodeChanged;

        protected virtual void OnCodeChanged(string newCode)
        {
            CodeChanged?.Invoke(this, newCode);
        }

        protected virtual void UpdateCodeImplementation(string newCode) { }
        protected virtual void UpdateThemeImplementation(string newTheme) { }
        protected virtual void UpdateLanguageImplementation(string newLanguage) { }
        protected virtual void UpdateReadOnlyImplementation(bool isReadOnly) { }

        private static void OnCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CodeEditorBase editor)
            {
                editor.UpdateCodeImplementation(e.NewValue as string);
            }
        }

        private static void OnThemeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CodeEditorBase editor)
            {
                editor.UpdateThemeImplementation(e.NewValue as string);
            }
        }

        private static void OnLanguageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CodeEditorBase editor)
            {
                editor.UpdateLanguageImplementation(e.NewValue as string);
            }
        }

        private static void OnIsReadOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CodeEditorBase editor)
            {
                editor.UpdateReadOnlyImplementation((bool)e.NewValue);
            }
        }
    }
}