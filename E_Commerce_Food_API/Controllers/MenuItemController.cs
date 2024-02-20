using E_Commerce_Food_API.Data;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Food_API.Controllers
{
    public class MenuItemController : ControllerBase
    {
        private readonly AppDbContext _db;
        public MenuItemController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAll()
        {
            return Ok(_db.MenuItems.Select(x => new { x.Id, x.Name, x.Description,x.Category, x.Price, x.SpecialTag }));
        }

    }
}
