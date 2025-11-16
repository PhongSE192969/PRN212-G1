using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using MilkTea.BLL.Services;
using MilkTea.DAL.Data;
using MilkTea.DAL.Models;

namespace MilkTea.GUI.Views
{
    public partial class InvoiceDetailWindow : Window
    {
        private readonly InvoiceService _invoiceService;
        private readonly TeaPOSDbContext _context;
        private Invoice? _selectedInvoice;
        private ObservableCollection<InvoiceDetail> _invoiceDetails;

        public InvoiceDetailWindow()
        {
            InitializeComponent();

            _invoiceService = new InvoiceService();
            _context = new TeaPOSDbContext();
            _invoiceDetails = new ObservableCollection<InvoiceDetail>();
            lstInvoiceDetails.ItemsSource = _invoiceDetails;


            dpFromDate.SelectedDate = DateTime.Today.AddDays(-30);
            dpToDate.SelectedDate = DateTime.Today;

            LoadInvoices();
            LoadProducts();
            LoadToppings();
        }

        private void LoadInvoices()
        {
            try
            {
                var invoices = _invoiceService.GetRecentInvoices(100);
                dgInvoices.ItemsSource = invoices.OrderByDescending(i => i.InvoiceDate).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải hóa đơn:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                var products = _context.Products.ToList();
                cmbProducts.ItemsSource = products;
                cmbProducts.DisplayMemberPath = "ProductName";
                cmbProducts.SelectedValuePath = "ProductId";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải sản phẩm:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadToppings()
        {
            try
            {
                var toppings = _context.Toppings.ToList();
                cmbToppings.ItemsSource = toppings;
                cmbToppings.DisplayMemberPath = "ToppingName";
                cmbToppings.SelectedValuePath = "ToppingId";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải topping:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgInvoices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgInvoices.SelectedItem is Invoice invoice)
            {
                _selectedInvoice = invoice;
                LoadInvoiceDetails(invoice.InvoiceId);
                UpdateSummary();

                // Set status in ComboBox
                if (!string.IsNullOrEmpty(invoice.Status))
                {
                    foreach (ComboBoxItem item in cmbStatus.Items)
                    {
                        if (item.Content.ToString() == invoice.Status)
                        {
                            cmbStatus.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
        }

        private void LoadInvoiceDetails(int invoiceId)
        {
            try
            {
                var invoice = _invoiceService.GetInvoiceById(invoiceId);
                if (invoice != null)
                {
                    _invoiceDetails.Clear();
                    foreach (var detail in invoice.InvoiceDetails)
                    {
                        _invoiceDetails.Add(detail);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải chi tiết hóa đơn:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnAddDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedInvoice == null)
                {
                    MessageBox.Show("Vui lòng chọn một hóa đơn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cmbProducts.SelectedValue == null)
                {
                    MessageBox.Show("Vui lòng chọn một sản phẩm!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Số lượng không hợp lệ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int productId = (int)cmbProducts.SelectedValue;
                int? toppingId = cmbToppings.SelectedValue != null ? (int?)cmbToppings.SelectedValue : null;

                var product = _context.Products.Find(productId);
                if (product == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                var existingDetail = _context.InvoiceDetails.FirstOrDefault(d =>
                    d.InvoiceId == _selectedInvoice.InvoiceId &&
                    d.ProductId == productId &&
                    d.ToppingId == toppingId);

                if (existingDetail != null)
                {

                    existingDetail.Quantity += quantity;
                    _context.SaveChanges();


                    var uiDetail = _invoiceDetails.FirstOrDefault(d =>
                        d.InvoiceId == existingDetail.InvoiceId &&
                        d.ProductId == existingDetail.ProductId &&
                        d.ToppingId == existingDetail.ToppingId);

                    if (uiDetail != null)
                    {
                        uiDetail.Quantity = existingDetail.Quantity;
                    }

                    MessageBox.Show($"Đã cập nhật số lượng cho {product.ProductName} thành {existingDetail.Quantity}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    // New product, add it
                    var detail = new InvoiceDetail
                    {
                        InvoiceId = _selectedInvoice.InvoiceId,
                        ProductId = productId,
                        ToppingId = toppingId,
                        Quantity = quantity,
                        UnitPrice = product.Price
                    };

                    _context.InvoiceDetails.Add(detail);
                    _context.SaveChanges();

                    _invoiceDetails.Add(detail);

                    MessageBox.Show("Đã thêm chi tiết hóa đơn thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                }


                var updatedInvoice = _invoiceService.GetInvoiceById(_selectedInvoice.InvoiceId);
                if (updatedInvoice != null)
                {
                    _selectedInvoice = updatedInvoice;
                    _invoiceDetails.Clear();
                    foreach (var detail in updatedInvoice.InvoiceDetails)
                    {
                        _invoiceDetails.Add(detail);
                    }
                }

                UpdateSummary();

                // Reset form
                cmbProducts.SelectedValue = null;
                cmbToppings.SelectedValue = null;
                txtQuantity.Text = "1";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm chi tiết:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRemoveDetail_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button btn && btn.Tag is InvoiceDetail detail)
                {
                    var result = MessageBox.Show($"Bạn muốn làm gì:\n\n[Yes] Xóa toàn bộ mục\n[No] Giảm số lượng", "Chọn hành động", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Delete entire detail
                        var dbDetail = _context.InvoiceDetails.FirstOrDefault(d =>
                            d.InvoiceId == detail.InvoiceId &&
                            d.ProductId == detail.ProductId &&
                            d.ToppingId == detail.ToppingId);

                        if (dbDetail != null)
                        {
                            _context.InvoiceDetails.Remove(dbDetail);
                            _context.SaveChanges();

                            _invoiceDetails.Remove(detail);

                            MessageBox.Show("Đã xóa hoàn toàn chi tiết hóa đơn!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else if (result == MessageBoxResult.No)
                    {

                        var reduceWindow = new QuantityInputWindow(detail.Quantity);
                        if (reduceWindow.ShowDialog() == true)
                        {
                            int quantityToReduce = reduceWindow.QuantityValue;

                            var dbDetail = _context.InvoiceDetails.FirstOrDefault(d =>
                                d.InvoiceId == detail.InvoiceId &&
                                d.ProductId == detail.ProductId &&
                                d.ToppingId == detail.ToppingId);

                            if (dbDetail != null)
                            {
                                if (quantityToReduce >= dbDetail.Quantity)
                                {

                                    _context.InvoiceDetails.Remove(dbDetail);
                                    _context.SaveChanges();
                                    _invoiceDetails.Remove(detail);

                                    MessageBox.Show($"Đã xóa mục (giảm {quantityToReduce})!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                                else
                                {

                                    dbDetail.Quantity -= quantityToReduce;
                                    _context.SaveChanges();


                                    detail.Quantity = dbDetail.Quantity;
                                    MessageBox.Show($"Đã giảm số lượng {quantityToReduce}. Còn lại: {dbDetail.Quantity}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi sửa đổi chi tiết:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }


            if (_selectedInvoice != null)
            {
                var updatedInvoice = _invoiceService.GetInvoiceById(_selectedInvoice.InvoiceId);
                if (updatedInvoice != null)
                {
                    _selectedInvoice = updatedInvoice;
                    _invoiceDetails.Clear();
                    foreach (var detail2 in updatedInvoice.InvoiceDetails)
                    {
                        _invoiceDetails.Add(detail2);
                    }
                }
            }

            UpdateSummary();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var searchText = txtSearchInvoice.Text.Trim();
                var fromDate = dpFromDate.SelectedDate ?? DateTime.Today.AddDays(-30);
                var toDate = dpToDate.SelectedDate ?? DateTime.Today;

                var invoices = _invoiceService.GetInvoicesByDateRange(fromDate, toDate);

                if (!string.IsNullOrEmpty(searchText))
                {
                    if (int.TryParse(searchText, out int invoiceId))
                    {
                        invoices = invoices.Where(i => i.InvoiceId == invoiceId).ToList();
                    }
                    else
                    {
                        invoices = invoices.Where(i => i.User != null && i.User.FullName.Contains(searchText, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                }

                dgInvoices.ItemsSource = invoices.OrderByDescending(i => i.InvoiceDate).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            txtSearchInvoice.Clear();
            dpFromDate.SelectedDate = DateTime.Today.AddDays(-60);
            dpToDate.SelectedDate = DateTime.Today;
            LoadInvoices();
            _invoiceDetails.Clear();
            _selectedInvoice = null;
            dgInvoices.SelectedItem = null;
            UpdateSummary();
        }

        private void UpdateSummary()
        {
            if (_selectedInvoice == null)
            {
                txtSummarySubtotal.Text = "0 d";
                txtSummaryVAT.Text = "0 d";
                txtSummaryDiscount.Text = "0 d";
                txtSummaryTotal.Text = "0 d";
                return;
            }

            decimal subtotal = _invoiceDetails.Sum(d => d.Subtotal);
            decimal vat = subtotal * 0.1m;
            decimal discount = _selectedInvoice.Discount;
            decimal finalAmount = subtotal + vat - discount;

            txtSummarySubtotal.Text = $"{subtotal:N0} d";
            txtSummaryVAT.Text = $"{vat:N0} d";
            txtSummaryDiscount.Text = $"{discount:N0} d";
            txtSummaryTotal.Text = $"{finalAmount:N0} d";


            if (_selectedInvoice != null)
            {
                _selectedInvoice.FinalAmount = finalAmount;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedInvoice == null)
                {
                    MessageBox.Show("Vui lòng chọn một hóa đơn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                decimal subtotal = _invoiceDetails.Sum(d => d.Subtotal);
                decimal vat = subtotal * 0.1m;
                decimal discount = _selectedInvoice.Discount;
                decimal finalAmount = subtotal + vat - discount;


                var sql = @"
                    UPDATE Invoices 
                    SET TotalAmount = {0}, 
                        VAT = {1}, 
                        Discount = {2}
                    WHERE InvoiceID = {3}";

                _context.Database.ExecuteSqlRaw(
                    sql,
                    subtotal,
                    vat,
                    discount,
                    _selectedInvoice.InvoiceId
                );

                LoadInvoices();

                MessageBox.Show($"Đã lưu thay đổi thành công!\nTổng mới: {finalAmount:N0} d", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu dữ liệu:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnDeleteInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedInvoice == null)
                {
                    MessageBox.Show("Vui lòng chọn một hóa đơn để xóa!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa hóa đơn #{_selectedInvoice.InvoiceId}?\n\nHành động này không thể hoàn tác!", "Xác nhận xóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    var invoiceToDelete = _context.Invoices.Find(_selectedInvoice.InvoiceId);
                    if (invoiceToDelete != null)
                    {
                        _context.Invoices.Remove(invoiceToDelete);
                        _context.SaveChanges();

                        LoadInvoices();
                        _invoiceDetails.Clear();
                        UpdateSummary();

                        MessageBox.Show("Đã xóa hóa đơn thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa hóa đơn:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnApplyDiscount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedInvoice == null)
                {
                    MessageBox.Show("Vui lòng chọn một hóa đơn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!decimal.TryParse(txtDiscountInput.Text, out decimal discountAmount) || discountAmount < 0)
                {
                    MessageBox.Show("Số tiền giảm giá không hợp lệ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }


                _selectedInvoice.Discount = discountAmount;


                UpdateSummary();

                MessageBox.Show($"Đã áp dụng giảm giá: {discountAmount:N0} d", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi áp dụng giảm giá:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnUpdateStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_selectedInvoice == null)
                {
                    MessageBox.Show("Vui lòng chọn một hóa đơn!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cmbStatus.SelectedItem == null)
                {
                    MessageBox.Show("Vui lòng chọn một trạng thái!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string newStatus = (cmbStatus.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";

                if (string.IsNullOrEmpty(newStatus))
                {
                    MessageBox.Show("Trạng thái được chọn không hợp lệ!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Use raw SQL to update status to avoid trigger conflicts
                var sql = "UPDATE Invoices SET Status = {0} WHERE InvoiceID = {1}";

                _context.Database.ExecuteSqlRaw(sql, newStatus, _selectedInvoice.InvoiceId);

                // Reload invoices to reflect changes
                LoadInvoices();

                MessageBox.Show($"Trạng thái đã được cập nhật thành: {newStatus}", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cập nhật trạng thái:\n{ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}