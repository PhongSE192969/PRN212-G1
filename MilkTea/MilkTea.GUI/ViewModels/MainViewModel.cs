using System.Collections.ObjectModel;
using MilkTea.DAL.Models;
using MilkTea.BLL.Services;
using MilkTea.GUI.Utils;

namespace MilkTea.GUI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ProductService _productService;
        private readonly ToppingService _toppingService;
        private readonly DiscountService _discountService;
        private readonly InvoiceService _invoiceService;
        
        private ObservableCollection<Category> _categories;
        private ObservableCollection<Product> _products;
        private ObservableCollection<Topping> _toppings;
        private ObservableCollection<CartItem> _cartItems;
        
        private Category? _selectedCategory;
        private Product? _selectedProduct;
        private string _discountCode = string.Empty;
        private Discount? _appliedDiscount;
        
        public MainViewModel()
        {
            _productService = new ProductService();
            _toppingService = new ToppingService();
            _discountService = new DiscountService();
            _invoiceService = new InvoiceService();
            
            _categories = new ObservableCollection<Category>();
            _products = new ObservableCollection<Product>();
            _toppings = new ObservableCollection<Topping>();
            _cartItems = new ObservableCollection<CartItem>();
            
            LoadData();
        }
        
        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }
        
        public ObservableCollection<Product> Products
        {
            get => _products;
            set => SetProperty(ref _products, value);
        }
        
        public ObservableCollection<Topping> Toppings
        {
            get => _toppings;
            set => SetProperty(ref _toppings, value);
        }
        
        public ObservableCollection<CartItem> CartItems
        {
            get => _cartItems;
            set => SetProperty(ref _cartItems, value);
        }
        
        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                SetProperty(ref _selectedCategory, value);
                LoadProductsByCategory();
            }
        }
        
        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }
        
        public string DiscountCode
        {
            get => _discountCode;
            set => SetProperty(ref _discountCode, value);
        }
        
        public Discount? AppliedDiscount
        {
            get => _appliedDiscount;
            set
            {
                SetProperty(ref _appliedDiscount, value);
                OnPropertyChanged(nameof(TotalAmount));
                OnPropertyChanged(nameof(VAT));
                OnPropertyChanged(nameof(DiscountAmount));
                OnPropertyChanged(nameof(FinalAmount));
            }
        }
        
        // Calculated properties
        public decimal TotalAmount => CartItems.Sum(item => item.Subtotal);
        public decimal VAT => TotalAmount * AppConfig.VATRate;
        public decimal DiscountAmount => AppliedDiscount != null 
            ? TotalAmount * (AppliedDiscount.Percentage / 100) 
            : 0;
        public decimal FinalAmount => TotalAmount + VAT - DiscountAmount;
        
        private void LoadData()
        {
            // Load categories
            var categories = _productService.GetAllCategories();
            Categories.Clear();
            foreach (var cat in categories)
                Categories.Add(cat);
            
            // Load all products initially
            var products = _productService.GetAllProducts();
            Products.Clear();
            foreach (var prod in products)
                Products.Add(prod);
            
            // Load toppings
            var toppings = _toppingService.GetAllToppings();
            Toppings.Clear();
            foreach (var top in toppings)
                Toppings.Add(top);
        }
        
        private void LoadProductsByCategory()
        {
            Products.Clear();
            
            if (SelectedCategory == null)
            {
                var allProducts = _productService.GetAllProducts();
                foreach (var prod in allProducts)
                    Products.Add(prod);
            }
            else
            {
                var products = _productService.GetProductsByCategory(SelectedCategory.CategoryId);
                foreach (var prod in products)
                    Products.Add(prod);
            }
        }
        
        public void AddToCart(int productId, int quantity, int? toppingId = null)
        {
            var product = _productService.GetProductById(productId);
            if (product == null) return;
            
            Topping? topping = null;
            if (toppingId.HasValue)
            {
                topping = _toppingService.GetToppingById(toppingId.Value);
            }
            
            // Check if item already exists in cart
            var existingItem = CartItems.FirstOrDefault(item => 
                item.ProductId == productId && item.ToppingId == toppingId);
            
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var cartItem = new CartItem
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    ProductPrice = product.Price,
                    Quantity = quantity,
                    ToppingId = topping?.ToppingId,
                    ToppingName = topping?.ToppingName,
                    ToppingPrice = topping?.Price ?? 0
                };
                
                CartItems.Add(cartItem);
            }
            
            UpdateTotals();
        }
        
        public void RemoveFromCart(CartItem item)
        {
            CartItems.Remove(item);
            UpdateTotals();
        }
        
        public void ClearCart()
        {
            CartItems.Clear();
            AppliedDiscount = null;
            DiscountCode = string.Empty;
            UpdateTotals();
        }
        
        public bool ApplyDiscountCode()
        {
            if (string.IsNullOrWhiteSpace(DiscountCode))
            {
                AppliedDiscount = null;
                return false;
            }
            
            var discount = _discountService.GetValidDiscountByCode(DiscountCode);
            if (discount == null)
            {
                AppliedDiscount = null;
                return false;
            }
            
            AppliedDiscount = discount;
            return true;
        }
        
        public int Checkout(string paymentMethod, string? qrCodeData = null)
        {
            if (CartItems.Count == 0)
                throw new InvalidOperationException("Giỏ hàng trống!");
            
            var invoiceId = _invoiceService.CreateInvoice(
                CartItems.ToList(),
                AppliedDiscount?.DiscountId,
                paymentMethod,
                qrCodeData
            );
            
            ClearCart();
            return invoiceId;
        }
        
        private void UpdateTotals()
        {
            OnPropertyChanged(nameof(TotalAmount));
            OnPropertyChanged(nameof(VAT));
            OnPropertyChanged(nameof(DiscountAmount));
            OnPropertyChanged(nameof(FinalAmount));
        }
    }
}
