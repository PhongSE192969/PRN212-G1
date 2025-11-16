using System.Windows;
using System.Windows.Controls;
using MilkTea.DAL.Models;

namespace MilkTea.GUI.Views
{
    public partial class AddToCartDialog : Window
    {
        private readonly Product _product;
        private readonly List<Topping> _toppings;
        private int _quantity = 1;
        private Topping? _selectedTopping;

        public int SelectedProductId => _product.ProductId;
        public int Quantity => _quantity;
        public int? SelectedToppingId => _selectedTopping?.ToppingId;

        public AddToCartDialog(Product product, List<Topping> toppings)
        {
            InitializeComponent();
            
            _product = product;
            _toppings = toppings;
            
            InitializeUI();
        }

        private void InitializeUI()
        {
            txtProductName.Text = _product.ProductName;
            txtProductPrice.Text = $"Giá: {_product.Price:N0} đ";
            
            // Add "Không chọn" option
            var toppingOptions = new List<Topping> { new Topping { ToppingId = 0, ToppingName = "Không chọn", Price = 0 } };
            toppingOptions.AddRange(_toppings);
            
            cboTopping.ItemsSource = toppingOptions;
            cboTopping.SelectedIndex = 0;
            
            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            decimal productPrice = _product.Price;
            decimal toppingPrice = _selectedTopping?.Price ?? 0;
            decimal total = (productPrice + toppingPrice) * _quantity;
            
            txtTotalPrice.Text = $"{total:N0} đ";
        }

        private void BtnIncrease_Click(object sender, RoutedEventArgs e)
        {
            _quantity++;
            txtQuantity.Text = _quantity.ToString();
            UpdateTotalPrice();
        }

        private void BtnDecrease_Click(object sender, RoutedEventArgs e)
        {
            if (_quantity > 1)
            {
                _quantity--;
                txtQuantity.Text = _quantity.ToString();
                UpdateTotalPrice();
            }
        }

        private void CboTopping_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboTopping.SelectedItem is Topping topping && topping.ToppingId != 0)
            {
                _selectedTopping = topping;
            }
            else
            {
                _selectedTopping = null;
            }
            
            UpdateTotalPrice();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
