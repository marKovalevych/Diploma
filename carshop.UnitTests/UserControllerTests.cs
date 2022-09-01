using AutoMapper;
using carshop.carshop.UI.Controllers;
using carshop.carshop.UI.Models;
using casrshop.Core;
using casrshop.Core.IServices;
using casrshop.Core.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace carshop.UnitTests
{
    [TestFixture]
    public class UserControllerTests
    {
        private IMapper SetMapper()
        {
            var mapper = new Mock<IMapper>();

            mapper.Setup(x => x.Map<OrderModel>(It.IsAny<Order>()))
                .Returns((Order o) =>
                {
                    return new OrderModel
                    {
                        Firstname = o.Firstname,
                        Lastname = o.Lastname,
                        Car_Id = o.Car_Id,
                        Email = o.Email,
                        Startdate = o.Startdate,
                        Enddate = o.Enddate
                    };
                });
            mapper.Setup(x => x.Map<Order>(It.IsAny<OrderModel>()))
               .Returns((OrderModel o) =>
               {
                   return new Order
                   {
                       Firstname = o.Firstname,
                       Lastname = o.Lastname,
                       Car_Id = o.Car_Id,
                       Email = o.Email,
                       Startdate = o.Startdate,
                       Enddate = o.Enddate
                   };
               });

            return mapper.Object;
        }
        [Test]
        public async Task GetCarByIdTest()
        {
            //Arrange
            var carRepo = new Mock<ICarRepository>();
            carRepo.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Car
            {
                Id=1,
                Manufacturer="Lancia"
            });

            var orderRepo = new Mock<IOrderRepository>();
            var emailSender = new Mock<IEmailSender>();
            var messageCreator = new Mock<IMessageCreator>();
            var priceCalculator = new Mock<IPriceCalculator>();
            var mapper = SetMapper();

            var userController = new UserController(carRepo.Object, orderRepo.Object, mapper, messageCreator.Object, emailSender.Object, priceCalculator.Object);

            //Act
            var result = await userController.GetCar(1);
            var okObject = result as OkObjectResult;

            //Assert
            okObject.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okObject.Value.Should().BeEquivalentTo(new CarModel
            {
                Id = 1,
                Manufacturer = "Lancia"
            });
        }

        [Test]
        public async Task GetCarByModel()
        {
            //Arrange
            var carRepo = new Mock<ICarRepository>();
            carRepo.Setup(x => x.GetByModelAsync("Stratos", "Lancia")).ReturnsAsync(new Car
            {
                Manufacturer="Lancia",
                Model = "Stratos"
            });

            var orderRepo = new Mock<IOrderRepository>();
            var emailSender = new Mock<IEmailSender>();
            var messageCreator = new Mock<IMessageCreator>();
            var priceCalculator = new Mock<IPriceCalculator>();
            var mapper = SetMapper();

            var userController = new UserController(carRepo.Object, orderRepo.Object, mapper, messageCreator.Object, emailSender.Object, priceCalculator.Object);

            //Act
            var result = await userController.GetCarByModel("Stratos", "Lancia");
            var okObject = result as OkObjectResult;

            //Assert
            okObject.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okObject.Value.Should().BeEquivalentTo(new CarModel
            {
                Manufacturer = "Lancia",
                Model = "Stratos"
            });
        }

        [TestCase("gsd", "20.09.2022")]
        [TestCase("20.09.2022", "gsd")]
        public async Task GetCarsByDate_Conflict_WrongDataType(string start, string end)
        {
            var carRepo = new Mock<ICarRepository>();
            var orderRepo = new Mock<IOrderRepository>();
            var emailSender = new Mock<IEmailSender>();
            var messageCreator = new Mock<IMessageCreator>();
            var priceCalculator = new Mock<IPriceCalculator>();
            var mapper = SetMapper();

            var userController = new UserController(carRepo.Object, orderRepo.Object, mapper, messageCreator.Object, emailSender.Object, priceCalculator.Object);


            var result = await userController.GetCarsAsync(start, end);
            var conflictResult = result as ConflictObjectResult;
            

            conflictResult.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
            conflictResult.Value.Should().BeEquivalentTo("Wrong datatype");
        }

        [TestCase("20.09.2022", "15.09.2022")]
        [TestCase("20.08.2022", "20.09.2022")]
        public async Task GetCarsByDate_Conflict_WrongDate(string start, string end)
        {
            var carRepo = new Mock<ICarRepository>();
            var orderRepo = new Mock<IOrderRepository>();
            var emailSender = new Mock<IEmailSender>();
            var messageCreator = new Mock<IMessageCreator>();
            var priceCalculator = new Mock<IPriceCalculator>();
            var mapper = SetMapper();

            var userController = new UserController(carRepo.Object, orderRepo.Object, mapper, messageCreator.Object, emailSender.Object, priceCalculator.Object);


            var result = await userController.GetCarsAsync(start, end);
            var conflictResult = result as ConflictObjectResult;


            conflictResult.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
            conflictResult.Value.Should().BeEquivalentTo("Wrong date");
        }

        [TestCase("20.09.2022", "25.09.2022")]
        public async Task GetCarsByDate_OkResult(string start, string end)
        {
            //Arrange
            var s = DateTime.Parse(start);
            var e = DateTime.Parse(end);
            var carRepo = new Mock<ICarRepository>();
            var car = new Car
            {
                Id=1
            };
            carRepo.Setup(x => x.GetByDateAsync(s, e)).ReturnsAsync(new List<Car>
            {
                car
            });
            var orderRepo = new Mock<IOrderRepository>();
            var order = new Order
            {
                Car_Id = 1,
                Car=car,
                Startdate=DateTime.Parse("14.09.2022"),
                Enddate = DateTime.Parse("18.09.2002")
            };
            orderRepo.Setup(x => x.CreateOrderAsync(order));

            var emailSender = new Mock<IEmailSender>();
            var messageCreator = new Mock<IMessageCreator>();
            var priceCalculator = new Mock<IPriceCalculator>();
            var mapper = SetMapper();

            var userController = new UserController(carRepo.Object, orderRepo.Object, mapper, messageCreator.Object, emailSender.Object, priceCalculator.Object);


            var result = await userController.GetCarsAsync(start, end);
            var okObject = result as OkObjectResult;

            okObject.StatusCode.Should().Be((int)HttpStatusCode.OK);
            okObject.Value.Should().BeEquivalentTo(new List<CarModel>
            {
                new CarModel
                {
                    Id = 1
                }
            });
        }

        [Test]
        public async Task CreateOrder_CarNotFoundTest()
        {
            var orderModel = new OrderModel
            {
                Firstname = "",
                Lastname = "",
                Car_Id = 1,
                Email = "motukasapka@gmail.com",
                Startdate = DateTime.Parse("14.09.2022"),
                Enddate = DateTime.Parse("18.09.2022")
            };
            var car = new Car
            {
                Id = 1
            };

            var carRepo = new Mock<ICarRepository>();
            var orderRepo = new Mock<IOrderRepository>();
            carRepo.Setup(x => x.GetByDateAsync(orderModel.Startdate, orderModel.Enddate))
                .ReturnsAsync(new List<Car>());
            var emailSender = new Mock<IEmailSender>();
            var messageCreator = new Mock<IMessageCreator>();
            var priceCalculator = new Mock<IPriceCalculator>();
            var mapper = SetMapper();

            var userController = new UserController(carRepo.Object, orderRepo.Object, mapper, messageCreator.Object, emailSender.Object, priceCalculator.Object);

            
            var result = await userController.CreateOrder(orderModel);
            var notFoundResult = result as NotFoundObjectResult;

            notFoundResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            notFoundResult.Value.Should().Be($"Car is not available, please choose another car");
        }

        [Test]
        public async Task CreateOrder_WrongEmail()
        {
            var orderModel = new OrderModel
            {
                Firstname = "",
                Lastname = "",
                Car_Id = 1,
                Email = "",
                Startdate = DateTime.Parse("14.09.2022"),
                Enddate = DateTime.Parse("18.09.2022")
            };
            var car = new Car
            {
                Id = 1
            };

            var carRepo = new Mock<ICarRepository>();
            var orderRepo = new Mock<IOrderRepository>();
            carRepo.Setup(x => x.GetByDateAsync(orderModel.Startdate, orderModel.Enddate))
                .ReturnsAsync(new List<Car>
                {
                    car
                });
            var emailSender = new Mock<IEmailSender>();
            var messageCreator = new Mock<IMessageCreator>();
            var priceCalculator = new Mock<IPriceCalculator>();
            var mapper = SetMapper();

            var userController = new UserController(carRepo.Object, orderRepo.Object, mapper, messageCreator.Object, emailSender.Object, priceCalculator.Object);


            var result = await userController.CreateOrder(orderModel);
            var conflictResult = result as ConflictObjectResult;

            conflictResult.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
            conflictResult.Value.Should().BeEquivalentTo($"{mapper.Map<Order>(orderModel).Email} is not in correct format");
        }

        [Test]
        public async Task CreateOrder_OkResult()
        {
            var orderModel = new OrderModel
            {
                Firstname = "",
                Lastname = "",
                Car_Id = 1,
                Email = "motukasapka@gmail.com",
                Startdate = DateTime.Parse("14.09.2022"),
                Enddate = DateTime.Parse("18.09.2022")
            };
            var car = new Car
            {
                Id = 1
            };

            var carRepo = new Mock<ICarRepository>();
            var orderRepo = new Mock<IOrderRepository>();
            carRepo.Setup(x => x.GetByDateAsync(orderModel.Startdate, orderModel.Enddate))
                .ReturnsAsync(new List<Car>
                {
                    car
                });
            var emailSender = new Mock<IEmailSender>();
            var messageCreator = new Mock<IMessageCreator>();
            var priceCalculator = new Mock<IPriceCalculator>();
            var mapper = SetMapper();
            messageCreator.Setup(x => x.MessageCreate(mapper.Map<Order>(orderModel), car)).Returns("");
            emailSender.Setup(x => x.SendEmail("", mapper.Map<Order>(orderModel).Email));
            priceCalculator.Setup(x => x.CalculatePrice(mapper.Map<Order>(orderModel), car));

            var userController = new UserController(carRepo.Object, orderRepo.Object, mapper, messageCreator.Object, emailSender.Object, priceCalculator.Object);


            var result = await userController.CreateOrder(orderModel);
            var okResult = result as OkResult;

            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }

        [Test]
        public async Task UpdateOrder_StartAfterEnd()
        {
            var order = new Order
            {
                Id = 1,
                Firstname = "",
                Lastname = "",
                Car_Id = 1,
                Email = "motukasapka@gmail.com",
                Startdate = DateTime.Parse("14.09.2022"),
                Enddate = DateTime.Parse("13.09.2022")
            };

            var carRepo = new Mock<ICarRepository>();
            var orderRepo = new Mock<IOrderRepository>();
            var emailSender = new Mock<IEmailSender>();
            var messageCreator = new Mock<IMessageCreator>();
            var priceCalculator = new Mock<IPriceCalculator>();
            var mapper = SetMapper();

            var userController = new UserController(carRepo.Object, orderRepo.Object, mapper, messageCreator.Object, emailSender.Object, priceCalculator.Object);

            var result = await userController.UpdateOrder(mapper.Map<OrderModel>(order), order.Id);
            var conflictResult = result as ConflictObjectResult;

            conflictResult.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
            conflictResult.Value.Should().Be($"Wrong Date. {order.Enddate} is earlier than {order.Startdate}");
        }

        [Test]
        public async Task UpdateOrder_StartBeforeNow()
        {
            var order = new Order
            {
                Id = 1,
                Firstname = "",
                Lastname = "",
                Car_Id = 1,
                Email = "motukasapka@gmail.com",
                Startdate = DateTime.Parse("20.08.2022"),
                Enddate = DateTime.Parse("13.09.2022")
            };

            var carRepo = new Mock<ICarRepository>();
            var orderRepo = new Mock<IOrderRepository>();
            var emailSender = new Mock<IEmailSender>();
            var messageCreator = new Mock<IMessageCreator>();
            var priceCalculator = new Mock<IPriceCalculator>();
            var mapper = SetMapper();

            var userController = new UserController(carRepo.Object, orderRepo.Object, mapper, messageCreator.Object, emailSender.Object, priceCalculator.Object);

            var result = await userController.UpdateOrder(mapper.Map<OrderModel>(order), order.Id);
            var conflictResult = result as ConflictObjectResult;

            conflictResult.StatusCode.Should().Be((int)HttpStatusCode.Conflict);
            conflictResult.Value.Should().Be($"Wrong date");
        }

        [Test]
        public async Task UpdateOrder_CarNotFound()
        {
            var order = new Order
            {
                Id = 1,
                Firstname = "",
                Lastname = "",
                Car_Id = 1,
                Email = "motukasapka@gmail.com",
                Startdate = DateTime.Parse("14.09.2022"),
                Enddate = DateTime.Parse("18.09.2022")
            };

            var carRepo = new Mock<ICarRepository>();
            var orderRepo = new Mock<IOrderRepository>();
            carRepo.Setup(x => x.GetByDateAsync(order.Startdate, order.Enddate))
                .ReturnsAsync(new List<Car>());
            var emailSender = new Mock<IEmailSender>();
            var messageCreator = new Mock<IMessageCreator>();
            var priceCalculator = new Mock<IPriceCalculator>();
            var mapper = SetMapper();

            var userController = new UserController(carRepo.Object, orderRepo.Object, mapper, messageCreator.Object, emailSender.Object, priceCalculator.Object);

            var result = await userController.UpdateOrder(mapper.Map<OrderModel>(order), order.Id);
            var notFoundResult = result as NotFoundObjectResult;

            notFoundResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
            notFoundResult.Value.Should().Be($"Car is not available, please choose another car");
        }

        [Test]
        public async Task UpdateOrder_WrongOrderId()
        {
            var order = new Order
            {
                Id = 1,
                Firstname = "",
                Lastname = "",
                Car_Id = 1,
                Email = "motukasapka@gmail.com",
                Startdate = DateTime.Parse("14.09.2022"),
                Enddate = DateTime.Parse("18.09.2022")
            };
            var car = new Car
            {
                Id = 1
            };

            var carRepo = new Mock<ICarRepository>();
            var orderRepo = new Mock<IOrderRepository>();
            carRepo.Setup(x => x.GetByDateAsync(order.Startdate, order.Enddate))
                .ReturnsAsync(new List<Car>
                {
                    car
                });
            orderRepo.Setup(x => x.GetByIdAsync(2));

            var messageCreator = new Mock<IMessageCreator>();
            var emailSender = new Mock<IEmailSender>();
            var priceCalculator = new Mock<IPriceCalculator>();
            var mapper = SetMapper();

            var userController = new UserController(carRepo.Object, orderRepo.Object, mapper, messageCreator.Object, emailSender.Object, priceCalculator.Object);
            var result = await userController.UpdateOrder(mapper.Map<OrderModel>(order), 2);
            var notFoundResult = result as NotFoundResult;

            notFoundResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

        }

        [Test]
        public async Task UpdateOrder_OkResult()
        {
            var order = new Order
            {
                Id = 1,
                Firstname = "",
                Lastname = "",
                Car_Id = 1,
                Email = "motukasapka@gmail.com",
                Startdate = DateTime.Parse("14.09.2022"),
                Enddate = DateTime.Parse("18.09.2022")
            };
            var car = new Car
            {
                Id = 1
            };

            var carRepo = new Mock<ICarRepository>();
            var orderRepo = new Mock<IOrderRepository>();
            carRepo.Setup(x => x.GetByDateAsync(order.Startdate, order.Enddate))
                .ReturnsAsync(new List<Car>
                {
                    car
                });
            orderRepo.Setup(x => x.GetByIdAsync(order.Id)).
                ReturnsAsync(order);
            var emailSender = new Mock<IEmailSender>();
            var messageCreator = new Mock<IMessageCreator>();
            var priceCalculator = new Mock<IPriceCalculator>();
            var mapper = SetMapper();
            messageCreator.Setup(x => x.MessageEdit(order, car)).Returns("");
            emailSender.Setup(x => x.SendEmail("", order.Email));
            priceCalculator.Setup(x => x.CalculatePrice(order, car));

            var userController = new UserController(carRepo.Object, orderRepo.Object, mapper, messageCreator.Object, emailSender.Object, priceCalculator.Object);

            var result = await userController.UpdateOrder(mapper.Map<OrderModel>(order), order.Id);
            var okResult = result as OkResult;

            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);

        }

        [Test]
        public async Task DeleteOrder_WrongOrderId()
        {
            var carRepo = new Mock<ICarRepository>();
            var orderRepo = new Mock<IOrderRepository>();
            orderRepo.Setup(x => x.GetByIdAsync(2));

            var emailSender = new Mock<IEmailSender>();
            emailSender.Setup(x => x.SendEmail("", "motukasapka@gmail.com"));
            var messageCreator = new Mock<IMessageCreator>();
            messageCreator.Setup(x => x.MessageDelete(new Order
            {

            })).Returns("");
            var priceCalculator = new Mock<IPriceCalculator>();
            var mapper = SetMapper();

            var userController = new UserController(carRepo.Object, orderRepo.Object, mapper, messageCreator.Object, emailSender.Object, priceCalculator.Object);
            var result = await userController.DeleteOrder(2);
            var notFoundResult = result as NotFoundResult;

            notFoundResult.StatusCode.Should().Be((int)HttpStatusCode.NotFound);

        }

        [Test]
        public async Task DeleteOrder_OkResult()
        {
            var order = new Order
            {
                Id = 1,
                Firstname = "",
                Lastname = "",
                Car_Id = 1,
                Email = "motukasapka@gmail.com",
                Startdate = DateTime.Parse("14.09.2022"),
                Enddate = DateTime.Parse("18.09.2022")
            };
            var carRepo = new Mock<ICarRepository>();
            var orderRepo = new Mock<IOrderRepository>();
            orderRepo.Setup(x => x.GetByIdAsync(order.Id)).
                ReturnsAsync(order);
            var emailSender = new Mock<IEmailSender>();
            var messageCreator = new Mock<IMessageCreator>();
            var priceCalculator = new Mock<IPriceCalculator>();
            var mapper = SetMapper();

            var userController = new UserController(carRepo.Object, orderRepo.Object, mapper, messageCreator.Object, emailSender.Object, priceCalculator.Object);

            var result = await userController.DeleteOrder(order.Id);
            var okResult = result as OkResult;

            okResult.StatusCode.Should().Be((int)HttpStatusCode.OK);
        }
    }
}
