using System;
using System.Collections.Generic;
using System.Linq;
using BubbleTeaApp.Data;
using BubbleTeaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BubbleTeaApp.Services
{
    public class OrderService
    {
        public int CreateOrder(Order order)
        {
            using var context = new BubbleTeaDbContext();
            
            // Tính toán lại các giá trị
            order.FinalAmount = order.TotalAmount - (order.Discount ?? 0) + (order.VAT ?? 0);
            
            context.Orders.Add(order);
            context.SaveChanges();
            return order.Id;
        }

        public Order? GetOrderById(int id)
        {
            using var context = new BubbleTeaDbContext();
            return context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Topping)
                .Include(o => o.User)
                .FirstOrDefault(o => o.Id == id);
        }

        public List<Order> GetAllOrders()
        {
            using var context = new BubbleTeaDbContext();
            return context.Orders
                .Include(o => o.OrderDetails)
                .Include(o => o.User)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public List<Order> GetOrdersByDate(DateTime date)
        {
            using var context = new BubbleTeaDbContext();
            return context.Orders
                .Where(o => o.OrderDate.HasValue && o.OrderDate.Value.Date == date.Date)
                .Include(o => o.OrderDetails)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
        }

        public void UpdateOrderStatus(int orderId, string status)
        {
            using var context = new BubbleTeaDbContext();
            var order = context.Orders.Find(orderId);
            if (order != null)
            {
                order.Status = status;
                context.SaveChanges();
            }
        }

        public decimal GetTotalRevenue(DateTime fromDate, DateTime toDate)
        {
            using var context = new BubbleTeaDbContext();
            return context.Orders
                .Where(o => o.OrderDate.HasValue && 
                           o.OrderDate.Value >= fromDate && 
                           o.OrderDate.Value <= toDate && 
                           o.Status == "Completed")
                .Sum(o => o.FinalAmount ?? o.TotalAmount);
        }
    }
}
