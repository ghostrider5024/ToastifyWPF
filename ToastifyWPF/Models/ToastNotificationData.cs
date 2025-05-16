using ToastifyWPF.Enums;

namespace ToastifyWPF.Models
{
    public class ToastNotificationData
    {
        public string? Message { get; set; }
        public ToastTypeEnum Type { get; set; }
    }
}
