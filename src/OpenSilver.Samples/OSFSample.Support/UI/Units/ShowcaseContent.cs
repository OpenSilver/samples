using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using OSFSample.Support.Local.Utils;

namespace OSFSample.Support.UI.Units;


public class ShowcaseContent : ContentControl
{
    private TabMenuBar _tabMenu;
    private StackPanel _cachedContent;
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(nameof(Title), typeof(string), typeof(ShowcaseContent), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(ShowcaseContent), new PropertyMetadata(string.Empty));

    public ShowcaseContent()
    {
        DefaultStyleKey = typeof(ShowcaseContent);
        Name = $"OSF_DEMO_{GetHashCode()}";
        Tag = GetUserControlAssemblyVersion();
        LoadShowcaseControls();
    }

    private void LoadShowcaseControls()
    {
        var containerPanel = new StackPanel();
        // 파생 클래스의 어셈블리 가져오기
        var derivedType = this.GetType();
        var sourceAsm = derivedType.Assembly;

        var showcaseTypes = FindShowcaseItemTypes();
        var ShowcaseItems = new List<ShowcaseItem>();

        foreach (var type in showcaseTypes)
        {
            try
            {
                var instance = (ShowcaseItem)Activator.CreateInstance(type);
                foreach (var source in instance.CodeSources ?? new ObservableCollection<CodeSource>())
                {
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
                            source.Code = codeText; // fallback
                        }
                    }
                }
                instance.SelectedCodeTab = instance.CodeSources.FirstOrDefault();
                ShowcaseItems.Add(instance);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating {type.Name}: {ex.Message}");
            }
        }

        foreach (var card in ShowcaseItems.OrderBy(c => c.Order))
        {
            containerPanel.Children.Add(card);
        }
        this.Content = containerPanel;
    }

    private List<Type> FindShowcaseItemTypes()
    {
        // 파생 클래스의 타입 가져오기
        Type derivedType = this.GetType();

        // 파생 클래스의 어셈블리와 네임스페이스 가져오기
        Assembly assembly = derivedType.Assembly;
        string namespaceFilter = derivedType.Namespace;

        // 디버그 정보 출력
        System.Diagnostics.Debug.WriteLine($"Looking for ShowcaseItem in namespace: {namespaceFilter}");

        return assembly.GetTypes()
            .Where(t => t.IsClass &&
                       !t.IsAbstract &&
                       t.IsSubclassOf(typeof(ShowcaseItem)) &&
                       t.Namespace != null &&
                       t.Namespace.StartsWith(namespaceFilter))
            .ToList();
    }

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
            MessageBox.Show($"Error getting assembly version: {ex.Message}");
            return "Unknown";
        }
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        _tabMenu = GetTemplateChild("TabMenu") as TabMenuBar;

        if (_tabMenu != null)
        {
            _tabMenu.MenuItemSelected += TabMenu_MenuItemSelected;
            UpdateMenuItems();
        }
    }

    private void UpdateMenuItems()
    {
        if (_tabMenu == null || _cachedContent == null)
            return;

        try
        {
            // 카드 수집 + 정렬
            List<ShowcaseItem> cards = new();
            foreach (UIElement child in _cachedContent.Children)
            {
                if (child is ShowcaseItem card)
                {
                    cards.Add(card);
                }
            }

            // Order 기준 정렬
            cards.Sort((a, b) => a.Order.CompareTo(b.Order));

            // StackPanel 정렬 반영
            _cachedContent.Children.Clear();
            foreach (var card in cards)
            {
                _cachedContent.Children.Add(card);
            }

            // 메뉴 구성
            List<string> menuItems = new() { "All View" };
            foreach (var card in cards)
            {
                menuItems.Add(card.Title);
            }

            _tabMenu.MenuItems = menuItems;
            _tabMenu.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating menu items: {ex.Message}");
        }
    }


    private async void TabMenu_MenuItemSelected(object sender, MenuItemSelectedEventArgs e)
    {
        if (_cachedContent == null)
            return;
        try
        {
            // 초기 지연 추가 (원래 코드에 있던 200ms 지연)
            await Task.Delay(200);

            string selectedItem = _tabMenu.MenuItems[e.SelectedIndex];
            bool isAllView = e.SelectedIndex == 0;

            // 1단계: 현재 보이는 카드와 표시할 카드 파악
            List<ShowcaseItem> visibleCards = new List<ShowcaseItem>();
            List<ShowcaseItem> cardsToShow = new List<ShowcaseItem>();

            foreach (Control item in _cachedContent.Children)
            {
                if (item is ShowcaseItem card)
                {
                    // 현재 보이는 카드 수집
                    if (card.Visibility == Visibility.Visible)
                    {
                        visibleCards.Add(card);
                    }

                    // 표시해야 할 카드 결정
                    if (isAllView || card.Title == selectedItem)
                    {
                        if (card.Visibility != Visibility.Visible)
                        {
                            cardsToShow.Add(card);
                        }
                    }
                }
            }

            // 2단계: 페이드 아웃 (현재 보이는 카드 중 앞으로 표시되지 않을 카드)
            var fadeOutTasks = new List<Task>();
            foreach (var card in visibleCards)
            {
                if (!(isAllView || card.Title == selectedItem))
                {
                    // 시간차를 줘서 살짝 순차적으로 페이드 아웃
                    await Task.Delay(30); // 각 카드마다 30ms 지연
                    fadeOutTasks.Add(FadeOutAndCollapse(card));
                }
            }

            // 페이드 아웃이 모두 완료될 때까지 대기
            await Task.WhenAll(fadeOutTasks);

            // 약간의 지연 후 페이드 인 시작
            await Task.Delay(100);

            // 3단계: 페이드 인 준비 (새로 표시할 카드들)
            foreach (var card in cardsToShow)
            {
                card.Opacity = 0;
                card.Visibility = Visibility.Visible;
            }

            // 4단계: 페이드 인 (순차적으로)
            foreach (var card in cardsToShow)
            {
                await Task.Delay(50); // 각 카드마다 50ms 지연
                await FadeIn(card);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error in menu selection: {ex.Message}");
        }
    }

    // 페이드 아웃 후 Collapse 처리하는 메서드 - 이 부분만 수정했습니다
    private Task FadeOutAndCollapse(FrameworkElement element)
    {
        var tcs = new TaskCompletionSource<bool>();

        // Storyboard 생성
        Storyboard storyboard = new Storyboard();

        // DoubleAnimation 생성 (Opacity 1 -> 0)
        DoubleAnimation fadeOutAnimation = new DoubleAnimation
        {
            From = element.Opacity,
            To = 0.0,
            Duration = new Duration(TimeSpan.FromMilliseconds(280))
        };

        // Easing 함수 추가 (부드러운 효과)
        QuadraticEase easingFunction = new QuadraticEase();
        easingFunction.EasingMode = EasingMode.EaseInOut;
        fadeOutAnimation.EasingFunction = easingFunction;

        // Storyboard에 애니메이션 추가
        Storyboard.SetTarget(fadeOutAnimation, element);
        Storyboard.SetTargetProperty(fadeOutAnimation, new PropertyPath("Opacity"));
        storyboard.Children.Add(fadeOutAnimation);

        // 애니메이션 완료 이벤트 처리
        storyboard.Completed += async (s, e) =>
        {
            // 완전히 투명해진 후 약간의 지연을 주고 Visibility 변경
            await Task.Delay(50); // 페이드 아웃 후 Visibility 변경 전 50ms 지연 추가

            // 애니메이션 완료 후 Visibility를 Collapsed로 변경
            element.Visibility = Visibility.Collapsed;
            tcs.SetResult(true);
        };

        // 애니메이션 시작
        storyboard.Begin();

        return tcs.Task;
    }

    // 페이드 인 애니메이션 메서드 - 수정 없음
    private Task FadeIn(FrameworkElement element)
    {
        var tcs = new TaskCompletionSource<bool>();

        // 이미 Visible 상태이고 Opacity가 이미 1인 경우 즉시 완료 처리
        if (element.Visibility == Visibility.Visible && Math.Abs(element.Opacity - 1.0) < 0.01)
        {
            tcs.SetResult(true);
            return tcs.Task;
        }

        // 우선 Visible로 설정
        element.Visibility = Visibility.Visible;

        // Storyboard 생성
        Storyboard storyboard = new Storyboard();

        // DoubleAnimation 생성 (Opacity 0 -> 1)
        DoubleAnimation fadeInAnimation = new DoubleAnimation
        {
            From = 0.0,
            To = 1.0,
            Duration = new Duration(TimeSpan.FromMilliseconds(320))
        };

        // Easing 함수 추가 (부드러운 효과)
        QuadraticEase easingFunction = new QuadraticEase();
        easingFunction.EasingMode = EasingMode.EaseOut;
        fadeInAnimation.EasingFunction = easingFunction;

        // Storyboard에 애니메이션 추가
        Storyboard.SetTarget(fadeInAnimation, element);
        Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath("Opacity"));
        storyboard.Children.Add(fadeInAnimation);

        // 애니메이션 완료 이벤트 처리
        storyboard.Completed += (s, e) =>
        {
            tcs.SetResult(true);
        };

        // 애니메이션 시작
        storyboard.Begin();

        return tcs.Task;
    }
}