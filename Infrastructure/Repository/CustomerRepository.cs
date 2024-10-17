using Core.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerDbContext _context;

        public CustomerRepository(CustomerDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CustomerOrder>> GetCustomerOrderDetails(int id)
        {
            IEnumerable<CustomerOrder> customerOrders = _context.Customers.Join(_context.Orders,
                                customer => customer.Id,
                                order => order.CustomerId,
                                (customer, order) => new { Customer = customer, Order = order })
                                .Where(co => co.Order.Amount > 50)            
                                .OrderBy(co => co.Customer.LastName)         
                                .Select(co => new CustomerOrder
                                {
                                    CustomerName = $"{co.Customer.FirstName} {co.Customer.LastName}",
                                    Product=co.Order.ProductName,
                                    Price = co.Order.Amount,
                                }).ToList();

            foreach (var item in customerOrders)
            {
                Console.WriteLine($"{item.CustomerName} bought {item.Product} for ${item.Price}");
            }
            return customerOrders;
        }

        public async Task<IEnumerable<CustomerOrder>> GetOrderPlacedCustomerDetails()
        {
            IEnumerable<CustomerOrder> customers = _context.Customers.Join(_context.Orders,
                                  customer => customer.Id,
                                  order => order.CustomerId,
                                  (customer, order) => new
                                  {
                                      customer.Id,
                                      customer.FirstName,
                                      customer.LastName
                                  })
                                .GroupBy(c => new { c.Id, c.FirstName, c.LastName }) 
                                .Select(group => new CustomerOrder
                                {
                                    CustomerName = group.Key.FirstName + " " + group.Key.LastName
                                })
                                .ToList();
            return customers;
        }
    }
}
