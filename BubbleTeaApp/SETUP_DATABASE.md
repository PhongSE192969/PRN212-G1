# Hướng dẫn thiết lập Database cho Bubble Tea App

## Bước 1: Cài đặt SQL Server
1. Tải và cài đặt **SQL Server Express** từ: https://www.microsoft.com/sql-server/sql-server-downloads
2. Tải và cài đặt **SQL Server Management Studio (SSMS)** từ: https://aka.ms/ssmsfullsetup

## Bước 2: Kiểm tra kết nối SQL Server
1. Mở SSMS
2. Kết nối với server name: `localhost\SQLEXPRESS` hoặc `(localdb)\MSSQLLocalDB`
3. Nếu kết nối thành công, copy tên server

## Bước 3: Cập nhật Connection String
1. Mở file `BubbleTeaApp/Data/BubbleTeaDbContext.cs`
2. Tìm dòng:
```csharp
optionsBuilder.UseSqlServer(
    @"Server=localhost\SQLEXPRESS;Database=BubbleTeaDB;Trusted_Connection=True;TrustServerCertificate=True;");
```
3. Thay `localhost\SQLEXPRESS` bằng tên server của bạn (nếu khác)

## Bước 4: Chạy Migration để tạo Database

### Mở PowerShell/Terminal tại thư mục project và chạy:

```powershell
# Restore packages
dotnet restore

# Tạo migration đầu tiên
dotnet ef migrations add InitialCreate

# Cập nhật database (tạo tables và seed data)
dotnet ef database update
```

## Bước 5: Kiểm tra Database
1. Mở SSMS
2. Refresh Databases
3. Kiểm tra database `BubbleTeaDB` đã được tạo
4. Kiểm tra các bảng:
   - Products (10 sản phẩm mẫu)
   - Toppings (8 topping mẫu)
   - Orders
   - OrderDetails
   - OrderToppings

## Bước 6: Chạy ứng dụng
```powershell
dotnet run
```

## Cấu trúc Database

### Table: Products
- Id (int, PK)
- Name (nvarchar(200))
- Price (decimal(18,2))
- Category (nvarchar(100))
- ImageUrl (nvarchar(500))
- Description (nvarchar(1000))

### Table: Toppings
- Id (int, PK)
- Name (nvarchar(100))
- Price (decimal(18,2))

### Table: Orders
- Id (int, PK)
- OrderDate (datetime)
- TotalAmount (decimal(18,2))
- CustomerName (nvarchar(100))
- PhoneNumber (nvarchar(20))
- Address (nvarchar(500))
- Status (nvarchar(50))
- Notes (nvarchar(1000))

### Table: OrderDetails
- Id (int, PK)
- OrderId (int, FK)
- ProductId (int, FK)
- Quantity (int)
- UnitPrice (decimal(18,2))
- TotalPrice (decimal(18,2))

### Table: OrderToppings
- Id (int, PK)
- OrderDetailId (int, FK)
- ToppingId (int, FK)
- Price (decimal(18,2))

## Lưu ý quan trọng

1. **Nếu gặp lỗi "dotnet ef not found":**
```powershell
dotnet tool install --global dotnet-ef
```

2. **Nếu muốn reset database:**
```powershell
dotnet ef database drop
dotnet ef database update
```

3. **Nếu muốn thêm migration mới:**
```powershell
dotnet ef migrations add TenMigration
dotnet ef database update
```

## Các truy vấn SQL hữu ích

### Xem tất cả sản phẩm:
```sql
SELECT * FROM Products
```

### Xem tất cả đơn hàng:
```sql
SELECT * FROM Orders ORDER BY OrderDate DESC
```

### Xem chi tiết đơn hàng:
```sql
SELECT o.Id, o.OrderDate, o.TotalAmount, o.Status,
       p.Name AS ProductName, od.Quantity, od.TotalPrice
FROM Orders o
JOIN OrderDetails od ON o.Id = od.OrderId
JOIN Products p ON od.ProductId = p.Id
ORDER BY o.OrderDate DESC
```

### Xem doanh thu theo ngày:
```sql
SELECT CAST(OrderDate AS DATE) AS Date, 
       COUNT(*) AS TotalOrders,
       SUM(TotalAmount) AS Revenue
FROM Orders
WHERE Status = 'Completed'
GROUP BY CAST(OrderDate AS DATE)
ORDER BY Date DESC
```

## Troubleshooting

### Lỗi kết nối database:
- Kiểm tra SQL Server đang chạy
- Kiểm tra tên server đúng
- Kiểm tra firewall không chặn kết nối

### Lỗi migration:
- Xóa thư mục Migrations và tạo lại
- Kiểm tra các Model class có đúng không
- Kiểm tra DbContext configuration

### Lỗi chạy ứng dụng:
- Kiểm tra database đã được tạo
- Kiểm tra seed data đã được insert
- Xem chi tiết lỗi trong Exception message
