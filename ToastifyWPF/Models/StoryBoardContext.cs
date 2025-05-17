using System.Collections.ObjectModel;
using System.Windows.Media.Animation;
using System.Windows;

namespace ToastifyWPF.Models
{
    public class StoryBoardContext
    {
        public ObservableCollection<StoryBoardContextItem>? ForwardAnimation { get; set; }
        public ObservableCollection<StoryBoardContextItem>? BackwardAnimation { get; set; }

        public static StoryBoardContext InitDefault(TimeSpan animationDuration)
        {
            StoryBoardContext storyBoardContext = new StoryBoardContext();
            var fadeAnimationPropertyPath = new PropertyPath(UIElement.OpacityProperty);
            var slideAnimationPropertyPath = new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.X)");

            storyBoardContext.ForwardAnimation = new ObservableCollection<StoryBoardContextItem>
            {
                new StoryBoardContextItem
                {
                    Animation = new DoubleAnimation(0, 1, animationDuration)
                    {
                        EasingFunction = new SineEase { EasingMode = EasingMode.EaseOut }
                    },
                    PropertyPath = fadeAnimationPropertyPath

                },
               new StoryBoardContextItem
               {
                   Animation = new DoubleAnimation(100, 0, animationDuration)
                   {
                        EasingFunction = new BackEase
                        {
                            EasingMode = EasingMode.EaseOut,
                            Amplitude = 1.2
                        }
                   },
                   PropertyPath = slideAnimationPropertyPath
               }

            };
            storyBoardContext.BackwardAnimation = new ObservableCollection<StoryBoardContextItem>
            {
                new StoryBoardContextItem
                {
                    Animation = new DoubleAnimation(1, 0, animationDuration)
                    {
                        EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
                    },
                    PropertyPath = fadeAnimationPropertyPath
                },
                new StoryBoardContextItem
                {
                    Animation = new DoubleAnimation(0, 100, animationDuration)
                    {
                        EasingFunction = new CubicEase
                        {
                            EasingMode = EasingMode.EaseIn,
                        }
                    },
                    PropertyPath = slideAnimationPropertyPath
                }
            };

            return storyBoardContext;
        }
    }
}
