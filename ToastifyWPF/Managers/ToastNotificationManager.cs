using ToastifyWPF.Controls;
using ToastifyWPF.Models;
using ToastifyWPF.UI;

namespace ToastifyWPF.Managers
{
    public class ToastNotificationManager
    {
        private static readonly ToastNotificationManager _instance = new ToastNotificationManager();
        public static ToastNotificationManager Instance => _instance;

        private ToastHost _host;

        private ToastNotificationManager() { }

        public void SetHost(ToastHost host)
        {
            _host = host;
        }

        public void Show(ToastNotificationData toastNotificationData)
        {
            _host?.Dispatcher.Invoke(() => _host.ShowToast(toastNotificationData));
        }
    }
}
