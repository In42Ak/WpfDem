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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfDem.Helpers;
using WpfDem.Statics;
using WpfDem.DataBase;

namespace WpfDem
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ShopDBEntities _db = new ShopDBEntities();
        private MessageHelper _mh = new MessageHelper();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginEnter.Text; ;
            string password = PasswordEnter.Password; 
            var users = _db.Users.Where(u => u.Login == login && u.Password == password).FirstOrDefault();

            if (users == null)
            {
                _mh.ShowError("Введен неправильнй логин или пароль!");
                return;
            }
            else
            {
                CurentSession.CurrentUser = users;
                new ProductWindow().Show();
                Close();
            }

        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new ProductWindow().Show();
            Close();
        }
    }
}
