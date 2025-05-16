using System.Windows.Controls;
using System.Windows;

using ToastifyWPF.Controls;
using ToastifyWPF.Managers;
using ToastifyWPF.Models;


namespace ToastifyWPF.UI
{
    /// <summary>
    /// Interaction logic for ToastHost.xaml
    /// </summary>
    public partial class ToastHost : UserControl
    {
        // DependencyProperty cho MaxToastCount
        public static readonly DependencyProperty MaxToastCountProperty =
            DependencyProperty.Register(
                nameof(MaxToastCount),
                typeof(int?),
                typeof(ToastHost),
                new PropertyMetadata(null)); 

        public int? MaxToastCount
        {
            get => (int?)GetValue(MaxToastCountProperty);
            set => SetValue(MaxToastCountProperty, value);
        }

        public ToastHost()
        {
            InitializeComponent();
            ToastNotificationManager.Instance.SetHost(this);
        }

        public void ShowToast(ToastNotificationData toastNotificationData)
        {
            var toast = ToastNotificationPool.Instance.Get();

            toast.OnClose += Toast_OnClose;

            if (MaxToastCount != null && ToastPanel.Children.Count >= MaxToastCount)
            {
                if (ToastPanel.Children[ToastPanel.Children.Count - 1] is ToastNotification oldToast)
                {
                    oldToast.OnClose -= Toast_OnClose;
                    ToastPanel.Children.Remove(oldToast);
                    ToastNotificationPool.Instance.Return(oldToast);
                }
            }

            ToastPanel.Children.Insert(0, toast);
            toast.Show(toastNotificationData);
        }

        private void Toast_OnClose(object sender, RoutedEventArgs e)
        {
            if (sender is ToastNotification child)
            {
                ToastPanel.Children.Remove(child);
                child.OnClose -= Toast_OnClose;
                ToastNotificationPool.Instance.Return(child);
            }
        }
    }

}
