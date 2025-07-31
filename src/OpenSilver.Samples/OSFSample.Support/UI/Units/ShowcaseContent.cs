using Jamesnet.Foundation;
using OSFSample.Support.Local.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace OSFSample.Support.UI.Units
{
    public class ShowcaseContent : ContentControl, IView, IShowcaseContentSelector
    {
        private TabMenuBar _tabMenu;
        private StackPanel _cachedContent;
        private Dictionary<string, Type> _showcaseTypes;
        private Dictionary<string, ShowcaseItem> _loadedItems;
        private Border _menuPanelBorder;
        private MonacoEditor _monacoEditor;

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(ShowcaseContent), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(ShowcaseContent), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IsMenuPanelVisibleProperty =
            DependencyProperty.Register(
                nameof(IsMenuPanelVisible),
                typeof(bool),
                typeof(ShowcaseContent),
                new PropertyMetadata(false, OnIsMenuPanelVisibleChanged));

        public static readonly DependencyProperty DefaultSelectedItemNameProperty =
            DependencyProperty.Register(
                nameof(DefaultSelectedItemName),
                typeof(string),
                typeof(ShowcaseContent),
                new PropertyMetadata(string.Empty, OnDefaultSelectedItemNameChanged));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public bool IsMenuPanelVisible
        {
            get => (bool)GetValue(IsMenuPanelVisibleProperty);
            set => SetValue(IsMenuPanelVisibleProperty, value);
        }

        public string DefaultSelectedItemName
        {
            get => (string)GetValue(DefaultSelectedItemNameProperty);
            set => SetValue(DefaultSelectedItemNameProperty, value);
        }

        public ShowcaseContent()
        {
            DefaultStyleKey = typeof(ShowcaseContent);
            Name = $"OSF_DEMO_{GetHashCode()}";
            Tag = GetUserControlAssemblyVersion();
            _showcaseTypes = new Dictionary<string, Type>();
            _loadedItems = new Dictionary<string, ShowcaseItem>();
            LoadShowcaseTypes();
        }

        private static void OnIsMenuPanelVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShowcaseContent control)
            {
                control.UpdateMenuPanelVisibility();
            }
        }

        private static async void OnDefaultSelectedItemNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ShowcaseContent control && control._tabMenu != null)
            {
                var name = e.NewValue as string;
                if (!string.IsNullOrWhiteSpace(name))
                {
                    await control.SelectShowcaseItemByNameAsync(name);
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _tabMenu = GetTemplateChild("TabMenu") as TabMenuBar;
            _menuPanelBorder = GetTemplateChild("MenuPanelBorder") as Border;
            UpdateMenuPanelVisibility();

            if (_tabMenu != null)
            {
                _tabMenu.MenuItemSelected += TabMenu_MenuItemSelected;
                UpdateMenuItems();

                if (!string.IsNullOrWhiteSpace(DefaultSelectedItemName))
                {
                    _ = SelectShowcaseItemByNameAsync(DefaultSelectedItemName);
                }
            }
        }

        private void UpdateMenuPanelVisibility()
        {
            if (_menuPanelBorder == null)
                return;

            if (IsMenuPanelVisible)
            {
                _menuPanelBorder.Visibility = Visibility.Visible;
                _menuPanelBorder.Opacity = 1;
                _menuPanelBorder.Height = double.NaN;
            }
            else
            {
                _menuPanelBorder.Visibility = Visibility.Collapsed;
                _menuPanelBorder.Opacity = 0;
                _menuPanelBorder.Height = 0;
            }
        }

        private void LoadShowcaseTypes()
        {
            var containerPanel = new StackPanel();
            _cachedContent = containerPanel;

            var showcaseTypes = FindShowcaseItemTypes();

            foreach (var type in showcaseTypes)
            {
                try
                {
                    var tempInstance = (ShowcaseItem)Activator.CreateInstance(type);
                    _showcaseTypes[tempInstance.Title] = type;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error getting type info {type.Name}: {ex.Message}");
                }
            }

            this.Content = containerPanel;
        }

        private ShowcaseItem LoadShowcaseItem(Type type)
        {
            var derivedType = this.GetType();
            var sourceAsm = derivedType.Assembly;
            var instance = (ShowcaseItem)Activator.CreateInstance(type);

            string ns = type.Namespace;
            string[] parts = ns?.Split('.') ?? Array.Empty<string>();
            string projectName = (parts.Length >= 2) ? $"{parts[0]}.{parts[1]}" : "Unknown";

            foreach (var source in instance.CodeSources ?? new ObservableCollection<CodeSource>())
            {
                source.ProjectName = projectName;

                var embeddedPath = EmbeddedUriResolver.ToEmbeddedPath(source.Source, sourceAsm);
                string? codeText = EmbeddedCodeLoader.Load(embeddedPath, sourceAsm);
                if (!string.IsNullOrWhiteSpace(codeText))
                {
                    try
                    {
                        source.Code = EmbeddedCodeLoader.ExtractDemoContentOnly(codeText);
                    }
                    catch
                    {
                        source.Code = codeText;
                    }
                }
            }

            instance.SelectedCodeTab = instance.CodeSources.FirstOrDefault();
            return instance;
        }

        private List<Type> FindShowcaseItemTypes()
        {
            Type derivedType = this.GetType();
            Assembly assembly = derivedType.Assembly;
            string namespaceFilter = derivedType.Namespace;

            return assembly.GetTypes()
                .Where(t => t.IsClass &&
                           !t.IsAbstract &&
                           t.IsSubclassOf(typeof(ShowcaseItem)) &&
                           t.Namespace != null &&
                           t.Namespace.StartsWith(namespaceFilter))
                .ToList();
        }

        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            if (newContent is StackPanel cards)
            {
                _cachedContent = cards;
                UpdateMenuItems();
            }
        }

        private string GetUserControlAssemblyVersion()
        {
            try
            {
                Assembly assembly = typeof(UserControl).Assembly;
                AssemblyName assemblyNameInfo = assembly.GetName();
                Version version = assemblyNameInfo.Version;
                return version.ToString();
            }
            catch (Exception ex)
            {
                return "Unknown";
            }
        }

        private void UpdateMenuItems()
        {
            if (_tabMenu == null || _showcaseTypes == null)
                return;

            try
            {
                var sortedTypes = _showcaseTypes.OrderBy(kvp =>
                {
                    try
                    {
                        var tempInstance = (ShowcaseItem)Activator.CreateInstance(kvp.Value);
                        return tempInstance.Order;
                    }
                    catch
                    {
                        return int.MaxValue;
                    }
                }).ToList();

                List<string> menuItems = new();
                foreach (var kvp in sortedTypes)
                {
                    menuItems.Add(kvp.Key);
                }

                _tabMenu.MenuItems = menuItems;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating menu items: {ex.Message}");
            }
        }

        private async void TabMenu_MenuItemSelected(object sender, MenuItemSelectedEventArgs e)
        {
            if (_cachedContent == null || _tabMenu == null)
                return;

            try
            {
                await Task.Delay(50);

                string selectedItem = _tabMenu.MenuItems[e.SelectedIndex];

                var fadeOutTasks = new List<Task>();
                foreach (Control item in _cachedContent.Children.Cast<Control>().ToList())
                {
                    if (item is ShowcaseItem card)
                    {
                        await Task.Delay(30);
                        fadeOutTasks.Add(FadeOutAndCollapse(card));
                    }
                }

                await Task.WhenAll(fadeOutTasks);
                _cachedContent.Children.Clear();

                await Task.Delay(100);

                if (_showcaseTypes.ContainsKey(selectedItem))
                {
                    ShowcaseItem selectedCard;

                    if (_loadedItems.ContainsKey(selectedItem))
                    {
                        selectedCard = _loadedItems[selectedItem];
                    }
                    else
                    {
                        selectedCard = LoadShowcaseItem(_showcaseTypes[selectedItem]);
                        _loadedItems[selectedItem] = selectedCard;
                    }

                    selectedCard.Opacity = 0;
                    selectedCard.Visibility = Visibility.Visible;
                    _cachedContent.Children.Add(selectedCard);

                    await Task.Delay(50);
                    await FadeIn(selectedCard);
                }

                OnSelectedItemChanged?.Invoke(this, new MenuItemSelectedEventArgs
                {
                    SelectedIndex = e.SelectedIndex,
                    SelectedItem = selectedItem
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in menu selection: {ex.Message}");
            }
        }

        private Task FadeOutAndCollapse(FrameworkElement element)
        {
            var tcs = new TaskCompletionSource<bool>();

            Storyboard storyboard = new Storyboard();

            DoubleAnimation fadeOutAnimation = new DoubleAnimation
            {
                From = element.Opacity,
                To = 0.0,
                Duration = new Duration(TimeSpan.FromMilliseconds(280)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard.SetTarget(fadeOutAnimation, element);
            Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));
            storyboard.Children.Add(fadeOutAnimation);

            storyboard.Completed += async (s, e) =>
            {
                await Task.Delay(50);
                element.Visibility = Visibility.Collapsed;
                tcs.SetResult(true);
            };

            storyboard.Begin();

            return tcs.Task;
        }

        private Task FadeIn(FrameworkElement element)
        {
            var tcs = new TaskCompletionSource<bool>();

            if (element.Visibility == Visibility.Visible && Math.Abs(element.Opacity - 1.0) < 0.01)
            {
                tcs.SetResult(true);
                return tcs.Task;
            }

            element.Visibility = Visibility.Visible;

            Storyboard storyboard = new Storyboard();

            DoubleAnimation fadeInAnimation = new DoubleAnimation
            {
                From = 0.0,
                To = 1.0,
                Duration = new Duration(TimeSpan.FromMilliseconds(320)),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };

            Storyboard.SetTarget(fadeInAnimation, element);
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));
            storyboard.Children.Add(fadeInAnimation);

            storyboard.Completed += (s, e) =>
            {
                tcs.SetResult(true);
            };

            storyboard.Begin();

            return tcs.Task;
        }

        public async Task SelectShowcaseItemByNameAsync(string itemName)
        {
            if (_tabMenu == null || _showcaseTypes == null || _tabMenu.MenuItems.Count == 0)
                return;

            int index = 0;

            if (!string.IsNullOrEmpty(itemName))
            {
                string Normalize(string s) => new string(s.Where(c => !char.IsWhiteSpace(c)).ToArray()).ToLower();

                var matchedItem = _tabMenu.MenuItems
                    .Select((item, idx) => new { Item = item, Index = idx })
                    .FirstOrDefault(x => Normalize(x.Item) == Normalize(itemName));

                if (matchedItem != null)
                {
                    index = matchedItem.Index;
                }
            }

            if (index != _tabMenu.SelectedIndex)
            {
                _tabMenu.SelectedIndex = index;
                await SimulateTabMenuSelection(index);
            }
        }

        private async Task SimulateTabMenuSelection(int index)
        {
            if (_tabMenu != null && index >= 0 && index < _tabMenu.MenuItems.Count)
            {
                TabMenu_MenuItemSelected(this, new MenuItemSelectedEventArgs
                {
                    SelectedIndex = index,
                    SelectedItem = _tabMenu.MenuItems[index]
                });
            }
            await Task.CompletedTask;
        }

        internal void ShareItem(CodeSource selectedCodeTab)
        {
            Share?.Invoke();
        }

        public event Action Share;
        public event Action<string, string> FullScreenCode;
        public event EventHandler<MenuItemSelectedEventArgs> OnSelectedItemChanged;

        internal void CodeItem(CodeSource obj)
        {
            FullScreenCode?.Invoke(obj.Code, obj.Language);
        }

        public IReadOnlyList<string> GetShowcaseItemNames()
        {
            if (_showcaseTypes == null)
                return Array.Empty<string>();

            var sortedTypes = _showcaseTypes.OrderBy(kvp =>
            {
                try
                {
                    var tempInstance = (ShowcaseItem)Activator.CreateInstance(kvp.Value);
                    return tempInstance.Order;
                }
                catch
                {
                    return int.MaxValue;
                }
            }).ToList();

            return sortedTypes.Select(kvp => kvp.Key).ToList();
        }
    }

    public interface IShowcaseContentSelector
    {
        Task SelectShowcaseItemByNameAsync(string itemName);
        string Description { get; set; }
        string Title { get; set; }
        event Action Share;
        event Action<string, string> FullScreenCode;
        IReadOnlyList<string> GetShowcaseItemNames();
    }
}
