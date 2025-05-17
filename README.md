# 🔔 Toast Notification System for WPF (No MVVM Required)

Hệ thống toast nhẹ, hiệu quả và dễ dùng cho WPF, không cần MVVM – có thể gọi trực tiếp từ bất kỳ đâu trong code.  
Hỗ trợ giới hạn số lượng toast, tái sử dụng UI toast thông qua pooling, hỗ trợ nhiều kiểu thông báo.

---

## 🚀 Cách sử dụng

### 1. Thêm `ToastHost` vào UI

Chèn `ToastHost` vào XAML – thường là ở `MainWindow.xaml` hoặc control nào luôn tồn tại:

```xml
<Window ...
        xmlns:ui="clr-namespace:YourApp.Controls">
    <Grid>
        <!-- Các nội dung khác -->

        <!-- ToastHost hiển thị toast -->
        <ui:ToastHost Padding="0,10,0,0" MaxToastCount="10" />
    </Grid>
</Window>
```

### 2. Gọi toast từ code (không MVVM)

Chỉ cần gọi toast như sau – bạn có thể gọi từ bất kỳ đâu: sự kiện button, xử lý API, v.v.

csharp
Copy
Edit

```csharp
public class HelloWorld
{
    public void DoSomething()
    {
        var notification = new ToastNotificationData
        {
            Message = "Đăng nhập thành công!",
            Type = ToastTypeEnum.Success
        };

        ToastNotificationManager.Instance.Show(notification);
    }
}
```

# 🍞 ToastHost for WPF

`ToastHost` là một control tùy biến để hiển thị các toast notification trong WPF, tương tự như `react-toastify` trong React. Bạn có thể đặt nó vào bất kỳ `Window` hoặc `UserControl` nào và gọi thông báo từ mọi nơi trong code.

---

## 🧩 Thuộc tính (Dependency Properties)

| Tên Property    | Kiểu dữ liệu | Mặc định | Mô tả                                                                                |
| --------------- | ------------ | -------- | ------------------------------------------------------------------------------------ |
| `MinWidthItem`  | `double`     | `310`    | Chiều rộng tối thiểu của mỗi toast.                                                  |
| `MaxWidthItem`  | `double`     | `500`    | Chiều rộng tối đa của mỗi toast.                                                     |
| `MaxToastCount` | `int?`       | `10`     | Số lượng toast hiển thị cùng lúc. Nếu vượt quá, toast cũ nhất sẽ bị loại bỏ tự động. |

---

## ⚙️ Phương thức công khai

| Tên phương thức  | Trả về | Tham số                 | Mô tả                                                                                   |
| ---------------- | ------ | ----------------------- | --------------------------------------------------------------------------------------- |
| `ShowToast(...)` | `void` | `ToastNotificationData` | Hiển thị một toast mới với dữ liệu truyền vào. Nếu toast đang đầy, sẽ loại bỏ toast cũ. |
| `ToastHost()`    | —      | Không                   | Hàm khởi tạo. Đăng ký chính `ToastHost` vào `ToastNotificationManager`.                 |

---

## 📦 Ví dụ sử dụng trong XAML

```xml
<ui:ToastHost Padding="0,10,0,0" MaxToastCount="10" />

```

# 🍞 ToastNotification Class

## 🧩 Public Properties

| Property Name            | Type                     | Description                                                      |
| ------------------------ | ------------------------ | ---------------------------------------------------------------- |
| `DisplayDuration`        | `TimeSpan`               | Thời gian thông báo hiển thị trước khi tự động ẩn (mặc định 3s). |
| `AnimationDuration`      | `TimeSpan`               | Thời lượng animation vào-ra (mặc định 300ms).                    |
| `CornerRadius`           | `CornerRadius`           | Bo góc của thông báo.                                            |
| `StoryBoardContext`      | `StoryBoardContext?`     | Context định nghĩa animation storyboard cho transition.          |
| `ToastNotificationTheme` | `ToastNotificationTheme` | Theme hiện tại của thông báo (màu sắc, icon, kiểu nền...).       |
| `Data`                   | `ToastNotificationData`  | Dữ liệu nội dung của thông báo (text, loại, v.v.).               |

## 📦 Public Methods

| Method Name                   | Return Type | Description                                                 |
| ----------------------------- | ----------- | ----------------------------------------------------------- |
| `Show()`                      | `void`      | Hiển thị toast với dữ liệu đã có sẵn.                       |
| `Show(ToastNotificationData)` | `void`      | Hiển thị toast với dữ liệu mới truyền vào.                  |
| `Hide()`                      | `void`      | Ẩn toast với animation.                                     |
| `Reset()`                     | `void`      | Reset trạng thái hiện tại của toast.                        |
| `Close()`                     | `void`      | Đóng toast và raise sự kiện `OnClose`.                      |
| `StartFillOutAnimation()`     | `void`      | Bắt đầu animation tiến trình thanh progress bar (fill out). |
| `PauseFillOutAnimation()`     | `void`      | Tạm dừng animation fill out.                                |
| `ResumeFillOutAnimtion()`     | `void`      | Tiếp tục animation fill out.                                |
| `StartFillInAnimation()`      | `void`      | Bắt đầu animation fill in (phần cuối progress bar).         |
