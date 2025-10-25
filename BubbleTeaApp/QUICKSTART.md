# HƯỚNG DẪN THIẾT LẬP DATABASE - NHANH

## 🚀 Cách nhanh nhất (Khuyến nghị)

### Bước 1: Chạy script tự động
```powershell
.\setup-database.ps1
```

Script sẽ tự động:
- ✓ Kiểm tra .NET SDK
- ✓ Cài EF Core Tools (nếu chưa có)
- ✓ Restore packages
- ✓ Build project
- ✓ Tạo migration
- ✓ Tạo database với dữ liệu mẫu

---

## ⚠️ Nếu gặp lỗi kết nối SQL Server

### Giải pháp 1: Sử dụng LocalDB (Đơn giản nhất)
LocalDB thường có sẵn nếu bạn cài Visual Studio

1. Mở file `Data/BubbleTeaDbContext.cs`
2. Tìm dòng có `OPTION 1: LocalDB`
3. Đảm bảo dòng này đang được dùng (không có `//` ở đầu)
4. Chạy lại script

### Giải pháp 2: Cài SQL Server Express
1. Tải: https://www.microsoft.com/sql-server/sql-server-downloads
2. Cài "Express" edition (miễn phí)
3. Mở file `Data/BubbleTeaDbContext.cs`
4. Bỏ comment dòng `OPTION 2: SQL Server Express`
5. Chạy lại script

### Giải pháp 3: Tìm tên SQL Server của bạn
1. Mở SQL Server Management Studio (SSMS)
2. Khi connect, copy tên Server
3. Mở file `Data/BubbleTeaDbContext.cs`
4. Thay `YOUR_SERVER_NAME` bằng tên server đã copy
5. Chạy lại script

---

## 🔧 Các lệnh thủ công (nếu cần)

```powershell
# Restore packages
dotnet restore

# Tạo migration
dotnet ef migrations add InitialCreate

# Tạo database
dotnet ef database update

# Chạy ứng dụng
dotnet run
```

---

## 📊 Kiểm tra Database đã tạo thành công

Sau khi chạy thành công, bạn sẽ có:

### Database: `BubbleTeaDB`

**Bảng Products** - 10 sản phẩm:
- Trà Sữa Truyền Thống (35,000đ)
- Trà Sữa Matcha (40,000đ)
- Trà Sữa Socola (42,000đ)
- ... và 7 sản phẩm khác

**Bảng Toppings** - 8 loại topping:
- Trân Châu Đen (5,000đ)
- Trân Châu Trắng (5,000đ)
- Thạch Dừa (5,000đ)
- ... và 5 topping khác

**Các bảng khác:**
- Orders - Lưu đơn hàng
- OrderDetails - Chi tiết đơn hàng
- OrderToppings - Topping của từng món

---

## 🎯 Chạy ứng dụng

```powershell
dotnet run
```

Ứng dụng sẽ tự động load dữ liệu từ database!

---

## 🔄 Reset Database (nếu cần)

```powershell
# Xóa database hiện tại
dotnet ef database drop

# Tạo lại từ đầu
dotnet ef database update
```

---

## ❓ Các lỗi thường gặp

### Lỗi: "dotnet ef not found"
```powershell
dotnet tool install --global dotnet-ef
```

### Lỗi: "Cannot connect to SQL Server"
→ Xem phần "Giải pháp" ở trên

### Lỗi: "Database already exists"
```powershell
dotnet ef database drop
dotnet ef database update
```

---

## 📞 Cần trợ giúp?

1. Đọc file `SETUP_DATABASE.md` (chi tiết hơn)
2. Kiểm tra connection string trong `Data/BubbleTeaDbContext.cs`
3. Đảm bảo SQL Server đang chạy
