using Core.Interfaces;
using Core.Services;
using Domain.Entities;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;

namespace CustomerTest
{
    public class OrderServiceTests
    {
        private readonly OrderService _orderService;
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<ICustomerRepository> _mockCustomerRepository;

        public OrderServiceTests()
        {
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockCustomerRepository = new Mock<ICustomerRepository>();
            _orderService = new OrderService(_mockOrderRepository.Object, _mockCustomerRepository.Object);
        }

        // 1. Test GetAllOrdersAsync
        [Fact]
        public async Task GetAllOrdersAsync_ShouldReturnAllOrders()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { Id = 1, CustomerId = 1, Amount = 100 },
                new Order { Id = 2, CustomerId = 1, Amount = 150 }
            };

            _mockOrderRepository.Setup(repo => repo.GetAllAsync())
                                .ReturnsAsync(orders);

            // Act
            var result = await _orderService.GetAllOrdersAsync();

            // Assert
            result.Should().BeEquivalentTo(orders);
            _mockOrderRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        // 2. Test GetOrderByIdAsync
        [Fact]
        public async Task GetOrderByIdAsync_OrderExists_ShouldReturnOrder()
        {
            // Arrange
            var order = new Order { Id = 1, CustomerId = 1, Amount = 100 };

            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(1))
                                .ReturnsAsync(order);

            // Act
            var result = await _orderService.GetOrderByIdAsync(1);

            // Assert
            result.Should().Be(order);
            _mockOrderRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetOrderByIdAsync_OrderDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            _mockOrderRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<int>()))
                                .ReturnsAsync((Order)null);

            // Act
            var result = await _orderService.GetOrderByIdAsync(1);

            // Assert
            result.Should().BeNull();
            _mockOrderRepository.Verify(repo => repo.GetByIdAsync(1), Times.Once);
        }

        // 3. Test GetOrdersByCustomerIdAsync
        [Fact]
        public async Task GetOrdersByCustomerIdAsync_ShouldReturnOrdersForCustomer()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { Id = 1, CustomerId = 1, Amount = 100 },
                new Order { Id = 2, CustomerId = 1, Amount = 150 }
            };

            _mockOrderRepository.Setup(repo => repo.GetOrdersByCustomerIdAsync(1))
                                .ReturnsAsync(orders);

            // Act
            var result = await _orderService.GetOrdersByCustomerIdAsync(1);

            // Assert
            result.Should().BeEquivalentTo(orders);
            _mockOrderRepository.Verify(repo => repo.GetOrdersByCustomerIdAsync(1), Times.Once);
        }

        // 4. Test AddOrderAsync
        [Fact]
        public async Task AddOrderAsync_ValidCustomer_ShouldAddOrder()
        {
            // Arrange
            var customer = new Customer { Id = 1, FirstName = "John", LastName = "Doe" };
            var order = new Order { Id = 1, CustomerId = 1, Amount = 100 };

            _mockCustomerRepository.Setup(repo => repo.GetCustomerByIdAsync(1))
                                   .ReturnsAsync(customer);

            _mockOrderRepository.Setup(repo => repo.AddAsync(order))
                                .Returns(Task.CompletedTask);

            // Act
            await _orderService.AddOrderAsync(order);

            // Assert
            _mockCustomerRepository.Verify(repo => repo.GetCustomerByIdAsync(1), Times.Once);
            _mockOrderRepository.Verify(repo => repo.AddAsync(order), Times.Once);
        }

        [Fact]
        public async Task AddOrderAsync_InvalidCustomer_ShouldThrowException()
        {
            // Arrange
            var order = new Order { Id = 1, CustomerId = 999, Amount = 100 }; // Invalid customer ID

            _mockCustomerRepository.Setup(repo => repo.GetCustomerByIdAsync(999))
                                   .ReturnsAsync((Customer)null); // Customer not found

            // Act & Assert
            await Assert.ThrowsAsync<System.Exception>(() => _orderService.AddOrderAsync(order));

            _mockCustomerRepository.Verify(repo => repo.GetCustomerByIdAsync(999), Times.Once);
            _mockOrderRepository.Verify(repo => repo.AddAsync(It.IsAny<Order>()), Times.Never);
        }

        // 5. Test UpdateOrderAsync
        [Fact]
        public async Task UpdateOrderAsync_ValidOrder_ShouldUpdateOrder()
        {
            // Arrange
            var order = new Order { Id = 1, CustomerId = 1, Amount = 200 };

            _mockOrderRepository.Setup(repo => repo.UpdateAsync(order))
                                .Returns(Task.CompletedTask);

            // Act
            await _orderService.UpdateOrderAsync(order);

            // Assert
            _mockOrderRepository.Verify(repo => repo.UpdateAsync(order), Times.Once);
        }

        // 6. Test DeleteOrderAsync
        [Fact]
        public async Task DeleteOrderAsync_ValidOrderId_ShouldDeleteOrder()
        {
            // Arrange
            var orderId = 1;

            _mockOrderRepository.Setup(repo => repo.DeleteAsync(orderId))
                                .Returns(Task.CompletedTask);

            // Act
            await _orderService.DeleteOrderAsync(orderId);

            // Assert
            _mockOrderRepository.Verify(repo => repo.DeleteAsync(orderId), Times.Once);
        }
    }
}
