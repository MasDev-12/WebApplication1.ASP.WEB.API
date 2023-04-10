using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.DAL.Interfaces;
using WebApplication.DAL.Repositories;
using WebApplication.Domain.Entity;
using WebApplication.Domain.Enum;
using WebApplication.Service.Implementations;

namespace WebApplication1.ASP.Tests
{
    public class OrderServiceTests
    {
        private Mock<IOrderRepository> _orderRepositoryMock;
        private Mock<IRegionRepository> _regionRepositoryMock;
        private Mock<ItemRepository> _itemRepositoryMock;
        private OrderService _orderService;

        [SetUp]
        public void Setup()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _regionRepositoryMock = new Mock<IRegionRepository>();
            _itemRepositoryMock = new Mock<ItemRepository>();
            _orderService = new OrderService(_orderRepositoryMock.Object, _regionRepositoryMock.Object, _itemRepositoryMock.Object);
        }
        [Test]
        public async Task UpdateOrder_Should_Return_OrderViewModel_With_Updated_Fields()
        {
            // Arrange
            int orderId = 1;
            int regionId = 2;
            int itemId = 3;
            int amount = 4;
            DateTime orderDate = new DateTime(2022, 4, 7);
            var order = new Order { Id = orderId, RegionId = 1, ItemId = 2, Amount = 3, OrderDate = new DateTime(2021, 4, 7) };
            var region = new Region { Id = regionId, Name = "Region 2" };
            var item = new Item { Id = itemId, Name = "Item 3" };
            _orderRepositoryMock.Setup(x => x.GetById(orderId)).ReturnsAsync(order);
            _regionRepositoryMock.Setup(x => x.GetById(regionId)).ReturnsAsync(region);
            _itemRepositoryMock.Setup(x => x.GetById(itemId)).ReturnsAsync(item);

            // Act
            var result = await _orderService.UpdateOrder(orderId, regionId, itemId, amount, orderDate);

            // Assert
            Assert.IsNotNull(result.Data);
            Assert.AreEqual(regionId, result.Data.RegionId);
            Assert.AreEqual(item.Id, result.Data.ItemId);
            Assert.AreEqual(amount, result.Data.Amount);
            Assert.AreEqual(orderDate, result.Data.OrderDate);
        }

        [Test]
        public async Task UpdateOrder_Should_Return_NotFound_Response_If_Order_Not_Exists()
        {
            // Arrange
            int orderId = 1;
            _orderRepositoryMock.Setup(x => x.GetById(orderId)).ReturnsAsync((Order)null);

            // Act
            var result = await _orderService.UpdateOrder(orderId, null, null, null, null);

            // Assert
            Assert.AreEqual(StatusCodeEnum.NotFound, result.StatusCode);
            Assert.IsNull(result.Data);
        }

        [Test]
        public async Task UpdateOrder_Should_Return_InternalServerError_Response_If_Exception_Occurs()
        {
            // Arrange
            int orderId = 1;
            _orderRepositoryMock.Setup(x => x.GetById(orderId)).ThrowsAsync(new Exception());

            // Act
            var result = await _orderService.UpdateOrder(orderId, null, null, null, null);

            // Assert
            Assert.AreEqual(StatusCodeEnum.InternalServerError, result.StatusCode);
            Assert.IsNull(result.Data);
        }
    }
}
