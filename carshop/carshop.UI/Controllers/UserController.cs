using AutoMapper;
using carshop.carshop.UI.Models;
using casrshop.Core;
using System.Linq;
using casrshop.Core.IServices;
using System;
using casrshop.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace carshop.carshop.UI.Controllers
{
    [Route("api/home")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMessageCreator _messageCreator;
        private readonly IEmailSender _emailSender;
        private readonly IPriceCalculator _priceCalculator;

        private readonly ICarRepository _carRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        public UserController(ICarRepository carRepository, IOrderRepository orderRepository, IMapper mapper, IMessageCreator messageCreator, IEmailSender emailSender, IPriceCalculator priceCalculator)
        {
            _orderRepository=orderRepository;
            _carRepository=carRepository;
            _mapper=mapper;
            _messageCreator=messageCreator;
            _emailSender=emailSender;
            _priceCalculator=priceCalculator;
        }

        [HttpGet("available/cars")]
        public async Task<IActionResult> GetCarsAsync([FromQuery] string startDate, [FromQuery] string endDate)
        {
            var isStartDateCorrectFormat = DateTime.TryParse(startDate, out DateTime start);
            if (!isStartDateCorrectFormat)
            {
                return Forbid($"Wrong datatype {startDate}");
            }
            var isEndDateCorrectFormat = DateTime.TryParse(endDate,out DateTime end);
            if (!isEndDateCorrectFormat)
            {
                return Forbid($"Wrong datatype {endDate}");
            }
            if (start>end)
            {
                return Conflict($"Wrong Date. {end} is earlier than {start}");
            }
            if(start < DateTime.Now || end <DateTime.Now)
            {
                return Conflict($"This date is not available");
            }

            var cars =await _carRepository.GetByDateAsync(start, end);

            

            var carModels = new List<CarModel>();
            foreach (var car in cars)
            {
                carModels.Add(_mapper.Map<CarModel>(car));
            }
            return Ok(carModels);
        }

        [HttpGet("{id:int}/car")]
        public async Task<IActionResult> GetCar([FromRoute]int id)
        {
            var car=await _carRepository.GetByIdAsync(id);
            return Ok(car);
        }

        [HttpGet("car/{model}")]
        public async Task<IActionResult> GetCarByModel([FromRoute]string model)
        {
            var car=await _carRepository.GetByModelAsync(model);
            return Ok(car);
        }

        [HttpPost("create/order")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderModel model)
        {
            var order = this._mapper.Map<Order>(model);
            var carsAvailable=await _carRepository.GetByDateAsync(order.Startdate, order.Enddate);
            var idAvailable =carsAvailable.Select(x => x.Id).ToList();
            if (!idAvailable.Contains(order.Car_Id))
            {
                return NotFound($"Car is not available, please choose another car");
            }
            var car = await _carRepository.GetByIdAsync(order.Car_Id);
            _priceCalculator.CalculatePrice(order, car);
            await _orderRepository.CreateOrderAsync(order);

            var message = _messageCreator.MessageCreate(order, car);
            await _emailSender.SendEmail(message, order.Email);

            return Ok();
        }

        [HttpPut("{id:int}/edit/order")]
        public async Task<IActionResult> UpdateOrder([FromBody]OrderModel model, [FromRoute]int id)
        {
            var order=this._mapper.Map<Order>(model);
            var car = await _carRepository.GetByIdAsync(order.Car_Id);
            _priceCalculator.CalculatePrice(order, car);
            var res = await UpdateAsync(order, id);
            if (order.Startdate>order.Enddate)
            {
                return Conflict($"Wrong Date. {order.Enddate} is earlier than {order.Startdate}");
            }

            var message = _messageCreator.MessageEdit(order, car);
            await _emailSender.SendEmail(message, order.Email);

            return res ? Ok():NotFound();
        }

        

        private async Task<bool> UpdateAsync(Order order, int id)
        {
            var toUpdate = await _orderRepository.GetByIdAsync(id);
            if (toUpdate!=null)
            {
                toUpdate.Firstname=order.Firstname;
                toUpdate.Lastname=order.Lastname;
                toUpdate.Email=order.Email;
                toUpdate.Car_Id=order.Car_Id;
                toUpdate.Startdate=order.Startdate;
                toUpdate.Enddate=order.Enddate;

                await _orderRepository.UpdateOrderAsync(toUpdate);

                return true;
            }

            return false;
        }

        [HttpDelete("{id:int}/delete/order")]
        public async Task<IActionResult> DeleteOrder([FromRoute]int id)
        {
            var toDel = await _orderRepository.GetByIdAsync(id);
            if (toDel != null)
            {
                var message= _messageCreator.MessageDelete(toDel);
                var email = toDel.Email;
                await _orderRepository.DeleteOrderAsync(id);
                
                await _emailSender.SendEmail(message, email);

                return Ok();
            }

            return NotFound();
        }
    }
}
