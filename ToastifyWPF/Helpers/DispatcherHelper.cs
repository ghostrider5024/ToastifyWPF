using System.Windows.Threading;

namespace ToastifyWPF.Helpers
{
    public static class DispatcherHelper
    {
        public static void InvokeOnUIThread(Action action)
        {
            if (Dispatcher.CurrentDispatcher.CheckAccess())
                action();
            else
                Dispatcher.CurrentDispatcher.Invoke(action);
        }
    }
}
