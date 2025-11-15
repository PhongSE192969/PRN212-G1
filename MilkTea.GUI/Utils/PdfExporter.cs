using System;
using System.Collections.Generic;
using System.IO;
using MilkTea.DAL.Models;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace MilkTea.GUI.Utils
{
    public static class PdfExporter
    {
        public static void ExportInvoice(Invoice invoice, List<InvoiceDetail> details, string outputPath)
        {
            var doc = new PdfDocument();
            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var fontTitle = new XFont("Arial", 18, XFontStyle.Bold);
            var fontHeader = new XFont("Arial", 12, XFontStyle.Bold);
            var fontNormal = new XFont("Arial", 11);
            var fontSmall = new XFont("Arial", 10);

            double y = 40;
            double leftMargin = 50;
            double rightMargin = page.Width - 50;

            // Title
            gfx.DrawString("HÓA ĐƠN BÁN HÀNG", fontTitle, XBrushes.DarkBlue,
                new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
            y += 40;

            gfx.DrawString("TEAPOS - HỆ THỐNG BÁN TRÀ SỮA", fontSmall, XBrushes.Gray,
                new XRect(0, y, page.Width, 20), XStringFormats.TopCenter);
            y += 30;

            // Invoice info
            gfx.DrawString($"Mã hóa đơn: #{invoice.InvoiceId}", fontHeader, XBrushes.Black, leftMargin, y);
            y += 20;

            gfx.DrawString($"Ngày: {invoice.InvoiceDate:dd/MM/yyyy HH:mm:ss}", fontNormal, XBrushes.Black, leftMargin, y);
            y += 18;

            gfx.DrawString($"Thu ngân: {invoice.User?.Username ?? "N/A"}", fontNormal, XBrushes.Black, leftMargin, y);
            y += 18;

            gfx.DrawString($"Phương thức: {invoice.PaymentMethod}", fontNormal, XBrushes.Black, leftMargin, y);
            y += 18;

            gfx.DrawString($"Trạng thái: {invoice.Status}", fontNormal, XBrushes.Black, leftMargin, y);
            y += 30;

            // Line separator
            gfx.DrawLine(XPens.Black, leftMargin, y, rightMargin, y);
            y += 20;

            // Table header
            gfx.DrawString("Sản phẩm", fontHeader, XBrushes.Black, leftMargin, y);
            gfx.DrawString("SL", fontHeader, XBrushes.Black, rightMargin - 200, y);
            gfx.DrawString("Đơn giá", fontHeader, XBrushes.Black, rightMargin - 150, y);
            gfx.DrawString("Thành tiền", fontHeader, XBrushes.Black, rightMargin - 80, y, XStringFormats.TopRight);
            y += 20;

            gfx.DrawLine(XPens.Black, leftMargin, y, rightMargin, y);
            y += 15;

            // Invoice details
            foreach (var detail in details)
            {
                string productName = detail.Product?.ProductName ?? "N/A";
                if (detail.Topping != null)
                {
                    productName += $" + {detail.Topping.ToppingName}";
                }

                // Product name (wrap if too long)
                if (productName.Length > 30)
                {
                    productName = productName.Substring(0, 27) + "...";
                }

                gfx.DrawString(productName, fontNormal, XBrushes.Black, leftMargin, y);
                gfx.DrawString($"{detail.Quantity}", fontNormal, XBrushes.Black, rightMargin - 200, y);
                gfx.DrawString($"{detail.UnitPrice:N0}", fontNormal, XBrushes.Black, rightMargin - 150, y);
                gfx.DrawString($"{detail.Subtotal:N0}", fontNormal, XBrushes.Black, rightMargin - 80, y, XStringFormats.TopRight);
                y += 18;

                // Check if need new page
                if (y > page.Height - 150)
                {
                    page = doc.AddPage();
                    gfx = XGraphics.FromPdfPage(page);
                    y = 50;
                }
            }

            y += 10;
            gfx.DrawLine(XPens.Black, leftMargin, y, rightMargin, y);
            y += 20;

            // Totals
            decimal subtotal = invoice.TotalAmount;
            decimal vat = subtotal * 0.1m; // 10% VAT
            decimal discount = 0; // Discount not stored separately in current model
            decimal finalAmount = invoice.FinalAmount;

            gfx.DrawString("Tạm tính:", fontNormal, XBrushes.Black, rightMargin - 200, y);
            gfx.DrawString($"{subtotal:N0} đ", fontNormal, XBrushes.Black, rightMargin - 80, y, XStringFormats.TopRight);
            y += 18;

            gfx.DrawString("VAT (10%):", fontNormal, XBrushes.Black, rightMargin - 200, y);
            gfx.DrawString($"{vat:N0} đ", fontNormal, XBrushes.Black, rightMargin - 80, y, XStringFormats.TopRight);
            y += 18;

            if (discount > 0)
            {
                gfx.DrawString("Giảm giá:", fontNormal, XBrushes.Red, rightMargin - 200, y);
                gfx.DrawString($"-{discount:N0} đ", fontNormal, XBrushes.Red, rightMargin - 80, y, XStringFormats.TopRight);
                y += 18;
            }

            y += 5;
            gfx.DrawLine(XPens.Black, rightMargin - 250, y, rightMargin, y);
            y += 15;

            gfx.DrawString("TỔNG CỘNG:", fontHeader, XBrushes.DarkBlue, rightMargin - 200, y);
            gfx.DrawString($"{finalAmount:N0} đ", new XFont("Arial", 13, XFontStyle.Bold), 
                XBrushes.DarkBlue, rightMargin - 80, y, XStringFormats.TopRight);
            y += 40;

            // Footer
            y = page.Height - 80;
            gfx.DrawString("Cảm ơn quý khách! Hẹn gặp lại!", fontSmall, XBrushes.Gray,
                new XRect(0, y, page.Width, 20), XStringFormats.TopCenter);
            y += 15;
            gfx.DrawString("TeaPOS - Hotline: 1900-xxxx", fontSmall, XBrushes.Gray,
                new XRect(0, y, page.Width, 20), XStringFormats.TopCenter);

            // Save PDF
            doc.Save(outputPath);
        }

        public static string GetDefaultInvoicePath(int invoiceId)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"HoaDon_{invoiceId}_{DateTime.Now:ddMMyyyy_HHmmss}.pdf";
            return Path.Combine(desktopPath, fileName);
        }
    }
}
