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
    /// Логика взаимодействия для ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        private ShopDBEntities _db = new ShopDBEntities();
        private MessageHelper _mh = new MessageHelper();
        private List<ProductViewModel> _productViewModels = new List<ProductViewModel>();

        private string[] _sortingTypes = new string[]
        {
        "По умолчанию",
        "По убыванию",
        "По возрастанию"
        };
        private List<string> _filteringTypes = new List<string>()
    {
        "Все поставщики"
    };


        public ProductWindow()
        {
            InitializeComponent();
            LoadData();
            ApplyAll();
            LoadUI();
        }

        private void LoadUI()
        {
            Users users = CurentSession.CurrentUser;
            if (users == null || users.RoleId == 3)
            {
                AdminPanel.Visibility = Visibility.Collapsed;
                OrdersButton.Visibility = Visibility.Collapsed;
            }
            else if (users.RoleId == 1)
                    {
                CreateButton.Visibility = Visibility.Visible;
            }
        }

        private void LoadData()
        {
            SortingCombobox.ItemsSource = _sortingTypes;
            SortingCombobox.SelectedIndex = 0;

            _filteringTypes.Clear();
            _filteringTypes.Add("Все поставщики");
            var provider = _db.Provider.ToList();
            foreach (var p in provider)
                _filteringTypes.Add(p.Name);

            FilteringCombobox.ItemsSource = null;
            FilteringCombobox.ItemsSource = _filteringTypes;
            FilteringCombobox.SelectedIndex = 0;

            Users users = CurentSession.CurrentUser;
            if (users != null)
            {
                FullUserName.Text = users.FullName;
                FullUserRole.Text = users.Role.Name;
            }
        }

        private void ApplyAll()
        {
            
            var query = _db.Products.AsQueryable();

            
            string searchText = SearchingTextbox?.Text?.ToLower() ?? "";
            if (!string.IsNullOrEmpty(searchText))
            {

                query = query.Where(p =>
                    p.Category.Name.ToLower().Contains(searchText) ||
                    p.Name.ToLower().Contains(searchText) ||
                    p.Description.ToLower().Contains(searchText) ||
                    p.Provider.Name.ToLower().Contains(searchText) ||
                    p.Producer.Name.ToLower().Contains(searchText) ||
                    p.Unit.Name.ToLower().Contains(searchText));
            }

            
            string filterText = FilteringCombobox?.SelectedValue?.ToString() ?? "Все поставщики";
            if (filterText != "Все поставщики")
            {
                query = query.Where(p => p.Provider.Name == filterText);
            }

            _productViewModels = query
                .ToList()
                .Select(p => new ProductViewModel(p))
                .ToList();
            
            int sortingType = SortingCombobox?.SelectedIndex ?? 0;
            if (sortingType == 1)
                _productViewModels = _productViewModels.OrderByDescending(p => p.StockQuantity).ToList();
            else if (sortingType == 2)
                _productViewModels = _productViewModels.OrderBy(p => p.StockQuantity).ToList();

            
            ProductList.ItemsSource = _productViewModels;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            CurentSession.CurrentUser = null;
            new MainWindow().Show();
            Close();
        }

        private void SearchingTextbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyAll();
        }

        private void SortingCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyAll();
        }

        private void FilteringCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyAll();
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            new AddEditProductWindow(null).Show();
            Close() ;

        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Users users = CurentSession.CurrentUser;
            if (users.RoleId != 1) 
                return;
            int id = (int)(sender as Border).Tag;
            
            new AddEditProductWindow(id).Show();
            Close();

        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            new OrderWindow().Show();
            Close();
        }

       
    }
}
