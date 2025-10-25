# ===================================
# QUẢN LÝ DATABASE - BUBBLE TEA APP
# ===================================

function Show-Menu {
    Clear-Host
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "  QUẢN LÝ DATABASE - BUBBLE TEA APP   " -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "1. Tạo/Cập nhật Database" -ForegroundColor Green
    Write-Host "2. Xem dữ liệu trong Database" -ForegroundColor Yellow
    Write-Host "3. Reset Database (Xóa và tạo lại)" -ForegroundColor Red
    Write-Host "4. Xóa Database" -ForegroundColor Red
    Write-Host "5. Chạy ứng dụng" -ForegroundColor Green
    Write-Host "6. Thoát" -ForegroundColor Gray
    Write-Host ""
}

function Create-Database {
    Write-Host "→ Đang tạo/cập nhật database..." -ForegroundColor Yellow
    dotnet ef database update
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Thành công!" -ForegroundColor Green
    } else {
        Write-Host "✗ Có lỗi xảy ra!" -ForegroundColor Red
    }
    Pause
}

function View-Data {
    Write-Host "→ Đang kết nối database..." -ForegroundColor Yellow
    
    # Tạo script SQL để xem dữ liệu
    $sqlScript = @"
-- Xem số lượng dữ liệu
SELECT 'Products' AS TableName, COUNT(*) AS Count FROM Products
UNION ALL
SELECT 'Toppings', COUNT(*) FROM Toppings
UNION ALL
SELECT 'Orders', COUNT(*) FROM Orders
UNION ALL
SELECT 'OrderDetails', COUNT(*) FROM OrderDetails
UNION ALL
SELECT 'OrderToppings', COUNT(*) FROM OrderToppings;

-- Xem sản phẩm
SELECT 'DANH SÁCH SẢN PHẨM' AS Info;
SELECT Id, Name, Price, Category FROM Products;

-- Xem topping
SELECT 'DANH SÁCH TOPPING' AS Info;
SELECT Id, Name, Price FROM Toppings;

-- Xem đơn hàng gần đây
SELECT 'ĐƠN HÀNG GẦN ĐÂY (TOP 5)' AS Info;
SELECT TOP 5 Id, OrderDate, TotalAmount, Status 
FROM Orders 
ORDER BY OrderDate DESC;
"@
    
    Write-Host ""
    Write-Host "Để xem chi tiết dữ liệu, mở SQL Server Management Studio và chạy query trên database BubbleTeaDB" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Hoặc sử dụng VS Code với extension SQL Server" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Connection String:" -ForegroundColor Yellow
    Write-Host "Server=(localdb)\MSSQLLocalDB;Database=BubbleTeaDB;Trusted_Connection=True;" -ForegroundColor White
    Write-Host ""
    Pause
}

function Reset-Database {
    Write-Host "⚠️  CẢNH BÁO: Thao tác này sẽ XÓA TẤT CẢ dữ liệu!" -ForegroundColor Red
    $confirm = Read-Host "Bạn có chắc chắn muốn reset database? (yes/no)"
    
    if ($confirm -eq 'yes') {
        Write-Host "→ Đang xóa database..." -ForegroundColor Yellow
        dotnet ef database drop --force
        
        Write-Host "→ Đang tạo lại database..." -ForegroundColor Yellow
        dotnet ef database update
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Reset thành công!" -ForegroundColor Green
        } else {
            Write-Host "✗ Có lỗi xảy ra!" -ForegroundColor Red
        }
    } else {
        Write-Host "Đã hủy." -ForegroundColor Gray
    }
    Pause
}

function Remove-Database {
    Write-Host "⚠️  CẢNH BÁO: Thao tác này sẽ XÓA database!" -ForegroundColor Red
    $confirm = Read-Host "Bạn có chắc chắn? (yes/no)"
    
    if ($confirm -eq 'yes') {
        Write-Host "→ Đang xóa database..." -ForegroundColor Yellow
        dotnet ef database drop --force
        
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Đã xóa database!" -ForegroundColor Green
        } else {
            Write-Host "✗ Có lỗi xảy ra!" -ForegroundColor Red
        }
    } else {
        Write-Host "Đã hủy." -ForegroundColor Gray
    }
    Pause
}

function Run-App {
    Write-Host "→ Đang khởi động ứng dụng..." -ForegroundColor Yellow
    dotnet run
}

# Main loop
do {
    Show-Menu
    $choice = Read-Host "Chọn chức năng (1-6)"
    
    switch ($choice) {
        '1' { Create-Database }
        '2' { View-Data }
        '3' { Reset-Database }
        '4' { Remove-Database }
        '5' { Run-App; break }
        '6' { 
            Write-Host "Tạm biệt!" -ForegroundColor Cyan
            break 
        }
        default { 
            Write-Host "Lựa chọn không hợp lệ!" -ForegroundColor Red
            Pause
        }
    }
} while ($choice -ne '6' -and $choice -ne '5')
