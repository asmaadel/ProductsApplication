using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductsApplication.Repository
{
    public class OrderProductRepo
    {

        #region Fileds

        private readonly ApplicationContext _appContext;

        #endregion

        public OrderProductRepo(ApplicationContext applicationContext)
        {
            _appContext = applicationContext;
        }

        #region Order

        public List<OrderProduct> GetUserOrders(string userId)
        {
            return _appContext.OrderProducts.Include(o => o.Order).Include(o => o.Product).Where(o=>o.Order.UserId==userId).ToList();
        }
        public List<OrderProduct> GetOrder(int id)
        {
            return _appContext.OrderProducts.Include(o => o.Order).Include(o => o.Product).Where(o => o.Order.Id == id).ToList();
        }
        public bool AddOrderProducts(OrderDto data, string userId)
        {
            bool added = false;
            using (IDbContextTransaction transaction = _appContext.Database.BeginTransaction())
            {
                try
                {
                    var order = new Order();
                    order.UserId = userId;
                    order.Date = DateTime.Now;
                    _appContext.Order.Add(order);
                    _appContext.SaveChanges();
                    if (order.Id > 0)
                    {
                        foreach (var item in data.Products)
                        {
                            var orderprod = new OrderProduct();
                            orderprod.OrderId = order.Id;
                            orderprod.Quantity = item.Quantity;
                            orderprod.ProductId = item.Id;
                            _appContext.OrderProducts.Add(orderprod);
                        }
                        _appContext.SaveChanges();
                    }

                    transaction.Commit();
                    added= true;
                }
                catch
                {
                    transaction.Rollback();
                    added= false;
                }
                return added;
            }
        }

        public bool EditOrderProduct(OrderDto data)
        {
            bool updated = false;
            using (IDbContextTransaction transaction = _appContext.Database.BeginTransaction())
            {
                try
                {
                   var prods= _appContext.OrderProducts.Where(x => x.OrderId == data.OrderId).ToList();
                    foreach (var item in prods)
                    {
                        _appContext.Entry(item).State = EntityState.Deleted;
                    }
                    _appContext.SaveChanges();
                    if (data.OrderId > 0)
                    {
                        foreach (var item in data.Products)
                        {
                            var orderprod = new OrderProduct();
                            orderprod.OrderId = data.OrderId;
                            orderprod.Quantity = item.Quantity;
                            orderprod.ProductId = item.Id;
                            _appContext.OrderProducts.Add(orderprod);
                        }
                        _appContext.SaveChanges();
                    }

                    transaction.Commit();
                    updated = true;
                }
                catch
                {
                    transaction.Rollback();
                    updated = false;
                }
            }
            return updated;
        }
        public bool DeleteOrders(List<OrderDto> data)
        {
            bool deleted = false;
            using (IDbContextTransaction transaction = _appContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in data)
                    {
                     var orderprods= _appContext.OrderProducts.Where(o => o.OrderId == item.OrderId).ToList();
                        foreach (var orderPord in orderprods)
                        {
                            _appContext.Entry(orderPord).State = EntityState.Deleted;
                        }
                        _appContext.SaveChanges();

                        var order = _appContext.Order.FirstOrDefault(o => o.Id == item.OrderId);
                        _appContext.Entry(order).State = EntityState.Deleted;
                        _appContext.SaveChanges();
                    }
                    transaction.Commit();
                    deleted = true;
                }
                catch
                {
                    transaction.Rollback();
                    deleted = false;
                }
                return deleted;
            }
        }
        public bool DeleteOrderProduct(List<OrderProduct> emps)
        {
            try
            {
                if (emps != null && emps.Count() > 0)
                {
                    foreach (var item in emps)
                    {
                        DeleteOrderProduct(item);
                    }
                }
            }
            catch (Exception ex)
            {

                return false;
            }
            return true;
        }

        public void DeleteOrderProduct(OrderProduct data)
        {
            if (data != null)
            {
               // data.valid = true;
                _appContext.Entry(data).State = EntityState.Modified;
                _appContext.SaveChanges();
            }
        }
       
        #endregion

        #region Product

        public List<Product> GetProducts()
        {
            return _appContext.Products.Where(p=>p.valid==true).ToList();
        }
        public Product GetProduct(int id)
        {
            return _appContext.Products.FirstOrDefault(p=>p.Id==id);
        }
        public bool AddProduct(Product data)
        {
            bool added = false;
            try
            {
                data.CreatedDate = DateTime.Now;
                data.valid = true;
                _appContext.Products.Add(data);
                _appContext.SaveChanges();
                added = true;
            }
            catch (Exception ex)
            {

            }
            return added;
        }
        
        public bool EditProduct(Product data)
        {
            bool updated = false;
            try
            {
                _appContext.Entry(data).State = EntityState.Modified;
                _appContext.SaveChanges();
                updated = true;
            }
            catch (Exception ex)
            {

            }
            return updated;
        }

        public bool DeleteProduct(List<Product> prods)
        {
            try
            {
                if (prods != null && prods.Count() > 0)
                {
                    foreach (var item in prods)
                    {
                        item.valid = false;
                        EditProduct(item);
                    }
                }
            }
            catch (Exception ex)
            {

                return false;
            }
            return true;
        }

        #endregion

    }
}
