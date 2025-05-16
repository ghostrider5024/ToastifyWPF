using ToastifyWPF.Controls;

namespace ToastifyWPF.Models
{
    public class ToastNotificationPool
    {
        private readonly Stack<ToastNotification> _pool = new();

        public ToastNotification Get()
        {
            if (_pool.Count > 0)
            {
                return _pool.Pop();
            }

            return new ToastNotification(); // Tạo mới nếu hết pool
        }

        public void Return(ToastNotification toast)
        {
            // Optional: reset nội dung nếu cần
            toast.Reset();
            _pool.Push(toast);
        }

        public static ToastNotificationPool Instance { get; } = new();
    }

}
