using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
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

namespace WpfDem
{
    public partial class AddEditOrderWindow : Window
    {
        private ShopDBEntities _db = new ShopDBEntities();
        private MessageHelper _mh = new MessageHelper();

        private bool _isEditing;
        private Orders _orders;

        public AddEditOrderWindow(int? id)
        {
            InitializeComponent();

            if (id == null)
            {
                _isEditing = false;
            }
            else
            {
                _isEditing = true;
                _orders = _db.Orders.Find(id);
            }

            LoadData();
        }

        private void LoadData()
        {
            var pickPoints = _db.PickPoint.ToList();
            var statuses = _db.Status.ToList();
            var users = _db.Users.ToList();
            var products = _db.Products.ToList();

            OrderPickPoint.ItemsSource = pickPoints;
            OrderPickPoint.DisplayMemberPath = "City";
            OrderPickPoint.SelectedValuePath = "Id";
            OrderPickPoint.SelectedIndex = 0;

            OrderStatus.ItemsSource = statuses;
            OrderStatus.DisplayMemberPath = "Name";
            OrderStatus.SelectedValuePath = "Id";
            OrderStatus.SelectedIndex = 0;

            OrderUser.ItemsSource = users;
            OrderUser.DisplayMemberPath = "Name";
            OrderUser.SelectedValuePath = "Id";
            OrderUser.SelectedIndex = 0;

            OrderProduct.ItemsSource = products;
            OrderProduct.DisplayMemberPath = "Article";
            OrderProduct.SelectedValuePath = "Id";
            OrderProduct.SelectedIndex = 0;

            if (_isEditing == true)
            {
                DeleteButton.Visibility = Visibility.Visible;
                FillData();
            }
        }

        private void FillData()
        {
            OrderDate.SelectedDate = _orders.OrderDate;
            OrderDelivery.SelectedDate = _orders.OrderDelivery;
            OrderCode.Text = _orders.Code;
            OrderPickPoint.SelectedValue = _orders.PickUpPointId;
            OrderStatus.SelectedValue = _orders.StatusId;
            OrderUser.SelectedValue = _orders.UserId;

            var firstProduct = _orders.ProductInOrder.FirstOrDefault();
            if (firstProduct != null)
                OrderProduct.SelectedValue = firstProduct.ProductId;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            new OrderWindow().Show();
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;
            if (_isEditing == true)
                UpdateOrder();
            else
                CreateOrder();
        }

        private void CreateOrder()
        {
            Orders orders = new Orders();

            orders.OrderDate = OrderDate.SelectedDate.Value;
            orders.OrderDelivery = OrderDelivery.SelectedDate.Value;
            orders.Code = OrderCode.Text;
            orders.PickUpPointId = (int)OrderPickPoint.SelectedValue;
            orders.StatusId = (int)OrderStatus.SelectedValue;
            orders.UserId = (int)OrderUser.SelectedValue;

            try
            {
                _db.Orders.AddOrUpdate(orders);
                _db.SaveChanges();

                var productInOrder = new ProductInOrder();
                productInOrder.OrderId = orders.Id;
                productInOrder.ProductId = (int)OrderProduct.SelectedValue;
                productInOrder.Amount = 1;
                _db.ProductInOrder.AddOrUpdate(productInOrder);
                _db.SaveChanges();

                _mh.ShowInfo("Заказ успешно создан!");
                CancelButton_Click(null, null);
            }
            catch (Exception ex)
            {
                _mh.ShowError(ex.Message);
            }
        }

        private void UpdateOrder()
        {
            _orders.OrderDate = OrderDate.SelectedDate.Value;
            _orders.OrderDelivery = OrderDelivery.SelectedDate.Value;
            _orders.Code = OrderCode.Text;
            _orders.PickUpPointId = (int)OrderPickPoint.SelectedValue;
            _orders.StatusId = (int)OrderStatus.SelectedValue;
            _orders.UserId = (int)OrderUser.SelectedValue;

            try
            {
                _db.Orders.AddOrUpdate(_orders);
                _db.SaveChanges();
                _mh.ShowInfo("Заказ успешно отредактирован!");
                CancelButton_Click(null, null);
            }
            catch (Exception ex)
            {
                _mh.ShowError(ex.Message);
            }
        }

        private bool ValidateInput()
        {
            StringBuilder errors = new StringBuilder();

            if (OrderDate.SelectedDate == null)
                errors.AppendLine("Поле дата заказа не заполнено!");

            if (OrderDelivery.SelectedDate == null)
                errors.AppendLine("Поле дата доставки не заполнено!");

            if (string.IsNullOrWhiteSpace(OrderCode.Text))
                errors.AppendLine("Поле код заказа не заполнено!");

            if (errors.Length > 0)
            {
                _mh.ShowError(errors.ToString());
                return false;
            }
            return true;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var productInOrders = _db.ProductInOrder.Where(p => p.OrderId == _orders.Id).ToList();
            foreach (var item in productInOrders)
                _db.ProductInOrder.Remove(item);

            _db.Orders.Remove(_orders);
            _db.SaveChanges();
            _mh.ShowInfo("Заказ успешно удалён!");
            CancelButton_Click(null, null);

        }
    }
}