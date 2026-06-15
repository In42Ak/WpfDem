using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xml.Linq;
using WpfDem.DataBase;

namespace WpfDem.ViewModels
{
    internal class ProductViewModel
    {
        public ProductViewModel(Products products)
        {
            Id = products.Id;
            Article = products.Article;
            Name = products.Name;
            Price = products.Price;
            Discount = products.Discount;
            StockQuantity = products.StockQuantity;
            Description = products.Description;
            Photo = products.Photo;
            Category = products.Category;
            Producer = products.Producer;
            Provider = products.Provider;
            Unit = products.Unit;
            GetBackground();
            GetPhoto();
            GetPrice();
        }
        public int Id { get; set; }
        public string Article { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public decimal? OldPrice { get; set; }
        public decimal? Discount { get; set; }
        public decimal? StockQuantity { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }
        public Category Category { get; set; }
        public Producer Producer { get; set; }
        public Provider Provider { get; set; }
        public Unit Unit { get; set; }
        public Brush Background { get; set; }

        private void GetBackground()
        {
            if (Discount.GetValueOrDefault() >= 15)
            {
                Background = (Brush)new BrushConverter().ConvertFromString("#2E8B57");
                return;
            }
            if (StockQuantity.GetValueOrDefault() == 0)
            {
                Background = Brushes.LightBlue;
                return;
            }
            Background = Brushes.White;
        }

        private void GetPhoto()
        {
            if (!string.IsNullOrEmpty(Photo))
            {
                string fullPath = System.IO.Path.GetFullPath(
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", Photo));
                if (System.IO.File.Exists(fullPath))
                {
                    Photo = fullPath;
                    return;
                }
            }
            Photo = System.IO.Path.GetFullPath(
                System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "Res", "picture.png"));
        }

        private void GetPrice()
        {
            if (Discount.GetValueOrDefault() == 0)
                return;
            OldPrice = Price;
            Price = OldPrice * (1 - Discount.GetValueOrDefault() / 100);
        }
    }
}
