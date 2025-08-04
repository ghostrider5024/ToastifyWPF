using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToastifyWPF.Enums;

namespace ToastifyWPF.Models
{
    public class ToastNotificationData : INotifyPropertyChanged
    {
        // Tin nhắn trên Toast
        private string _message;
        public string Message
        {
            get => _message;
            set
            {
                if (_message != value)
                {
                    _message = value;
                    OnPropertyChanged();
                }
            }
        }

        // Loại thông báo
        public ToastTypeEnum Type { get; set; }
        // Thời gian tồn tại của thông báo
        public TimeSpan? Duration { get; set; }
        
        // Hàm để cập nhật text sau khi một tick hoàn thành (thường là 1 giây)
        public Func<string?, int, string>? UpdateMessageAction { get; set; }
        // Hàm sau khi đã thông báo xong (thông báo đã ẩn)
        public Action? FinishAction { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
