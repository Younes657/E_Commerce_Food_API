using AutoMapper;
using E_Commerce_Food_API.Data;
using E_Commerce_Food_API.Models;
using E_Commerce_Food_API.Models.DTO;
using E_Commerce_Food_API.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.Json;

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
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ApiResponse> GetOrders([FromRoute] string? UserId , string? searchValue , string? Status , int PageNumber = 1 , int Pagesize=5 )
        {
            try
            {
                IEnumerable<OrderHeader>? Orders = _db.OrderHeaders.Include(x => x.OrderDetails).OrderByDescending(x => x.Id);
               
                if (!string.IsNullOrEmpty(searchValue))
                {
                    Orders = Orders.Where(u => u.PickupEmail.Contains(searchValue) || u.PickupPhoneNumber.Contains(searchValue) || u.PickupName.Contains(searchValue));
                }
                if (!string.IsNullOrEmpty(Status))
                {
                    Orders = Orders.Where(u => u.Status.ToLower() == Status.ToLower());
                }
                if (!string.IsNullOrEmpty(UserId))
                {
                    Orders = Orders.Where(x => x.UserId == UserId);
                }
                Pagination pagination = new()
                {
                    CurrentPage = PageNumber,
                    PageSize = Pagesize,
                    TotalCount = Orders.Count(),
                };
                Response.Headers.Append("X-Pagination", JsonSerializer.Serialize(pagination));

                Orders = Orders.Skip((PageNumber - 1) * Pagesize).Take(Pagesize);

                _response.Result = Orders;
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
        [Authorize]
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
                var Order = _db.OrderHeaders.Include(x => x.OrderDetails).ThenInclude(x => x.MenuItem).FirstOrDefault(x => x.Id == Id);
                if(Order == null)
                {
                    _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    _response.IsSuccess = false;
                    _response.Errors.Add("The Id you provide does not exist !");
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
        [Authorize]

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
                    order.OrderDetails = null;
                    _response.Result = order;
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
        [Authorize]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //fromform because we need to upload an image not just a json data
        public async Task<ActionResult<ApiResponse>> UpdateOrder([FromBody] OrderHeaderUpdateDTO OrderUpDto, int id)
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
                    if (!string.IsNullOrEmpty(OrderUpDto.PickupName))
                    {
                        OrderDB.PickupName = OrderUpDto.PickupName;
                    }
                    if (!string.IsNullOrEmpty(OrderUpDto.PickupPhoneNumber))
                    {
                        OrderDB.PickupPhoneNumber = OrderUpDto.PickupPhoneNumber;
                    }
                    if (!string.IsNullOrEmpty(OrderUpDto.PickupEmail))
                    {
                        OrderDB.PickupEmail = OrderUpDto.PickupEmail;
                    }
                    if (!string.IsNullOrEmpty(OrderUpDto.Status))
                    {
                        OrderDB.Status = OrderUpDto.Status;
                    }
                    if (!string.IsNullOrEmpty(OrderUpDto.StripePaymentIntentID))
                    {
                        OrderDB.StripePaymentIntentID = OrderUpDto.StripePaymentIntentID;
                    }
                    _db.SaveChanges();
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.NoContent;
                    //_response.Result = OrderDB;
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
