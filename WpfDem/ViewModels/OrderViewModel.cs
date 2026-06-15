using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfDem.DataBase;

namespace WpfDem.ViewModels
{
    internal class OrderViewModel
    {
        public OrderViewModel(Orders orders)
        {
            Id = orders.Id;
            OrderDate = orders.OrderDate;
            OrderDelivery = orders.OrderDelivery;
            Code = orders.Code;
            PickPoint = orders.PickPoint;
            Status = orders.Status;
            Users = orders.Users;
            Article = string.Join(", ", orders.ProductInOrder.Select(p => p.Products.Article));
        }
        public int Id { get; set; }
        public System.DateTime? OrderDate { get; set; }
        public System.DateTime? OrderDelivery { get; set; }
        public string Code { get; set; }
        public string Article { get; set; }
        public virtual PickPoint PickPoint { get; set; }
        public virtual Status Status { get; set; }
        public virtual Users Users { get; set; }
    }


}
