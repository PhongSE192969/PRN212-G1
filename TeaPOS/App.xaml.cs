using System.Configuration;
using System.Data;
using System.Windows;
using TeaPOS.Application.Checkout;
using TeaPOS.Infrastructure.Services;
using TeaPOS.Presentation.Wpf.Features.Checkout;

namespace TeaPOS.Presentation.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public static ICheckout Checkout { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var discountSvc = new DiscountService();
            var vm = new CheckoutViewModel(discountSvc);
            Checkout = new CheckoutFacade(vm);

            // (tùy chọn) mở luôn UI phần 4:
            // new Features.Checkout.CheckoutView(vm).Show();
        }
    }

}
