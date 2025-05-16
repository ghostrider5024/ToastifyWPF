using System.Windows;
using System.Windows.Media.Animation;

namespace ToastifyWPF.Models
{
    public class StoryBoardContextItem
    {
        public AnimationTimeline? Animation { get; set; }
        public PropertyPath? PropertyPath { get; set; }
    }
}
