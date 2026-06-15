using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfDem.DataBase;
using WpfDem.Helpers;
using WpfDem.Statics;
using WpfDem.ViewModels;

namespace WpfDem
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        private ShopDBEntities _db = new ShopDBEntities();
        private MessageHelper _mh = new MessageHelper();
        public OrderWindow()
        {
            InitializeComponent();
            LoadOrders();
            LoadUI();
        }

        private void LoadUI()
        {
            Users users = CurentSession.CurrentUser;
            if (users.RoleId == 1)
            {
                CreateButton.Visibility = Visibility.Visible;
            }
        }
        private void LoadOrders()
        {
            var orders = _db.Orders
            .Include("ProductInOrder")
            .Include("ProductInOrder.Products")
            .ToList()
            .Select(o => new OrderViewModel(o))
            .ToList();

            OrderList.ItemsSource = orders;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            new ProductWindow().Show();
            Close();

        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            new AddEditOrderWindow(null).Show();
            Close();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Users users = CurentSession.CurrentUser;
            if (users.RoleId != 1)
                return;
            int id = (int)(sender as Border).Tag;
            new AddEditOrderWindow(id).Show();
            Close();
        }
    }
}
