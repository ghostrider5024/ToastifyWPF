using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

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
        public ToastNotificationData Data
        {
            get => (ToastNotificationData)GetValue(DataProperty);
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

        DispatcherTimer timer;
        private int currentTime = 0;
        private readonly int intervalTime = 1000; // ms
        private bool mouseEntering = false;
        private bool isFirstRender = true;

        private Storyboard fillOutStoryBoard;
        private Storyboard fillInStoryBoard;
        private Storyboard forwardStoryBoard;
        private Storyboard backwardStoryBoard;

        private AnimationTimeline fillOutAnimation;
        private AnimationTimeline fillInAnimation;
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
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(intervalTime)
            };

            timer.Tick += (s, e) =>
            {
                if (mouseEntering) return;

                currentTime += intervalTime;

                if (currentTime >= DisplayDuration.TotalMilliseconds)
                {
                    Hide();
                }
            };
        }
        #endregion

        #region Tương tác chuột

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            mouseEntering = true;
            PauseFillOutAnimation();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            mouseEntering = false;
            ResumeFillOutAnimtion();
        }

        #endregion

        #region Public API

        /// <summary>
        /// Hiển thị toast với dữ liệu mới
        /// </summary>
        public void Show(ToastNotificationData toastNotificationData)
        {
            Root.Width = MinWidth;
            Data = toastNotificationData;

            Dispatcher.BeginInvoke(() =>
            {
                Root.Width = double.NaN;
            }, DispatcherPriority.Background);

            backwardStoryBoard?.Pause();
            Show();
        }

        /// <summary>
        /// Hiển thị toast (nếu đã có sẵn Data)
        /// </summary>
        public void Show()
        {
            ApplyTheme(ToastThemeEnum.Light, Data.Type);
            timer.Start();

            StartTransitionIn();
            StartFillOutAnimation();
        }

        /// <summary>
        /// Ẩn toast với animation
        /// </summary>
        public void Hide()
        {
            timer.Stop();
            StartTransitionOut();
            currentTime = 0;
        }

        /// <summary>
        /// Reset trạng thái hiện tại
        /// </summary>
        public void Reset()
        {
            Hide();
        }

        /// <summary>
        /// Đóng và raise sự kiện `OnClose`
        /// </summary>
        public void Close()
        {
            RaiseEvent(new RoutedEventArgs(OnCloseEvent));
        }

        #endregion

        #region Animation tiến trình (progress bar)

        public void StartFillOutAnimation()
        {
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
            backwardStoryBoard.Completed += (_, __) =>
            {
                RaiseEvent(new RoutedEventArgs(OnCloseEvent));
            };
            StartTransitionStoryboard(backwardStoryBoard, StoryBoardContext?.BackwardAnimation, Root);
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
            fillOutAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = DisplayDuration,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                FillBehavior = FillBehavior.HoldEnd
            };

            fillOutStoryBoard = new Storyboard();
            fillOutStoryBoard.Children.Add(fillOutAnimation);
            fillOutStoryBoard.Completed += (s, e) => StartFillInAnimation();
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
