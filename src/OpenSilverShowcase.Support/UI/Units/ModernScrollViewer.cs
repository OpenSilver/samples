using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace OpenSilverShowcase.Support.UI.Units
{
    public class ModernScrollViewer : ScrollViewer
    {
        private DispatcherTimer _hideScrollBarTimer;
        private bool _isMouseOver = false;

        public ModernScrollViewer()
        {
            DefaultStyleKey = typeof(ModernScrollViewer);

            // 스크롤 이벤트 구독
            this.ScrollChanged += OnScrollChanged;
            this.MouseEnter += OnMouseEnter;
            this.MouseLeave += OnMouseLeave;

            // 타이머 초기화
            _hideScrollBarTimer = new DispatcherTimer();
            _hideScrollBarTimer.Interval = TimeSpan.FromSeconds(1.5);
            _hideScrollBarTimer.Tick += OnHideScrollBarTimer_Tick;
        }

        private void OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            // 스크롤 중이면 스크롤바 보이기
            VisualStateManager.GoToState(this, "MouseOver", true);

            // 타이머 리셋
            ResetHideTimer();
        }

        private void OnMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _isMouseOver = true;

            // 마우스 오버 시 스크롤바 보이기
            VisualStateManager.GoToState(this, "MouseOver", true);

            // 타이머 정지
            StopHideTimer();
        }

        private void OnMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _isMouseOver = false;

            // 마우스가 나가면 1.5초 후 숨기기 시작
            ResetHideTimer();
        }

        private void OnHideScrollBarTimer_Tick(object sender, EventArgs e)
        {
            // 마우스가 오버되어 있지 않으면 숨기기
            if (!_isMouseOver)
            {
                VisualStateManager.GoToState(this, "Normal", true);
            }

            StopHideTimer();
        }

        private void ResetHideTimer()
        {
            StopHideTimer();
            _hideScrollBarTimer.Start();
        }

        private void StopHideTimer()
        {
            if (_hideScrollBarTimer.IsEnabled)
            {
                _hideScrollBarTimer.Stop();
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // 초기 상태는 숨김
            VisualStateManager.GoToState(this, "Normal", false);
        }
    }
}
