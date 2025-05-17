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
        public SolidColorBrush ForegroundIcon { get; set; }

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
                ToastTypeEnum.Info => InitLightInforTheme(),
                ToastTypeEnum.Success => InitLightSuccessTheme(),
                ToastTypeEnum.Warning => InitLightWarningTheme(),
                ToastTypeEnum.Error => InitLightErrorTheme(),
            };
        }

        public static ToastNotificationTheme InitLightInforTheme()
        {
            var icon = ColorHelper.FrozenBrush(Color.FromRgb(52, 152, 219));
            return new ToastNotificationTheme()
            {
                ToastTheme = ToastThemeEnum.Light,
                ToastType = ToastTypeEnum.Info,
                Foreground = ColorHelper.FrozenBrush(Colors.Black),
                Background = ColorHelper.FrozenBrush(Colors.White),
                ForegroundIcon = icon,
                IndicatorProgressColor = icon,
                BackgroundProgressColor = ColorHelper.FrozenBrush(Color.FromRgb(214, 234, 248)),
            };
        }
        
        public static ToastNotificationTheme InitLightSuccessTheme()
        {
            var icon = ColorHelper.FrozenBrush(Color.FromRgb(7, 188, 12));
            return new ToastNotificationTheme()
            {
                ToastTheme = ToastThemeEnum.Light,
                ToastType = ToastTypeEnum.Success,
                Foreground = ColorHelper.FrozenBrush(Colors.Black),
                Background = ColorHelper.FrozenBrush(Colors.White),
                ForegroundIcon = icon,
                IndicatorProgressColor = icon,
                BackgroundProgressColor = ColorHelper.FrozenBrush(Color.FromRgb(205, 242, 206)),
            };
        }

        public static ToastNotificationTheme InitLightWarningTheme()
        {
            var icon = ColorHelper.FrozenBrush(Color.FromRgb(241, 196, 15));
            return new ToastNotificationTheme()
            {
                ToastTheme = ToastThemeEnum.Light,
                ToastType = ToastTypeEnum.Warning,
                Foreground = ColorHelper.FrozenBrush(Colors.Black),
                Background = ColorHelper.FrozenBrush(Colors.White),
                ForegroundIcon = icon,
                IndicatorProgressColor = icon,
                BackgroundProgressColor = ColorHelper.FrozenBrush(Color.FromRgb(252, 243, 207)),
            };
        }
        
        public static ToastNotificationTheme InitLightErrorTheme()
        {
            var icon = ColorHelper.FrozenBrush(Color.FromRgb(231, 77, 60));
            return new ToastNotificationTheme()
            {
                ToastTheme = ToastThemeEnum.Light,
                ToastType = ToastTypeEnum.Error,
                Foreground = ColorHelper.FrozenBrush(Colors.Black),
                Background = ColorHelper.FrozenBrush(Colors.White),
                ForegroundIcon = icon,
                IndicatorProgressColor = ColorHelper.FrozenBrush(Color.FromRgb(237, 119, 106)),
                BackgroundProgressColor = ColorHelper.FrozenBrush(Color.FromRgb(250, 219, 216)),
            };
        }
    }
}
