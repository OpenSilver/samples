using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace OpenSilverShowcase.Support.UI.Units
{
    public class FullScreenButton : ContentControl
    {
        // 스크롤 감지를 위한 필드들
        private double _lastKnownScrollOffset = 0;
        private const double ScrollThreshold = 1.0;

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(FullScreenButton), new PropertyMetadata(null));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public FullScreenButton()
        {
            DefaultStyleKey = typeof(FullScreenButton);
            Console.WriteLine("=== FullScreenButton 생성자 ===");
            Loaded += OnLoaded;
            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseLeftButtonUp += OnMouseLeftButtonUp;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("=== FullScreenButton OnLoaded 시작 ===");
            Console.WriteLine($"FullScreenButton ID: {this.GetHashCode()}");
            Console.WriteLine($"DataContext: {DataContext}");

            // 초기 스크롤 위치 기록
            var scrollViewer = FindAncestorScrollViewer(this);
            Console.WriteLine($"ScrollViewer 찾기 결과: {scrollViewer != null}");

            if (scrollViewer != null)
            {
                _lastKnownScrollOffset = scrollViewer.VerticalOffset;
                Console.WriteLine($"초기 스크롤 위치 기록 (Vertical): {_lastKnownScrollOffset}");
            }

            Console.WriteLine("=== FullScreenButton OnLoaded 완료 ===\n");
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            Console.WriteLine($"=== FullScreenButton OnMouseEnter - ID: {this.GetHashCode()} ===");

            // 스크롤 위치 업데이트
            var scrollViewer = FindAncestorScrollViewer(this);
            if (scrollViewer != null)
            {
                var oldOffset = _lastKnownScrollOffset;
                _lastKnownScrollOffset = scrollViewer.VerticalOffset;
                Console.WriteLine($"스크롤 위치 업데이트 (Vertical): {oldOffset} -> {_lastKnownScrollOffset}");
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            Console.WriteLine($"=== FullScreenButton OnMouseLeave - ID: {this.GetHashCode()} ===");
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine($"=== FullScreenButton OnMouseLeftButtonDown - ID: {this.GetHashCode()} ===");

            // 터치/클릭 시작 시점의 스크롤 위치 기록
            var scrollViewer = FindAncestorScrollViewer(this);
            if (scrollViewer != null)
            {
                var oldOffset = _lastKnownScrollOffset;
                _lastKnownScrollOffset = scrollViewer.VerticalOffset;
                Console.WriteLine($"클릭 다운 시점 스크롤 위치 기록 (Vertical): {oldOffset} -> {_lastKnownScrollOffset}");
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine($"\n=== FullScreenButton OnMouseLeftButtonUp 시작 - ID: {this.GetHashCode()} ===");

            var command = Command;
            var parameter = DataContext;

            Console.WriteLine($"DataContext: {parameter}");
            Console.WriteLine($"Command 존재: {command != null}");

            // 스크롤 감지 로직
            var scrollViewer = FindAncestorScrollViewer(this);
            bool hasScrolled = false;

            Console.WriteLine($"ScrollViewer 찾기 결과: {scrollViewer != null}");

            if (scrollViewer != null)
            {
                double currentOffset = scrollViewer.VerticalOffset;
                double scrollDifference = Math.Abs(currentOffset - _lastKnownScrollOffset);

                Console.WriteLine($"이전 스크롤 위치 (Vertical): {_lastKnownScrollOffset}");
                Console.WriteLine($"현재 스크롤 위치 (Vertical): {currentOffset}");
                Console.WriteLine($"스크롤 차이: {scrollDifference}");
                Console.WriteLine($"ScrollThreshold: {ScrollThreshold}");

                hasScrolled = scrollDifference >= ScrollThreshold;

                // 현재 위치를 업데이트
                _lastKnownScrollOffset = currentOffset;
                Console.WriteLine($"스크롤 위치 업데이트 (Vertical): {currentOffset}");
            }

            Console.WriteLine($"*** 스크롤 감지 결과: {hasScrolled} ***");

            // 스크롤하지 않았을 때만 커맨드 실행
            if (!hasScrolled)
            {
                Console.WriteLine(">>> 스크롤하지 않음 - 커맨드 실행 시도 <<<");

                if (command != null)
                {
                    Console.WriteLine("커맨드 실행!");
                    command?.Execute(parameter);
                }
                else
                {
                    Console.WriteLine("커맨드가 null입니다!");
                }
            }
            else
            {
                Console.WriteLine(">>> 스크롤함 - 커맨드 실행 안 함 <<<");
            }

            Console.WriteLine($"=== FullScreenButton OnMouseLeftButtonUp 완료 - ID: {this.GetHashCode()} ===\n");
        }

        /// <summary>
        /// 부모 요소 중에서 ScrollViewer를 찾는 메서드
        /// </summary>
        private ScrollViewer FindAncestorScrollViewer(DependencyObject element)
        {
            Console.WriteLine("FindAncestorScrollViewer 시작");
            while (element != null)
            {
                Console.WriteLine($"현재 요소: {element.GetType().Name}");
                if (element is ScrollViewer viewer)
                {
                    Console.WriteLine("ScrollViewer 찾음!");
                    return viewer;
                }

                element = VisualTreeHelper.GetParent(element);
            }
            Console.WriteLine("ScrollViewer 못 찾음");
            return null;
        }
    }
}