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

    public partial class AddEditProductWindow : Window
    {
        private ShopDBEntities _db = new ShopDBEntities();
        private MessageHelper _mh = new MessageHelper();
        private string _photoPath;

        private bool _isEditing;
        private Products _products;
        public AddEditProductWindow(int? id)
        {
            InitializeComponent();

            if (id == null)
            {
                _isEditing = false;
            }
            else
            {
                _isEditing = true;
                _products = _db.Products.Find(id);
            }
            
            LoadData();

        }

        private void LoadData()
        {
            var units = _db.Unit.ToList();
            var providers = _db.Provider.ToList();
            var producers = _db.Producer.ToList();
            var categories = _db.Category.ToList();
            ProductUnit.ItemsSource = units;
            ProductUnit.DisplayMemberPath = "Name";
            ProductUnit.SelectedValuePath = "Id";
            ProductUnit.SelectedIndex = 0;

            ProductProvider.ItemsSource = providers;
            ProductProvider.DisplayMemberPath = "Name";
            ProductProvider.SelectedValuePath = "Id";
            ProductProvider.SelectedIndex = 0;

            ProductProducer.ItemsSource = producers;
            ProductProducer.DisplayMemberPath = "Name";
            ProductProducer.SelectedValuePath = "Id";
            ProductProducer.SelectedIndex = 0;

            ProductCategory.ItemsSource = categories;
            ProductCategory.DisplayMemberPath = "Name";
            ProductCategory.SelectedValuePath = "Id";
            ProductCategory.SelectedIndex = 0;

            if (_isEditing == true)
            {
                FillData();
                DeleteButton.Visibility = Visibility.Visible;
            }


        }

        private void FillData()
        {
            ProductArticle.Text = _products.Article;
            ProductName.Text = _products.Name;
            ProductCost.Text = _products.Price.ToString();
            ProductDiscoubt.Text = _products.Discount.ToString();
            ProductInStock.Text = _products.StockQuantity.ToString();
            ProductDescription.Text = _products.Description;
            _photoPath = _products.Photo;
            if (!string.IsNullOrEmpty(_photoPath))
            {
                string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _photoPath);
                if (System.IO.File.Exists(fullPath))
                    ProductPhotoPreview.Source = new BitmapImage(new Uri(fullPath));
            }
            ProductUnit.SelectedValue = _products.UnitId;
            ProductProvider.SelectedValue = _products.ProviderId;
            ProductProducer.SelectedValue = _products.ProducerId;
            ProductCategory.SelectedValue = _products.CategoryId;


        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            new ProductWindow().Show();
            Close();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateInput())
                return;
            if (_isEditing == true)
                UpdateProduct();
            else
                CreateProduct();
        }

        private void CreateProduct()
        {
            Products products = new Products();
            
            string article = ProductArticle.Text;
            string name = ProductName.Text;
            decimal cost = Convert.ToDecimal(ProductCost.Text);
            decimal discount = Convert.ToDecimal(ProductDiscoubt.Text);
            decimal instock = Convert.ToDecimal(ProductInStock.Text);
            string description = ProductDescription.Text;

            products.Article = article;
            products.Name = name;
            products.Price = cost;
            products.Discount = discount;
            products.StockQuantity = instock;
            products.Description = description;

            products.UnitId = (int)ProductUnit.SelectedValue;
            products.ProducerId = (int)ProductProducer.SelectedValue;
            products.ProviderId = (int)ProductProvider.SelectedValue;
            products.CategoryId = (int)ProductCategory.SelectedValue;
            products.Photo = _photoPath;

            try
            {
                _db.Products.AddOrUpdate(products);
                _db.SaveChanges();
                _mh.ShowInfo("Продукт успешно создан!");
                CancelButton_Click(null, null);
            }
            catch (Exception ex)
            {
                _mh.ShowError(ex.Message);
            }
        }

        private void UpdateProduct()
        {
            string article = ProductArticle.Text;
            string name = ProductName.Text;
            decimal cost = Convert.ToDecimal(ProductCost.Text);
            decimal discount = Convert.ToDecimal(ProductDiscoubt.Text);
            decimal instock = Convert.ToDecimal(ProductInStock.Text);
            string description = ProductDescription.Text;

            _products.Article = article;
            _products.Name = name;
            _products.Price = cost;
            _products.Discount = discount;
            _products.StockQuantity = instock;
            _products.Description = description;

            _products.UnitId = (int)ProductUnit.SelectedValue;
            _products.ProducerId = (int)ProductProducer.SelectedValue;
            _products.ProviderId = (int)ProductProvider.SelectedValue;
            _products.CategoryId = (int)ProductCategory.SelectedValue;
            _products.Photo = _photoPath;

            try
            {
                _db.Products.AddOrUpdate(_products);
                _db.SaveChanges();
                _mh.ShowInfo("Продукт успешно отредактирован!");
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
            string article = ProductArticle.Text;
            string name = ProductName.Text;
            string cost = ProductCost.Text;
            string discount = ProductDiscoubt.Text;
            string instock = ProductInStock.Text;
            string description = ProductDescription.Text;

            if (string.IsNullOrWhiteSpace(article))
                errors.AppendLine("Поле артикул не заполнено!");

            if (string.IsNullOrWhiteSpace(name))
                errors.AppendLine("Поле наименование не заполнено!");

            if (string.IsNullOrWhiteSpace(cost) || !decimal.TryParse(cost, out decimal costdecimal))
                errors.AppendLine("Поле цена не заполнено!");

            if (string.IsNullOrWhiteSpace(discount) || !decimal.TryParse(discount, out decimal discountdecimal) || discountdecimal > 100 || discountdecimal < 0 )
                errors.AppendLine("Поле скидка не заполнено!");

            if (string.IsNullOrWhiteSpace(instock) || !decimal.TryParse(instock, out decimal inStockdecimal) || inStockdecimal < 0)
                errors.AppendLine("Поле остаток не заполнено!");

            if (string.IsNullOrWhiteSpace(description))
                errors.AppendLine("Поле описание не заполнено!");
            if (errors .Length > 0 )
            {
                _mh.ShowError(errors.ToString());
                return false;
            }
            return true;
        }
        private void SelectPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Изображения|*.jpg;*.jpeg;*.png;*.bmp";
            if (dialog.ShowDialog() != true) return;

            if (_isEditing && !string.IsNullOrEmpty(_products.Photo))
            {
                string oldFullPath = System.IO.Path.GetFullPath(
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", _products.Photo));
                if (System.IO.File.Exists(oldFullPath))
                    System.IO.File.Delete(oldFullPath);
            }

            string photoDir = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Res"));
            System.IO.Directory.CreateDirectory(photoDir);
            string fileName = $"{Guid.NewGuid()}.jpg";
            string destPath = System.IO.Path.Combine(photoDir, fileName);

            var image = new BitmapImage(new Uri(dialog.FileName));
            var resized = new TransformedBitmap(image, new ScaleTransform(300.0 / image.PixelWidth, 200.0 / image.PixelHeight));
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(resized));
            using (var stream = System.IO.File.Create(destPath))
                encoder.Save(stream);

            _photoPath = System.IO.Path.Combine("Res", fileName); 
            ProductPhotoPreview.Source = new BitmapImage(new Uri(destPath));
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (_products.ProductInOrder.Any())
            {
                _mh.ShowError("Нельзя удалить товар, который присутствует в заказе!");
                return;
            }

            _db.Products.Remove(_products);
            _db.SaveChanges();
            _mh.ShowInfo("Товар успешно удалён!");
            CancelButton_Click(null, null);
        }
    }
}
