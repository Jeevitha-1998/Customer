using Core.Interfaces;
using Core.Services;
using Domain.Entities;
using Moq;

namespace CustomerTest
{
    public class CustomerServiceTests
    {
        private readonly CustomerService _customerService;
        private readonly Mock<ICustomerRepository> _customerRepositoryMock;

        public CustomerServiceTests()
        {
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _customerService = new CustomerService(_customerRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAllCustomersAsync_ReturnsListOfCustomers()
        {
            // Arrange
            var mockCustomers = new List<Customer>
            {
                new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" },
                new Customer { Id = 2, FirstName = "Jane", LastName = "Doe", Email = "jane@example.com" }
            };

            // Mock the repository to return a list of customers
            _customerRepositoryMock
                .Setup(repo => repo.GetAllCustomersAsync())
                .ReturnsAsync(mockCustomers);

            // Act
            var result = await _customerService.GetAllCustomersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); 
            Assert.Equal("John", result.First().FirstName);
        }

        [Fact]
        public async Task GetAllCustomersAsync_ReturnsEmptyList_WhenNoCustomersExist()
        {
            // Arrange
            var mockCustomers = new List<Customer>(); 

            // Mock the repository to return an empty list
            _customerRepositoryMock
                .Setup(repo => repo.GetAllCustomersAsync())
                .ReturnsAsync(mockCustomers);

            // Act
            var result = await _customerService.GetAllCustomersAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);  
        }
    }
}