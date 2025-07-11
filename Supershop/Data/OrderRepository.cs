﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Supershop.Data.Entities;
using Supershop.Helpers;
using Supershop.Models;


namespace Supershop.Data
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public OrderRepository(DataContext context, IUserHelper userHelper) : base(context)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task AddItemToOrderAsync(AddItemViewModel model, string username)
        {
            var user = await _userHelper.GetUserByEmailAsync(username);
            if(user == null)
            {
                return;
            }

            var product = await _context.Products.FindAsync(model.ProductId);
            if (product == null)
            {
                return;
            }

            var orderDetailTemp = await _context.OrderDetailsTemp
                .Where(odt => odt.User == user && odt.Product == product)
                .FirstOrDefaultAsync();


            if(orderDetailTemp == null)
            {
                orderDetailTemp = new OrderDetailTemp
                {
                    Product = product,
                    Price = product.Price,
                    Quantity = model.Quantity,
                    User = user
                };

                _context.OrderDetailsTemp.Add(orderDetailTemp);
            }
            else
            {
                orderDetailTemp.Quantity += model.Quantity;
                _context.OrderDetailsTemp.Update(orderDetailTemp);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> ConfirmOrderAsync(string username)
        {
           var user = await _userHelper.GetUserByEmailAsync(username);
            if (user == null)
            {
                return false;
            }

            var orderTmps = await _context.OrderDetailsTemp
                .Include(o => o.Product)
                .Where(o => o.User == user)
                .ToListAsync();

            if (orderTmps == null || orderTmps.Count == 0)
            {
                return false;
            }

            var details = orderTmps.Select(o => new OrderDetail
            {
                Price = o.Price,
                Product = o.Product,
                Quantity = o.Quantity,

            }).ToList();

            var order = new Order
            {
                OrderDate = DateTime.UtcNow,
                User = user,
                Items = details,
            };

            await CreateAsync(order);
            _context.OrderDetailsTemp.RemoveRange(orderTmps);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task DeleteDetailTempAsync(int id)
        {
            var orderDetailTemp = await _context.OrderDetailsTemp.FindAsync(id);
            if ( orderDetailTemp == null)
            {
                return;
            }

            _context.OrderDetailsTemp.Remove(orderDetailTemp);
            await _context.SaveChangesAsync();
        }

        public async Task DeliverOrder(DeliveryViewModel model)
        {
            var order = await _context.Orders.FindAsync(model.Id);
            if (order == null)
            {
                return;
            }

            order.DeliveryDate = model.DeliveryDate;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task<IQueryable<OrderDetailTemp>> GetDetailTempsAsync(string username)
        {
            var user = await _userHelper.GetUserByEmailAsync(username);

            if (user == null)
            {
                return null;
            }

            return _context.OrderDetailsTemp
                .Include(p => p.Product)
                .Where(o => o.User == user)
                .OrderBy(o => o.Product.Name);


        }

        public async Task<IQueryable<Order>> GetOrderAsync(string username)
        {
           var user = await _userHelper.GetUserByEmailAsync(username);
            if (user == null)
            {
                return null;
            }
            if(await _userHelper.IsUserInRoleAsync(user, "Admin"))
            {
               return _context.Orders
                    .Include(o => o.Items)
                    .ThenInclude(p => p.Product)
                    .OrderByDescending(o => o.OrderDate );
            }


            return _context.Orders
                .Include(o => o.Items)
                .ThenInclude(p => p.Product)
                .Where(o => o.User == user)
                .OrderByDescending(o => o.OrderDate);


        }

        public async Task<Order> GetOrderAsync(int id)
        {
            return await _context.Orders.FindAsync(id);
        }

        public async Task ModifyOrderDetailTempQuantityAsync(int id, double quantity)
        {
            var orderDetailTemp = await _context.OrderDetailsTemp.FindAsync(id);

            if(orderDetailTemp == null)
            {
                return;
            }

            
            orderDetailTemp.Quantity += quantity;

            if(orderDetailTemp.Quantity > 0)
            { 
                _context.OrderDetailsTemp.Update(orderDetailTemp);
                await _context.SaveChangesAsync();
            }
           


        }
    }
}
