using System.Windows.Controls;
using System.Windows;

using ToastifyWPF.Controls;
using ToastifyWPF.Managers;
using ToastifyWPF.Models;


namespace ToastifyWPF.UI
{
    /// <summary>
    /// ToastHost là control dùng để hiển thị và quản lý danh sách các toast notification.
    /// Toast được tái sử dụng từ pool nhằm tối ưu hiệu năng.
    /// </summary>
    public partial class ToastHost : UserControl
    {
        #region Dependency Properties

        /// <summary>
        /// Chiều rộng tối thiểu của mỗi toast notification.
        /// </summary>
        public double MinWidthItem
        {
            get => (double)GetValue(MinWidthItemProperty);
            set => SetValue(MinWidthItemProperty, value);
        }

        /// <summary>
        /// DependencyProperty cho MinWidthItem.
        /// </summary>
        public static readonly DependencyProperty MinWidthItemProperty =
            DependencyProperty.Register(
                nameof(MinWidthItem),
                typeof(double),
                typeof(ToastHost),
                new PropertyMetadata(310.0));

        /// <summary>
        /// Chiều rộng tối đa của mỗi toast notification.
        /// </summary>
        public double MaxWidthItem
        {
            get => (double)GetValue(MaxWidthItemProperty);
            set => SetValue(MaxWidthItemProperty, value);
        }

        /// <summary>
        /// DependencyProperty cho MaxWidthItem.
        /// </summary>
        public static readonly DependencyProperty MaxWidthItemProperty =
            DependencyProperty.Register(
                nameof(MaxWidthItem),
                typeof(double),
                typeof(ToastHost),
                new PropertyMetadata(500.0));

        /// <summary>
        /// Số lượng toast tối đa được hiển thị cùng lúc. Nếu vượt quá thì toast cũ nhất sẽ bị remove.
        /// </summary>
        public int? MaxToastCount
        {
            get => (int?)GetValue(MaxToastCountProperty);
            set => SetValue(MaxToastCountProperty, value);
        }

        /// <summary>
        /// DependencyProperty cho MaxToastCount.
        /// </summary>
        public static readonly DependencyProperty MaxToastCountProperty =
            DependencyProperty.Register(
                nameof(MaxToastCount),
                typeof(int?),
                typeof(ToastHost),
                new PropertyMetadata(10));

        #endregion

        /// <summary>
        /// Khởi tạo ToastHost và đăng ký instance này vào ToastNotificationManager.
        /// </summary>
        public ToastHost()
        {
            InitializeComponent();
            ToastNotificationManager.Instance.SetHost(this);
        }

        /// <summary>
        /// Hiển thị một toast notification mới với dữ liệu truyền vào.
        /// Nếu đã đạt giới hạn toast, sẽ tự động xóa toast cũ nhất.
        /// </summary>
        /// <param name="toastNotificationData">Dữ liệu hiển thị trên toast.</param>
        public void ShowToast(ToastNotificationData toastNotificationData)
        {
            var toast = ToastNotificationPool.Instance.Get();

            // Lắng nghe sự kiện toast đóng
            toast.OnClose += Toast_OnClose;

            // Truyền thuộc tính cấu hình vào toast
            SeedPropToItem(toast);

            // Kiểm tra và remove toast cũ nếu đã đầy
            if (MaxToastCount != null && ToastPanel.Children.Count >= MaxToastCount)
            {
                if (ToastPanel.Children[ToastPanel.Children.Count - 1] is ToastNotification oldToast)
                {
                    oldToast.OnClose -= Toast_OnClose;
                    ToastPanel.Children.Remove(oldToast);
                    ToastNotificationPool.Instance.Return(oldToast);
                }
            }

            // Thêm toast mới vào đầu danh sách
            ToastPanel.Children.Insert(0, toast);
            toast.Show(toastNotificationData);
        }

        /// <summary>
        /// Gán các giá trị cấu hình (MinWidth, MaxWidth) từ ToastHost vào toast item.
        /// </summary>
        private void SeedPropToItem(ToastNotification toast)
        {
            toast.MinWidth = MinWidthItem;
            toast.MaxWidth = MaxWidthItem;
        }

        /// <summary>
        /// Hàm callback khi toast đóng: gỡ bỏ khỏi giao diện và trả về pool.
        /// </summary>
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
