using System.Windows.Media;

namespace ToastifyWPF.Helpers
{
    public static class ColorHelper
    {
        public static SolidColorBrush FrozenBrush(Color color)
        {
            var b = new SolidColorBrush(color);
            b.Freeze();
            return b;
        }

    }
}
