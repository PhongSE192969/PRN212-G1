using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using BubbleTeaApp.Models;
using BubbleTeaApp.Services;

namespace BubbleTeaApp.ViewModels
{
    public class ToppingViewModel : INotifyPropertyChanged
    {
        private bool _isSelected;
        public Topping Topping { get; set; } = new Topping();
        
        public int Id => Topping.Id;
        public string Name => Topping.Name;
        public decimal Price => Topping.Price;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        private Product? _selectedProduct;
        private int _quantity = 1;
        private string _searchText = string.Empty;
        
        private readonly ProductService _productService;
        private readonly ToppingService _toppingService;
        private readonly OrderService _orderService;

        public ObservableCollection<Product> Products { get; set; }
        public ObservableCollection<Product> FilteredProducts { get; set; }
        public ObservableCollection<ToppingViewModel> AvailableToppings { get; set; }
        public ObservableCollection<CartItem> CartItems { get; set; }

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged();
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value > 0)
                {
                    _quantity = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged();
                FilterProducts();
            }
        }

        public decimal TotalAmount => CartItems.Sum(item => item.TotalPrice);

        public ICommand AddToCartCommand { get; }
        public ICommand RemoveFromCartCommand { get; }
        public ICommand IncreaseQuantityCommand { get; }
        public ICommand DecreaseQuantityCommand { get; }
        public ICommand CheckoutCommand { get; }
        public ICommand ClearCartCommand { get; }
        public ICommand SelectProductCommand { get; }

        public MainViewModel()
        {
            _productService = new ProductService();
            _toppingService = new ToppingService();
            _orderService = new OrderService();
            
            Products = new ObservableCollection<Product>();
            FilteredProducts = new ObservableCollection<Product>();
            AvailableToppings = new ObservableCollection<ToppingViewModel>();
            CartItems = new ObservableCollection<CartItem>();

            AddToCartCommand = new RelayCommand(AddToCart, CanAddToCart);
            RemoveFromCartCommand = new RelayCommand<CartItem>(RemoveFromCart);
            IncreaseQuantityCommand = new RelayCommand(IncreaseQuantity);
            DecreaseQuantityCommand = new RelayCommand(DecreaseQuantity);
            CheckoutCommand = new RelayCommand(Checkout, CanCheckout);
            ClearCartCommand = new RelayCommand(ClearCart);
            SelectProductCommand = new RelayCommand<Product>(SelectProduct);

            LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                // Load products từ database
                var products = _productService.GetAllProducts();
                Products.Clear();
                FilteredProducts.Clear();
                
                foreach (var product in products)
                {
                    Products.Add(product);
                    FilteredProducts.Add(product);
                }

                // Load toppings từ database
                var toppings = _toppingService.GetAllToppings();
                AvailableToppings.Clear();
                
                foreach (var topping in toppings)
                {
                    AvailableToppings.Add(new ToppingViewModel { Topping = topping });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu từ database: {ex.Message}\n\n" +
                    "Hãy chắc chắn rằng:\n" +
                    "1. SQL Server đang chạy\n" +
                    "2. Connection string trong BubbleTeaDbContext.cs đúng\n" +
                    "3. Đã chạy migration để tạo database", 
                    "Lỗi Database", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectProduct(Product? product)
        {
            if (product != null)
            {
                SelectedProduct = product;
            }
        }

        private void FilterProducts()
        {
            FilteredProducts.Clear();
            var filtered = string.IsNullOrWhiteSpace(SearchText)
                ? Products
                : Products.Where(p => p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            foreach (var product in filtered)
            {
                FilteredProducts.Add(product);
            }
        }

        private bool CanAddToCart(object? parameter)
        {
            return SelectedProduct != null && Quantity > 0;
        }

        private void AddToCart(object? parameter)
        {
            if (SelectedProduct == null) return;

            var selectedToppings = AvailableToppings
                .Where(t => t.IsSelected)
                .Select(t => t.Topping)
                .ToList();

            var cartItem = new CartItem
            {
                Product = SelectedProduct,
                Quantity = Quantity,
                SelectedToppings = selectedToppings
            };

            CartItems.Add(cartItem);
            OnPropertyChanged(nameof(TotalAmount));

            // Reset
            foreach (var topping in AvailableToppings)
            {
                topping.IsSelected = false;
            }
            Quantity = 1;

            MessageBox.Show($"Đã thêm {SelectedProduct.Name} vào giỏ hàng!", "Thành công", 
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void RemoveFromCart(CartItem? item)
        {
            if (item != null)
            {
                CartItems.Remove(item);
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        private void IncreaseQuantity(object? parameter)
        {
            Quantity++;
        }

        private void DecreaseQuantity(object? parameter)
        {
            if (Quantity > 1)
                Quantity--;
        }

        private bool CanCheckout(object? parameter)
        {
            return CartItems.Count > 0;
        }

        private void Checkout(object? parameter)
        {
            var message = $"Tổng tiền: {TotalAmount:N0} VNĐ\n\nChi tiết đơn hàng:\n";
            foreach (var item in CartItems)
            {
                message += $"\n• {item.Product.Name} x{item.Quantity}";
                if (item.SelectedToppings.Count > 0)
                {
                    message += $"\n  Topping: {item.ToppingsDisplay}";
                }
                message += $"\n  Thành tiền: {item.TotalPrice:N0} VNĐ\n";
            }

            var result = MessageBox.Show(message + "\n\nXác nhận thanh toán?", "Thanh toán",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Tạo đơn hàng mới
                    var order = new Order
                    {
                        OrderDate = DateTime.Now,
                        TotalAmount = TotalAmount,
                        VAT = 0,
                        Discount = 0,
                        FinalAmount = TotalAmount,
                        Status = "Completed",
                        PaymentMethod = "Cash",
                        UserId = null // Có thể thêm UserId nếu có đăng nhập
                    };

                    // Thêm chi tiết đơn hàng - Mỗi sản phẩm 1 dòng
                    foreach (var cartItem in CartItems)
                    {
                        // Thêm sản phẩm chính
                        var orderDetail = new OrderDetail
                        {
                            ProductId = cartItem.Product.Id,
                            ToppingId = null,
                            Quantity = cartItem.Quantity,
                            UnitPrice = cartItem.Product.Price,
                            TotalPrice = cartItem.Product.Price * cartItem.Quantity
                        };
                        order.OrderDetails.Add(orderDetail);

                        // Thêm từng topping như một dòng riêng
                        foreach (var topping in cartItem.SelectedToppings)
                        {
                            var toppingDetail = new OrderDetail
                            {
                                ProductId = cartItem.Product.Id,
                                ToppingId = topping.Id,
                                Quantity = cartItem.Quantity,
                                UnitPrice = topping.Price,
                                TotalPrice = topping.Price * cartItem.Quantity
                            };
                            order.OrderDetails.Add(toppingDetail);
                        }
                    }

                    // Lưu vào database
                    int orderId = _orderService.CreateOrder(order);

                    MessageBox.Show($"Thanh toán thành công! Mã đơn hàng: #{orderId}\nCảm ơn quý khách!", 
                        "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                    ClearCart(null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi lưu đơn hàng: {ex.Message}\n\nChi tiết: {ex.InnerException?.Message}", "Lỗi",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearCart(object? parameter)
        {
            CartItems.Clear();
            OnPropertyChanged(nameof(TotalAmount));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool>? _canExecute;

        public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object? parameter) => _execute(parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T?> _execute;
        private readonly Func<T?, bool>? _canExecute;

        public RelayCommand(Action<T?> execute, Func<T?, bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => _canExecute == null || _canExecute((T?)parameter);

        public void Execute(object? parameter) => _execute((T?)parameter);

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
