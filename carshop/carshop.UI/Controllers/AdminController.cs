using AutoMapper;
using carshop.carshop.UI.Models;
using casrshop.Core;
using casrshop.Core.IServices;
using casrshop.Core.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using casrshop.Core.Entities;

namespace carshop.carshop.UI.Controllers
{
    [Authorize]
    [Route("api/admin/home")]
    [ApiController]
    
    public class AdminController : ControllerBase
    {
        private readonly IMessageCreator _messageCreator;
        private readonly IEmailSender _emailSender;

        private readonly ICarRepository _carRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public AdminController(IMessageCreator messageCreator, IEmailSender emailSender, ICarRepository carRepository, IOrderRepository orderRepository, IMapper mapper)
        {
            _messageCreator=messageCreator;
            _emailSender=emailSender;
            _carRepository=carRepository;
            _orderRepository=orderRepository;
            _mapper=mapper;
        }

        [HttpPost("create/car")]
        public async Task<IActionResult> AddCar([FromBody]CarModel model)
        {
            var car=_mapper.Map<Car>(model);
            await _carRepository.AddCarAsync(car);

            return Ok();
        }

        [HttpDelete("delete/orders/expired")]
        public async Task<IActionResult> DeleteExpired()
        {
            var toDelete =await _orderRepository.GetAllAsync();
            foreach(var order in toDelete)
            {
                if (order.Enddate<System.DateTime.Now)
                {
                    await _orderRepository.DeleteOrderAsync(order.Id);
                }
            }

            return Ok();
        }

        [HttpGet("cars")]
        public async Task<IActionResult> GetCars()
        {
            var cars=await _carRepository.GetAllAsync();
            var carModels = new List<CarModel>();
            foreach (var car in cars)
            {
                carModels.Add(_mapper.Map<CarModel>(car));
            }
            return Ok(carModels);
        }

        [HttpDelete("{id:int}/delete/car")]
        public async Task<IActionResult> DeleteCar([FromRoute] int id)
        {
            var car = await _carRepository.GetByIdAsync(id);
            var toSendMessage= await _orderRepository.GetOrdersByCar(car);
            if (toSendMessage!=null)
            {
                foreach (var order in toSendMessage)
                {
                    var message = _messageCreator.MessageToDeleteCarByAdmin(order, car);
                    await _emailSender.SendEmail(message, order.Email);
                    await _orderRepository.DeleteOrderAsync(order.Id);
                }
            }
            await _carRepository.DeleteByIdAsync(id);

            return Ok();
        }
    }
}
