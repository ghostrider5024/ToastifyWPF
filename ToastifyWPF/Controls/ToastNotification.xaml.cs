using System.Windows.Media.Animation;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

using ToastifyWPF.Models;
using System.Windows.Media;
using ToastifyWPF.Enums;

namespace ToastifyWPF.Controls
{
    /// <summary>
    /// Interaction logic for ToastNotification.xaml
    /// </summary>
    public partial class ToastNotification : UserControl
    {
        #region DP
        public TimeSpan DisplayDuration
        {
            get { return (TimeSpan)GetValue(DisplayDurationProperty); }
            set { SetValue(DisplayDurationProperty, value); }
        }

        public static readonly DependencyProperty DisplayDurationProperty =
            DependencyProperty.Register(nameof(DisplayDuration)
                , typeof(TimeSpan), typeof(ToastNotification)
                , new PropertyMetadata(TimeSpan.FromSeconds(3)));

        public TimeSpan AnimationDuration
        {
            get { return (TimeSpan)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register(nameof(AnimationDuration)
                , typeof(TimeSpan), typeof(ToastNotification)
                , new PropertyMetadata(TimeSpan.FromMilliseconds(300)));
       
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register(nameof(CornerRadius)
                , typeof(CornerRadius), typeof(ToastNotification)
                , new PropertyMetadata(new CornerRadius(6)));


        public StoryBoardContext? StoryBoardContext
        {
            get { return (StoryBoardContext)GetValue(StoryBoardContextProperty); }
            set { SetValue(StoryBoardContextProperty, value); }
        }

        public static readonly DependencyProperty StoryBoardContextProperty =
            DependencyProperty.Register(nameof(StopStoryboard)
                , typeof(StoryBoardContext), typeof(ToastNotification)
                , new PropertyMetadata(null));

        public ToastNotificationTheme ToastNotificationTheme
        {
            get { return (ToastNotificationTheme)GetValue(ToastNotificationThemeProperty); }
            set { SetValue(ToastNotificationThemeProperty, value); }
        }

        public static readonly DependencyProperty ToastNotificationThemeProperty =
            DependencyProperty.Register(nameof(ToastNotificationTheme)
                , typeof(ToastNotificationTheme), typeof(ToastNotification)
                , new PropertyMetadata(default));



        #endregion DP

        #region Events
        public event RoutedEventHandler OnOpen
        {
            add => AddHandler(OnOpenEvent, value);
            remove => RemoveHandler(OnOpenEvent, value);
        }

        public static readonly RoutedEvent OnOpenEvent = EventManager.RegisterRoutedEvent(
                nameof(OnOpen),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ToastNotification));

        public event RoutedEventHandler OnClose
        {
            add => AddHandler(OnCloseEvent, value);
            remove => RemoveHandler(OnCloseEvent, value);
        }

        public static readonly RoutedEvent OnCloseEvent = EventManager.RegisterRoutedEvent(
                nameof(OnClose),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(ToastNotification));
        #endregion Events

        DispatcherTimer timer;

        private int currentTime = 0;
        private readonly int intervalTime = 1000;
        private bool mouseEntering = false;
        private bool isFirstRender = true;

        private Storyboard fillOutStoryBoard;
        private Storyboard fillInStoryBoard;
        private Storyboard forwardStoryBoard;
        private Storyboard backwardStoryBoard;

        private AnimationTimeline fillOutAnimation;
        private AnimationTimeline fillInAnimation;

        private ToastNotificationData? data;

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

        private void Prepare(ToastNotificationData? toastNotificationData = null)
        {
            data = toastNotificationData;

           
            SetupFillOutStoryboard();
            SetupFillInStoryboard();
            SetupTransitions();
            InitTimer();
        }

        private void ApplyTheme(ToastThemeEnum theme, ToastTypeEnum type)
        {
            ToastNotificationTheme = ToastNotificationTheme 
                ?? ToastNotificationTheme.InitTheme(theme, type);
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        public void Show(ToastNotificationData toastNotificationData)
        {
            data = toastNotificationData;
            Show();
        }

        public void Show()
        {
            ApplyTheme(ToastThemeEnum.Light, data.Type);
            MessageText.Text = data?.Message;

            timer.Start();

            StartTransitionIn();
            StartFillOutAnimation();

        }

        public void Hide()
        {
            timer.Stop();            
            StartTransitionOut();
            currentTime = 0;           
        }

        public void Reset()
        {
            
        }

        public void Close()
        {
            RaiseEvent(new RoutedEventArgs(OnCloseEvent));
        }

        #region Animation fill out
        public void StartFillOutAnimation()
        {
            Storyboard.SetTarget(fillOutAnimation, ProgressBar);
            Storyboard.SetTargetProperty(fillOutAnimation
                , new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));

            fillOutStoryBoard.Begin(ProgressBar, true);
        }

        public void PauseFillOutAnimation()
        {
            fillOutStoryBoard?.Pause(ProgressBar);
        }

        public void ResumeFillOutAnimtion()
        {
            fillOutStoryBoard?.Resume(ProgressBar);
        }
        #endregion Animation fill out


        #region Fill in
        public void StartFillInAnimation()
        {
            Storyboard.SetTarget(fillInStoryBoard, ProgressBar);
            Storyboard.SetTargetProperty(fillInStoryBoard
                , new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)")); 
            fillInStoryBoard.Begin(ProgressBar);
        }
        #endregion Fill in

        #region Transition


        private void StartTransitionStoryboard(Storyboard storyboard
            , ObservableCollection<StoryBoardContextItem>? animationContext
            , FrameworkElement target)
        {
            if (animationContext?.Count > 0 != true) return;

            foreach (var item in animationContext)
            {
                var animation = item.Animation;
                var propertyPath = item.PropertyPath;

                Storyboard.SetTarget(animation, target);
                Storyboard.SetTargetProperty(animation, propertyPath);
            }

            Timeline.SetDesiredFrameRate(storyboard, 60);
            storyboard?.Begin();
        }

        private void StartTransitionIn()
        {
            RaiseEvent(new RoutedEventArgs(OnOpenEvent));

            StartTransitionStoryboard(forwardStoryBoard
                , StoryBoardContext?.ForwardAnimation
                , Root);
        }
        private void StartTransitionOut()
        {
            backwardStoryBoard.Completed += 
                (_, __) => RaiseEvent(new RoutedEventArgs(OnCloseEvent));

            StartTransitionStoryboard(backwardStoryBoard
               , StoryBoardContext?.BackwardAnimation
               , Root);
        }
        #endregion Transition


        #region Set up
        private void SetupTransitions()
        {
            StoryBoardContext = StoryBoardContext ?? StoryBoardContext.InitDefault(AnimationDuration);
            forwardStoryBoard = new Storyboard();
            backwardStoryBoard = new Storyboard();

            SeedAnimationToStoryBoard(forwardStoryBoard, StoryBoardContext
                ?.ForwardAnimation
                ?.Select(item => item.Animation)
                ?.ToList());

            SeedAnimationToStoryBoard(backwardStoryBoard, StoryBoardContext
                ?.BackwardAnimation
                ?.Select(item => item.Animation)
                ?.ToList());
        }

        private void SeedAnimationToStoryBoard(Storyboard storyboard
            , List<AnimationTimeline?>? animations)
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

            fillOutStoryBoard.Completed += (s, e) =>
            {
                StartFillInAnimation();
            };

        }

        private void SetupFillInStoryboard()
        {
            fillInAnimation = new DoubleAnimation
            {
                
                From = 0,
                To = 10,
                Duration = DisplayDuration,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                FillBehavior = FillBehavior.HoldEnd
            };
            fillInStoryBoard = new Storyboard();
            fillInAnimation.BeginTime = DisplayDuration;
            fillInStoryBoard.Children.Add(fillInAnimation);

        }


        #endregion
    }
}
