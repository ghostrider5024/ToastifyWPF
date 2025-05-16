using System.Windows.Media;
using ToastifyWPF.Enums;
using ToastifyWPF.Helpers;

namespace ToastifyWPF.Models
{
    public class ToastNotificationTheme
    {
        public ToastThemeEnum ToastTheme { get; set; }
        public ToastTypeEnum ToastType { get; set; }
        public SolidColorBrush Foreground { get; set; }
        public SolidColorBrush Background { get; set; }
        public SolidColorBrush IndicatorProgressColor { get; set; }
        public SolidColorBrush BackgroundProgressColor { get; set; }

        public ToastNotificationTheme()
        {
            
        }

        public static ToastNotificationTheme InitTheme(ToastThemeEnum toastTheme = ToastThemeEnum.Light
            , ToastTypeEnum toastType = ToastTypeEnum.Info)
        {
            return toastTheme switch
            {
                ToastThemeEnum.Light => InitLightTheme(toastType),
                _ => InitLightTheme(toastType)
            };
        }

        public static ToastNotificationTheme InitLightTheme(ToastTypeEnum toastType)
        {
            return toastType switch
            {
                ToastTypeEnum.Info => InitLightInforTheme()
            };
        }

        public static ToastNotificationTheme InitLightInforTheme()
        {

            return new ToastNotificationTheme()
            {
                ToastTheme = ToastThemeEnum.Light,
                ToastType = ToastTypeEnum.Info,
                Foreground = ColorHelper.FrozenBrush(Colors.Black),
                Background = ColorHelper.FrozenBrush(Colors.White),
                IndicatorProgressColor = ColorHelper.FrozenBrush(Color.FromRgb(52, 152, 219)),
                BackgroundProgressColor = ColorHelper.FrozenBrush(Color.FromRgb(214, 234, 248)),
            };
        }
    }
}
