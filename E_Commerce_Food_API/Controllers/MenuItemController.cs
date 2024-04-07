using AutoMapper;
using E_Commerce_Food_API.Data;
using E_Commerce_Food_API.Models;
using E_Commerce_Food_API.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Runtime.Intrinsics.Arm;

namespace E_Commerce_Food_API.Controllers
{
    [Route("api/MenuItem")]
    [ApiController]
    public class MenuItemController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ApiResponse _response;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        public MenuItemController(AppDbContext db , IMapper mapper , IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _response = new ApiResponse();
            _mapper = mapper;
            _WebHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAll()
        {
            _response.Result =  _db.MenuItems;
                //.Select(x => new { x.Id, x.Name, x.Description, x.Category, x.Price, x.SpecialTag });
            _response.StatusCode = System.Net.HttpStatusCode.OK;

            return Ok(_response);
        }

        [Authorize]
        [HttpGet("{id:int}", Name ="GetMenuItem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetMenuItem(int id)
        {
            if (id == 0)
            {
                _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                return BadRequest(_response);
            }
            MenuItem menuItem = await _db.MenuItems.FirstOrDefaultAsync(x => x.Id == id);
            if (menuItem == null)
            {
                _response.StatusCode = System.Net.HttpStatusCode.NotFound;
                _response.IsSuccess = false;
                return NotFound(_response);
            }
            _response.Result = menuItem;
            //    new
            //{
            //    menuItem.Name,
            //    Description = menuItem.Description,
            //    Category = menuItem.Category,
            //    SpecialTag = menuItem.SpecialTag,
            //    Id = menuItem.Id,
            //    Price = menuItem.Price
            //};
          
            _response.StatusCode = System.Net.HttpStatusCode.OK;    
            return Ok(_response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //fromform because we need to upload an image not just a json data
        public async Task<ActionResult<ApiResponse>> CreateMenuItem([FromForm] MenuItemCreateDto MenuCreate)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (MenuCreate.ImageFile == null || MenuCreate.ImageFile.Length == 0)
                    {
                        return BadRequest(ModelState);
                    }
                    MenuItem menuItem = _mapper.Map<MenuItem>(MenuCreate);
                    //byte[] fileBytes;
                    //using (var memoryStream = new MemoryStream())
                    //{
                    //    await MenuCreate.ImageFile.CopyToAsync(memoryStream);
                    //    fileBytes = memoryStream.ToArray();
                    //}
                    //menuItem.Image = fileBytes;
                    if (MenuCreate.ImageFile != null)
                    {
                        string rootPath = _WebHostEnvironment.WebRootPath;
                        string filename = Guid.NewGuid().ToString() + Path.GetFileName(MenuCreate.ImageFile.FileName);

                        var ItemPath = Path.Combine(rootPath, @"Images\MenuItems");
                       
                        using (FileStream stream = new FileStream(Path.Combine(ItemPath, filename), FileMode.Create))
                        {
                            //Copies the contents of the uploaded file to the target stream.
                            MenuCreate.ImageFile.CopyTo(stream);
                        }
                        menuItem.Image = @"https://localhost:7034\Images\MenuItems\" + filename;
                    }
                    else
                    {
                        //we should add some logic in here 
                    }

                    _db.MenuItems.Add(menuItem);
                    _db.SaveChanges();
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.Created;
                    _response.Result = menuItem;
                    return CreatedAtRoute("GetMenuItem", new { id = menuItem.Id }, _response);
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
                _response.StatusCode = HttpStatusCode.BadRequest;
            }

            return _response;
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //fromform because we need to upload an image not just a json data
        public async Task<ActionResult<ApiResponse>> UpdateMenuItem([FromForm] MenuItemUpdateDto menuUpdate, int id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (id == 0 || menuUpdate == null || id != menuUpdate.Id)
                    {
                        return BadRequest(ModelState);
                    }
                    MenuItem menuDb = await _db.MenuItems.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
                    if (menuDb == null)
                    {
                        return BadRequest();
                    }
                    menuDb = _mapper.Map<MenuItem>(menuUpdate);
                    if (menuUpdate.ImageFile != null || menuUpdate.ImageFile.Length != 0)
                    {
                        //byte[] fileBytes;
                        //using (var memoryStream = new MemoryStream())
                        //{
                        //    await menuUpdate.ImageFile.CopyToAsync(memoryStream);
                        //    fileBytes = memoryStream.ToArray();
                        //}
                        //menuDb.Image = fileBytes;
                        string rootPath = _WebHostEnvironment.WebRootPath;
                        string filename = Guid.NewGuid().ToString() + Path.GetFileName(menuUpdate.ImageFile.FileName);

                        var ItemPath = Path.Combine(rootPath, @"Images\MenuItems");
                       
                        using (FileStream stream = new FileStream(Path.Combine(ItemPath,filename ),FileMode.Create))
                        {
                            //Copies the contents of the uploaded file to the target stream.
                            menuUpdate.ImageFile.CopyTo(stream);
                        }
                        menuDb.Image = @"https://localhost:7034\images\Products\" + filename;
                        
                    }
                    _db.MenuItems.Update(menuDb);
                    _db.SaveChanges();
                    _response.IsSuccess = true;
                    _response.StatusCode = HttpStatusCode.NoContent;
                    _response.Result = menuDb;
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
                _response.StatusCode = HttpStatusCode.BadRequest;
            }
            return _response;
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //fromform because we need to upload an image not just a json data
        public async Task<ActionResult<ApiResponse>> DeleteMenuItem(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }
                MenuItem menuDb = await _db.MenuItems.FindAsync(id);
                if (menuDb == null)
                {
                    return BadRequest();
                }

                //Thread.Sleep(2000);
                _db.MenuItems.Remove(menuDb);
                _db.SaveChanges();
                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors.Add(ex.Message);
                _response.StatusCode = HttpStatusCode.BadRequest;
            }
            return _response;
        }
    }
}
