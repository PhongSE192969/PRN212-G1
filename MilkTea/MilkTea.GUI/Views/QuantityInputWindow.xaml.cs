using System.Windows;

namespace MilkTea.GUI.Views
{
    public partial class QuantityInputWindow : Window
    {
 public int QuantityValue { get; private set; }
        private int _currentQuantity;

        public QuantityInputWindow(int currentQuantity)
        {
            InitializeComponent();
    _currentQuantity = currentQuantity;
            txtCurrentQuantity.Text = $"So luong hien tai: {currentQuantity}";
            QuantityValue = 1;
        }

      private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtQuantityReduce.Text, out int quantity) && quantity > 0)
     {
     QuantityValue = quantity;
     this.DialogResult = true;
  this.Close();
            }
 else
     {
    MessageBox.Show("Vui long nhap so luong hop le (lon hon 0)!", "Loi",
         MessageBoxButton.OK, MessageBoxImage.Warning);
     }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
  {
            this.DialogResult = false;
 this.Close();
        }
    }
}
