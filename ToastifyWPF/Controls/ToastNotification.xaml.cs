using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

using System.Diagnostics;
using System.Windows.Media;

using ToastifyWPF.Models;
using ToastifyWPF.Enums;

namespace ToastifyWPF.Controls
{
    /// <summary>
    /// ToastNotification là một UserControl dùng để hiển thị thông báo dạng "toast" với hiệu ứng hiển thị, tự động ẩn sau một khoảng thời gian.
    /// Hỗ trợ animation vào-ra, fill animation, và có thể tuỳ biến theme.
    /// </summary>
    public partial class ToastNotification : UserControl
    {
        readonly TimeSpan DEFAULT_DISPLAY_DURATION = TimeSpan.FromSeconds(3);
        #region Dependency Properties (Các thuộc tính có thể binding)

        /// <summary>
        /// Thời gian thông báo được hiển thị trước khi tự động ẩn (mặc định: 3 giây)
        /// </summary>
        public TimeSpan DisplayDuration
        {
            get => (TimeSpan)GetValue(DisplayDurationProperty);
            set => SetValue(DisplayDurationProperty, value);
        }

        public static readonly DependencyProperty DisplayDurationProperty =
            DependencyProperty.Register(nameof(DisplayDuration), typeof(TimeSpan), typeof(ToastNotification),
                new PropertyMetadata(TimeSpan.FromSeconds(3)));

        /// <summary>
        /// Thời lượng cho các animation vào-ra (mặc định: 300ms)
        /// </summary>
        public TimeSpan AnimationDuration
        {
            get => (TimeSpan)GetValue(AnimationDurationProperty);
            set => SetValue(AnimationDurationProperty, value);
        }

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register(nameof(AnimationDuration), typeof(TimeSpan), typeof(ToastNotification),
                new PropertyMetadata(TimeSpan.FromMilliseconds(300)));

        /// <summary>
        /// Bo góc của thông báo
        /// </summary>
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius), typeof(CornerRadius), typeof(ToastNotification),
                new PropertyMetadata(new CornerRadius(6)));

        /// <summary>
        /// Context định nghĩa animation storyboard cho transition vào-ra
        /// </summary>
        public StoryBoardContext? StoryBoardContext
        {
            get => (StoryBoardContext)GetValue(StoryBoardContextProperty);
            set => SetValue(StoryBoardContextProperty, value);
        }

        public static readonly DependencyProperty StoryBoardContextProperty =
            DependencyProperty.Register(nameof(StoryBoardContext), typeof(StoryBoardContext), typeof(ToastNotification),
                new PropertyMetadata(null));

        /// <summary>
        /// Theme hiện tại của thông báo (màu sắc, icon, kiểu nền...)
        /// </summary>
        public ToastNotificationTheme ToastNotificationTheme
        {
            get => (ToastNotificationTheme)GetValue(ToastNotificationThemeProperty);
            set => SetValue(ToastNotificationThemeProperty, value);
        }

        public static readonly DependencyProperty ToastNotificationThemeProperty =
            DependencyProperty.Register(nameof(ToastNotificationTheme), typeof(ToastNotificationTheme), typeof(ToastNotification),
                new PropertyMetadata(default));

        /// <summary>
        /// Dữ liệu nội dung của thông báo (text, loại thông báo, v.v.)
        /// </summary>
        public ToastNotificationData? Data
        {
            get => (ToastNotificationData?)GetValue(DataProperty);
            set => SetValue(DataProperty, value);
        }

        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register(nameof(Data), typeof(ToastNotificationData), typeof(ToastNotification),
                new PropertyMetadata(default));
        #endregion

        #region Sự kiện

        /// <summary>
        /// Sự kiện được raise khi toast bắt đầu hiển thị
        /// </summary>
        public event RoutedEventHandler OnOpen
        {
            add => AddHandler(OnOpenEvent, value);
            remove => RemoveHandler(OnOpenEvent, value);
        }

        public static readonly RoutedEvent OnOpenEvent = EventManager.RegisterRoutedEvent(
            nameof(OnOpen), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToastNotification));

        /// <summary>
        /// Sự kiện được raise khi toast kết thúc (ẩn đi)
        /// </summary>
        public event RoutedEventHandler OnClose
        {
            add => AddHandler(OnCloseEvent, value);
            remove => RemoveHandler(OnCloseEvent, value);
        }

        public static readonly RoutedEvent OnCloseEvent = EventManager.RegisterRoutedEvent(
            nameof(OnClose), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ToastNotification));
        #endregion

        #region Biến cục bộ

        private const int TIMER_INTERVAL_MILLISECONDS = 100;
        private DispatcherTimer timer = null!;
        private readonly Stopwatch elapsed = new();
        private int lastRemainingSeconds = -1;
        private bool isShown;
        private bool isHovered;
        private bool isClickPaused;
        private bool isPaused;
        private bool transitionOutStarted;
        private bool finishActionInvoked;

        private Storyboard fillOutStoryBoard = null!;
        private Storyboard fillInStoryBoard = null!;
        private Storyboard forwardStoryBoard = null!;
        private Storyboard backwardStoryBoard = null!;

        private AnimationTimeline fillOutAnimation = null!;
        private AnimationTimeline fillInAnimation = null!;
        #endregion

        #region Constructor
        public ToastNotification(ToastNotificationData toastNotificationData)
        {
            InitializeComponent();
            Prepare(toastNotificationData);
        }

        public ToastNotification()
        {
            InitializeComponent();
            Prepare();
        }
        #endregion

        #region Chuẩn bị khởi tạo
        private void Prepare(ToastNotificationData? toastNotificationData = null)
        {
            Data = toastNotificationData;
            SetupFillOutStoryboard();
            SetupFillInStoryboard();
            SetupTransitions();
            InitTimer();
        }

        private void ApplyTheme(ToastThemeEnum theme, ToastTypeEnum type)
        {
            ToastNotificationTheme = ToastNotificationTheme.InitTheme(theme, type);
        }

        private void InitTimer()
        {
            timer = new DispatcherTimer(DispatcherPriority.Background, Dispatcher)
            {
                Interval = TimeSpan.FromMilliseconds(TIMER_INTERVAL_MILLISECONDS)
            };

            timer.Tick += (_, _) => UpdateCountdown();
        }
        #endregion

        #region Tương tác chuột

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            Reset();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            UpdatePauseState();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            UpdatePauseState();
        }

        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonUp(e);

            if (FindVisualParent<Button>(e.OriginalSource as DependencyObject) != null)
                return;

            if (Data?.CloseOnClick == true)
            {
                Reset();
                e.Handled = true;
                return;
            }

            if (Data?.PauseOnClick == true)
            {
                isClickPaused = !isClickPaused;
                UpdatePauseState();
                e.Handled = true;
            }
        }

        private void UpdatePauseState()
        {
            var shouldPause = isShown &&
                ((isHovered && Data?.PauseOnHover == true) || isClickPaused);

            if (shouldPause == isPaused)
                return;

            isPaused = shouldPause;
            if (isPaused)
            {
                elapsed.Stop();
                timer.Stop();
                PauseFillOutAnimation();
                return;
            }

            elapsed.Start();
            timer.Start();
            ResumeFillOutAnimtion();
            UpdateCountdown();
        }

        private void UpdateCountdown()
        {
            if (!isShown || isPaused)
                return;

            var elapsedTime = elapsed.Elapsed;
            var remainingSeconds = Math.Max(0,
                (int)Math.Ceiling((DisplayDuration - elapsedTime).TotalSeconds));

            if (remainingSeconds != lastRemainingSeconds)
            {
                lastRemainingSeconds = remainingSeconds;
                UpdateMessageOnTick(remainingSeconds);
            }

            if (elapsedTime >= DisplayDuration)
                Hide();
        }

        private static T? FindVisualParent<T>(DependencyObject? child)
            where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T parent)
                    return parent;

                child = VisualTreeHelper.GetParent(child);
            }

            return null;
        }

        #endregion

        #region Public API

        /// <summary>
        /// Hiển thị toast với dữ liệu mới
        /// </summary>
        public void Show(ToastNotificationData toastNotificationData)
        {
            ArgumentNullException.ThrowIfNull(toastNotificationData);

            Data = toastNotificationData;

            Show();
        }

        /// <summary>
        /// Hiển thị toast (nếu đã có sẵn Data)
        /// </summary>
        public void Show()
        {
            if (Data == null)
                return;

            DisplayDuration = NormalizeDuration(Data.Duration);
            SetupFillOutStoryboard();

            timer.Stop();
            lastRemainingSeconds = -1;
            isShown = true;
            isHovered = false;
            isClickPaused = false;
            isPaused = false;
            transitionOutStarted = false;
            finishActionInvoked = false;

            ApplyTheme(ToastThemeEnum.Light, Data.Type);

            StartTransitionIn();
            elapsed.Restart();
            StartFillOutAnimation();
            timer.Start();
            UpdateCountdown();
        }

        /// <summary>
        /// Ẩn toast với animation
        /// </summary>
        public void Hide()
        {
            if (!isShown || transitionOutStarted)
                return;

            timer.Stop();
            elapsed.Stop();
            isShown = false;
            isPaused = false;
            transitionOutStarted = true;
            // Keep the last animated value during the exit transition. Stop() would
            // remove the animation and restore ScaleX=1, making the bar refill.
            fillOutStoryBoard?.Pause(ProgressBar);
            StartTransitionOut();
        }

        /// <summary>
        /// Reset trạng thái hiện tại
        /// </summary>
        public void Reset()
        {
            Hide();
            // Chạy hàm sau khi hoàn tất animation
            RunFinishActionOnStop();
        }

        /// <summary>
        /// Reset ngay toast đã được trả về pool, không chạy animation trên control đã tháo khỏi cây UI.
        /// </summary>
        internal void ResetForPool()
        {
            timer.Stop();
            elapsed.Reset();
            fillOutStoryBoard?.Stop(ProgressBar);
            isShown = false;
            isHovered = false;
            isClickPaused = false;
            isPaused = false;
            transitionOutStarted = true;
            backwardStoryBoard.Completed -= OnTransitionOutCompleted;
            ProgressScale.ScaleX = 1;
            Root.Opacity = 1;
            SlideTransform.X = 100;
            RunFinishActionOnStop();
        }

        /// <summary>
        /// Đóng và raise sự kiện `OnClose`
        /// </summary>
        public void Close()
        {
            Reset();
        }

        #endregion

        #region Chạy các hàm tùy chọn
        #region Cập nhật message
        private void UpdateMessageOnTick(int remainingSeconds)
        {
            if (Data?.UpdateMessageAction == null)
                return;

            Data.Message = Data.UpdateMessageAction(Data.Message, remainingSeconds) ?? "";
        }
        #endregion Cập nhật message

        private void RunFinishActionOnStop()
        {
            if (finishActionInvoked)
                return;

            finishActionInvoked = true;
            Data?.FinishAction?.Invoke();
        }
        #endregion Chạy các hàm tùy chọn

        private TimeSpan NormalizeDuration(TimeSpan? duration)
        {
            return duration is { } value && value > TimeSpan.Zero
                ? value
                : DEFAULT_DISPLAY_DURATION;
        }

        #region Animation tiến trình (progress bar)

        public void StartFillOutAnimation()
        {
            ProgressScale.ScaleX = 1;
            Storyboard.SetTarget(fillOutAnimation, ProgressBar);
            Storyboard.SetTargetProperty(fillOutAnimation,
                new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
            fillOutStoryBoard.Begin(ProgressBar, true);
        }

        public void PauseFillOutAnimation() => fillOutStoryBoard?.Pause(ProgressBar);

        public void ResumeFillOutAnimtion() => fillOutStoryBoard?.Resume(ProgressBar);

        public void StartFillInAnimation()
        {
            Storyboard.SetTarget(fillInStoryBoard, ProgressBar);
            Storyboard.SetTargetProperty(fillInStoryBoard,
                new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
            fillInStoryBoard.Begin(ProgressBar);
        }

        #endregion

        #region Transition animation in/out

        private void StartTransitionStoryboard(Storyboard storyboard,
            ObservableCollection<StoryBoardContextItem>? animationContext,
            FrameworkElement target)
        {
            if (animationContext?.Count > 0 != true) return;

            foreach (var item in animationContext)
            {
                Storyboard.SetTarget(item.Animation, target);
                Storyboard.SetTargetProperty(item.Animation, item.PropertyPath);
            }

            Timeline.SetDesiredFrameRate(storyboard, 60);
            storyboard?.Begin();
        }

        private void StartTransitionIn()
        {
            RaiseEvent(new RoutedEventArgs(OnOpenEvent));
            StartTransitionStoryboard(forwardStoryBoard, StoryBoardContext?.ForwardAnimation, Root);
        }

        private void StartTransitionOut()
        {
            if (StoryBoardContext?.BackwardAnimation?.Count > 0 != true)
            {
                OnTransitionOutCompleted(this, EventArgs.Empty);
                return;
            }

            backwardStoryBoard.Completed -= OnTransitionOutCompleted;
            backwardStoryBoard.Completed += OnTransitionOutCompleted;
            StartTransitionStoryboard(backwardStoryBoard, StoryBoardContext?.BackwardAnimation, Root);
        }

        private void OnTransitionOutCompleted(object? sender, EventArgs e)
        {
            backwardStoryBoard.Completed -= OnTransitionOutCompleted;
            RaiseEvent(new RoutedEventArgs(OnCloseEvent));
        }

        #endregion

        #region Setup Animation & Storyboard

        private void SetupTransitions()
        {
            StoryBoardContext = StoryBoardContext ?? StoryBoardContext.InitDefault(AnimationDuration);

            forwardStoryBoard = new Storyboard();
            backwardStoryBoard = new Storyboard();

            SeedAnimationToStoryBoard(forwardStoryBoard,
                StoryBoardContext?.ForwardAnimation?.Select(item => item.Animation)?.ToList());

            SeedAnimationToStoryBoard(backwardStoryBoard,
                StoryBoardContext?.BackwardAnimation?.Select(item => item.Animation)?.ToList());
        }

        private void SeedAnimationToStoryBoard(Storyboard storyboard, List<AnimationTimeline?>? animations)
        {
            if (animations?.Count > 0 != true) return;
            foreach (var item in animations)
            {
                storyboard.Children.Add(item);
            }
        }

        private void SetupFillOutStoryboard()
        {
            if (fillOutStoryBoard != null)
            {
                fillOutAnimation = null!;
                fillOutStoryBoard.Stop();
            }

            fillOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = DisplayDuration,
                FillBehavior = FillBehavior.HoldEnd
            };

            fillOutStoryBoard = new Storyboard();
            fillOutStoryBoard.Children.Add(fillOutAnimation);
        }

        private void SetupFillInStoryboard()
        {
            fillInAnimation = new DoubleAnimation
            {
                From = 0,
                To = 10,
                Duration = DisplayDuration,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                FillBehavior = FillBehavior.HoldEnd,
                BeginTime = DisplayDuration
            };

            fillInStoryBoard = new Storyboard();
            fillInStoryBoard.Children.Add(fillInAnimation);
        }

        #endregion
    }

}
