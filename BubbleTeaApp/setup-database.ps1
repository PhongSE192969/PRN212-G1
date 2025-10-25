# ===================================
# SCRIPT THIẾT LẬP DATABASE TỰ ĐỘNG
# ===================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  THIẾT LẬP DATABASE - BUBBLE TEA APP  " -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Kiểm tra .NET SDK
Write-Host "1. Kiểm tra .NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "   ✓ .NET SDK: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "   ✗ Lỗi: Chưa cài .NET SDK" -ForegroundColor Red
    Write-Host "   Tải tại: https://dotnet.microsoft.com/download" -ForegroundColor Yellow
    exit
}

# Kiểm tra EF Core Tools
Write-Host ""
Write-Host "2. Kiểm tra Entity Framework Tools..." -ForegroundColor Yellow
try {
    $efVersion = dotnet ef --version
    Write-Host "   ✓ EF Core Tools đã cài đặt" -ForegroundColor Green
} catch {
    Write-Host "   → Đang cài đặt EF Core Tools..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
    Write-Host "   ✓ Đã cài đặt EF Core Tools" -ForegroundColor Green
}

# Restore packages
Write-Host ""
Write-Host "3. Restore NuGet packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✓ Restore thành công" -ForegroundColor Green
} else {
    Write-Host "   ✗ Lỗi khi restore packages" -ForegroundColor Red
    exit
}

# Build project
Write-Host ""
Write-Host "4. Build project..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✓ Build thành công" -ForegroundColor Green
} else {
    Write-Host "   ✗ Lỗi khi build project" -ForegroundColor Red
    exit
}

# Kiểm tra và xóa migration cũ nếu có
Write-Host ""
Write-Host "5. Kiểm tra migrations hiện tại..." -ForegroundColor Yellow
if (Test-Path "Migrations") {
    Write-Host "   → Tìm thấy thư mục Migrations cũ" -ForegroundColor Yellow
    $response = Read-Host "   Bạn có muốn xóa và tạo lại không? (y/n)"
    if ($response -eq 'y' -or $response -eq 'Y') {
        Remove-Item -Path "Migrations" -Recurse -Force
        Write-Host "   ✓ Đã xóa migrations cũ" -ForegroundColor Green
    }
}

# Tạo migration
Write-Host ""
Write-Host "6. Tạo migration..." -ForegroundColor Yellow
dotnet ef migrations add InitialCreate
if ($LASTEXITCODE -eq 0) {
    Write-Host "   ✓ Tạo migration thành công" -ForegroundColor Green
} else {
    Write-Host "   ✗ Lỗi khi tạo migration" -ForegroundColor Red
    exit
}

# Tạo/Cập nhật database
Write-Host ""
Write-Host "7. Tạo/Cập nhật database..." -ForegroundColor Yellow
Write-Host "   (Có thể mất 10-30 giây...)" -ForegroundColor Gray

dotnet ef database update 2>&1 | Tee-Object -Variable output

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "   ✓ Database đã được tạo thành công!" -ForegroundColor Green
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "         THIẾT LẬP HOÀN TẤT!          " -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Database: BubbleTeaDB" -ForegroundColor White
    Write-Host "Sản phẩm: 10 items" -ForegroundColor White
    Write-Host "Toppings: 8 items" -ForegroundColor White
    Write-Host ""
    Write-Host "Bạn có thể chạy ứng dụng bằng lệnh:" -ForegroundColor Yellow
    Write-Host "  dotnet run" -ForegroundColor Cyan
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "   ✗ Lỗi khi tạo database" -ForegroundColor Red
    Write-Host ""
    Write-Host "Các nguyên nhân có thể:" -ForegroundColor Yellow
    Write-Host "1. SQL Server chưa được cài đặt hoặc chưa chạy" -ForegroundColor White
    Write-Host "2. Connection string trong BubbleTeaDbContext.cs chưa đúng" -ForegroundColor White
    Write-Host ""
    Write-Host "Giải pháp:" -ForegroundColor Yellow
    Write-Host "→ Cài SQL Server Express:" -ForegroundColor White
    Write-Host "  https://www.microsoft.com/sql-server/sql-server-downloads" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "→ Hoặc sử dụng LocalDB (có sẵn với Visual Studio):" -ForegroundColor White
    Write-Host "  Mở file Data/BubbleTeaDbContext.cs" -ForegroundColor Cyan
    Write-Host "  Bỏ comment dòng LocalDB connection string" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "→ Sau đó chạy lại script này" -ForegroundColor White
    Write-Host ""
}

Write-Host "Nhấn phím bất kỳ để đóng..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
