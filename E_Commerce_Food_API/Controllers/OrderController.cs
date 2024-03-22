using AutoMapper;
using E_Commerce_Food_API.Data;
using E_Commerce_Food_API.Models;
using E_Commerce_Food_API.Models.DTO;
using E_Commerce_Food_API.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace E_Commerce_Food_API.Controllers
{
    [Route("api/Order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ApiResponse _response;
        private readonly IMapper _mapper;
        public OrderController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _response = new ApiResponse();
            _mapper = mapper;
        }
        [HttpGet("{UserId?}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiResponse> GetOrders(string? UserId )
        {
            try
            {
                var Orders = _db.OrderHeaders.Include(x => x.OrderDetails).OrderByDescending(x => x.Id);
                if(!string.IsNullOrEmpty(UserId) )
                {
                    _response.Result = Orders.Where(x => x.UserId == UserId);
                }
                else
                {
                    _response.Result = Orders;
                }
                _response.StatusCode = System.Net.HttpStatusCode.OK;
                _response.IsSuccess = true;
            }catch(Exception ex)
            {
                _response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.Errors.Add(ex.Message);
            }

            return Ok(_response);
        }

        [HttpGet("{Id:int}" , Name = "GetOrder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiResponse> GetSpecificOrder(int Id)
        {
            try
            {
                if(Id == 0)
                {
                    _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    return BadRequest(_response);
                }
                var Order = _db.OrderHeaders.Include(x => x.OrderDetails).FirstOrDefault(x => x.Id == Id);
                if(Order == null)
                {
                    _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.Errors.Add("The id Id you provide does not exist !");
                    return BadRequest(_response);
                }
                _response.Result = Order;
                _response.StatusCode = System.Net.HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.Errors.Add(ex.Message);
                return BadRequest(_response);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiResponse> CreateOrder([FromBody] OrderHeaderCreateDTO orderHeaderCreateDTO)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var order = _mapper.Map<OrderHeader>(orderHeaderCreateDTO);
                    if (string.IsNullOrEmpty(order.Status))
                        order.Status = SD.status_pending;

                    order.OrderDate = DateTime.Now;
                    _db.OrderHeaders.Add(order);
                    _db.SaveChanges();
                    foreach (var orderDetailDTO in orderHeaderCreateDTO.OrderDetailsDTO)
                    {
                        var orderDetails = _mapper.Map<OrderDetail>(orderDetailDTO);
                        orderDetails.OrderHeaderId = order.Id;
                        _db.OrderDetails.Add(orderDetails);
                    }
                    _db.SaveChanges();
                    _response.Result = order;
                    order.OrderDetails = null;
                    _response.StatusCode = HttpStatusCode.Created;
                    return CreatedAtRoute("GetOrder", new { order.Id }, _response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Errors = ModelState.SelectMany(x => x.Value.Errors.Select(p => p.ErrorMessage)).ToList();
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

            }catch(Exception ex)
            {
                _response.StatusCode = System.Net.HttpStatusCode.InternalServerError;
                _response.IsSuccess = false;
                _response.Errors.Add(ex.Message);
                return BadRequest(_response);
            }
        }
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //fromform because we need to upload an image not just a json data
        public async Task<ActionResult<ApiResponse>> UpdateOrder([FromForm] OrderHeaderUpdateDTO OrderUpDto, int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (id == 0 || OrderUpDto == null || id != OrderUpDto.Id)
                    {
                        return BadRequest(ModelState);
                    }
                    OrderHeader OrderDB = await _db.OrderHeaders.FirstOrDefaultAsync(u => u.Id == id);
                    if (OrderDB == null)
                    {
                        _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        _response.IsSuccess = false;
                        _response.Errors.Add("the Order does not exist !");
                        return BadRequest(_response);
                    }
                    OrderDB = _mapper.Map<OrderHeader>(OrderUpDto);
                   
                    _db.OrderHeaders.Update(OrderDB);
                    _db.SaveChanges();
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.NoContent;
                    _response.Result = OrderDB;
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.Errors = ModelState.SelectMany(x => x.Value.Errors.Select(p => p.ErrorMessage)).ToList();
                    _response.StatusCode = HttpStatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return _response;
        }
    }
}
