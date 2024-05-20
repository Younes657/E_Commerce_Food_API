using E_Commerce_Food_API.Data;
using E_Commerce_Food_API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace E_Commerce_Food_API.Controllers
{
    [Route("api/ShoppingCart")]
    [ApiController]
    [Authorize]
    public class ShoppingCartController : ControllerBase
    {
        private readonly AppDbContext _db;
        protected ApiResponse _response;
        public ShoppingCartController(AppDbContext db)
        {
            _db = db;
            _response = new ApiResponse();
        }
        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> GetShopCart([FromRoute] string userId)
        {
            try
            {
                ShoppingCart? ShopCart;
                if (string.IsNullOrEmpty(userId))
                {
                    ShopCart = new();
                }
                else
                {
                     ShopCart = await _db.ShoppingCarts.Include(x => x.CartItems).ThenInclude(x => x.MenuItem).FirstOrDefaultAsync(x => x.UserId == userId);

                }
                
                //    .Select(x => new
                //{
                //    x.UserId,
                //    x.Id,
                //    x.Total,
                //    CartItems = x.CartItems.Select(u => new
                //    {
                //        u.Id,
                //        u.MenuItemId,
                //        MenuItem = new MenuItem()
                //        {
                //            Id = u.MenuItem.Id,
                //            Name = u.MenuItem.Name,
                //            Description = u.MenuItem.Description,
                //            SpecialTag= u.MenuItem.SpecialTag,
                //            Category= u.MenuItem.Category,
                //            Price = u.MenuItem.Price
                //        },
                //        u.Quantity,
                //        u.ShoppingCartId
                //    })
                //});
                if (ShopCart != null && ShopCart.CartItems != null && ShopCart.CartItems.Count > 0)
                    ShopCart.Total = ShopCart.CartItems.Sum(u => u.Quantity * u.MenuItem.Price);
                
                _response.IsSuccess = true;
                _response.Result = ShopCart;
                _response.StatusCode = System.Net.HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _response.Errors.Add(ex.Message);
                return BadRequest(_response);
            }
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> Add_Update_ShoppingCart(string UserId,
            int MenuId, int UpQuaBy )
        {
            // Shopping cart will have one entry per user id, even if a user has many items in cart.
            // Cart items will have all the items in shopping cart for a user
            // updatequantityby will have count by with an items quantity needs to be updated
            // if it is -1 that means we have lower a count if it is 5 it means we have to add 5 count to existing count.
            // if updatequantityby by is 0, item will be removed

            // when a user adds a new item to a new shopping cart for the first time
            // when a user adds a new item to an existing shopping cart (basically user has other items in cart)
            // when a user updates an existing item count
            // when a user removes an existing item
            var shopCart =await _db.ShoppingCarts.Include(x => x.CartItems).FirstOrDefaultAsync(x => x.UserId == UserId);
            var menuitem =await _db.MenuItems.FirstOrDefaultAsync(x => x.Id == MenuId);
            if ( menuitem == null )
            {
                _response.IsSuccess = false;
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _response.Errors.Add("Menu Item does not exist !");
                return BadRequest(_response);
            }
            if( shopCart == null && UpQuaBy > 0 )
            {
                shopCart = new ShoppingCart { UserId = UserId };
                await _db.ShoppingCarts.AddAsync(shopCart);
                _db.SaveChanges();
                var cartItem = new ItemCart()
                {
                    MenuItemId = menuitem.Id,
                    Quantity = UpQuaBy,
                    ShoppingCartId = shopCart.Id
                };
                await _db.ItemCarts.AddAsync(cartItem);
                _db.SaveChanges();
            }
            else 
            {
                if (shopCart != null)
                {
                    ItemCart? CartItemShop = shopCart.CartItems
                        .FirstOrDefault(x => x.MenuItemId == MenuId);
                    if (CartItemShop == null)
                    {//item does not exist in the current shopcart
                        CartItemShop = new ItemCart()
                        {
                            MenuItemId = menuitem.Id,
                            Quantity = UpQuaBy,
                            ShoppingCartId = shopCart.Id
                        };
                        await _db.ItemCarts.AddAsync(CartItemShop);
                        _db.SaveChanges();
                    }
                    else
                    {//item already exist and we have to update quantity
                        var newQuantity = CartItemShop.Quantity + UpQuaBy;
                        if (newQuantity <= 0 || UpQuaBy == 0)
                        {
                            _db.ItemCarts.Remove(CartItemShop);
                            if (shopCart.CartItems.Count() == 1) //we can not set it to 0 because we did not save the changes yet in the database
                            {
                                _db.ShoppingCarts.Remove(shopCart);
                            }
                            _db.SaveChanges();
                        }
                        else
                        {
                            CartItemShop.Quantity = newQuantity;
                            _db.SaveChanges();
                        }
                    }
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    _response.Errors.Add("The Quantity should not be less then 0 in this case , check again");
                    return BadRequest(_response);
                }
            }
            _response.IsSuccess = true;
            _response.StatusCode = System.Net.HttpStatusCode.OK;
            return _response;
        }
    }
}
