using System.ComponentModel;
using System.Runtime.CompilerServices;
using ToastifyWPF.Enums;

namespace ToastifyWPF.Models
{
    public class ToastNotificationData : INotifyPropertyChanged
    {
        // Tin nhắn trên Toast
        private string _message = string.Empty;
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

        // Tạm dừng countdown khi rê chuột lên toast (mặc định giữ hành vi cũ)
        public bool PauseOnHover { get; set; } = true;
        // Tạm dừng / tiếp tục countdown khi click vào nội dung toast
        public bool PauseOnClick { get; set; }
        // Đóng toast khi click vào nội dung (ưu tiên hơn PauseOnClick)
        public bool CloseOnClick { get; set; }
        
        // Hàm cập nhật text khi số giây còn lại thay đổi
        public Func<string?, int, string>? UpdateMessageAction { get; set; }
        // Hàm sau khi đã thông báo xong (thông báo đã ẩn)
        public Action? FinishAction { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
