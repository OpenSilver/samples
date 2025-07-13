using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace OSFSample.Support.UI.Units
{
    public class IconButton : Button
    {
        public IconButton()
        {
            DefaultStyleKey = typeof(IconButton);
        }

        public Geometry IconData
        {
            get => (Geometry)GetValue(IconDataProperty);
            set => SetValue(IconDataProperty, value);
        }

        public static readonly DependencyProperty IconDataProperty =
            DependencyProperty.Register(
                nameof(IconData),
                typeof(Geometry),
                typeof(IconButton),
                new PropertyMetadata(null));
    }


    public abstract class CopyButtonBase : Button
    {
        public static readonly DependencyProperty IsCopiedProperty =
            DependencyProperty.Register(nameof(IsCopied), typeof(bool), typeof(CopyButtonBase),
                new PropertyMetadata(false, OnIsCopiedChanged));

        public bool IsCopied
        {
            get => (bool)GetValue(IsCopiedProperty);
            set => SetValue(IsCopiedProperty, value);
        }

        private static void OnIsCopiedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CopyButtonBase control)
            {
                // Dispatcher를 사용하여 UI 스레드에서 실행
                control.Dispatcher.BeginInvoke(new Action(() =>
                {
                    var state = control.IsCopied ? "Copied" : "Normal";
                    VisualStateManager.GoToState(control, state, true);
                }), DispatcherPriority.Render);
            }
        }

        public static readonly DependencyProperty FeedbackDurationProperty =
            DependencyProperty.Register(nameof(FeedbackDuration), typeof(TimeSpan), typeof(CopyButtonBase),
                new PropertyMetadata(TimeSpan.FromSeconds(1)));

        public TimeSpan FeedbackDuration
        {
            get => (TimeSpan)GetValue(FeedbackDurationProperty);
            set => SetValue(FeedbackDurationProperty, value);
        }

        protected CopyButtonBase()
        {
            // Command 실행 후 피드백을 위한 이벤트 핸들러
            Click += OnClickForFeedback;
            Loaded += (s, e) => VisualStateManager.GoToState(this, "Normal", false);
        }

        private async void OnClickForFeedback(object sender, RoutedEventArgs e)
        {
            // Command가 실행된 후 피드백 애니메이션만 담당
            await ShowCopiedFeedback();
        }

        // 외부에서 호출할 수 있는 피드백 메서드
        public async Task ShowCopiedFeedback()
        {
            IsCopied = true;

            var delay = FeedbackDuration.TotalMilliseconds < 100 ? 100 : FeedbackDuration.TotalMilliseconds;
            await Task.Delay((int)delay);

            IsCopied = false;
        }
    }

    public class CopyIconButton : CopyButtonBase
    {
        public CopyIconButton()
        {
            DefaultStyleKey = typeof(CopyIconButton);
        }
    }
}
