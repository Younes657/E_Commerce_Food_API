using AutoMapper;
using E_Commerce_Food_API.Data;
using E_Commerce_Food_API.Models;
using E_Commerce_Food_API.Models.DTO;
using E_Commerce_Food_API.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace E_Commerce_Food_API.Controllers
{
    [Route("api/Authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ApiResponse _response;
        private readonly string SecretKey;
        private UserManager<ApplicationUser> _userManger;
        private RoleManager<IdentityRole> _roleManager;
        public AuthenticationController(AppDbContext db, IMapper mapper , IConfiguration configuration, RoleManager<IdentityRole> roleManager , UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _response = new ApiResponse();
            SecretKey = configuration.GetValue<string>("ApiSettings:SecretKey") ?? throw new NullReferenceException();
            _roleManager = roleManager;
            _userManger = userManager;
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser? userFromDb = await _userManger.FindByEmailAsync(model.Email);
                if (userFromDb != null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    _response.Errors.Add("User already exist ! try to login in");
                    return BadRequest(_response);
                }

                ApplicationUser NewUser = new()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    Name = model.Name,
                    PhoneNumber = string.IsNullOrEmpty(model.PhoneNumber) ? "" : model.PhoneNumber,
                };
                try
                {
                    IdentityResult result = await _userManger.CreateAsync(NewUser, model.Password);

                    if (result.Succeeded)
                    {
                        if (!_roleManager.RoleExistsAsync(SD.Role_admin).GetAwaiter().GetResult())
                        {
                            await _roleManager.CreateAsync(new IdentityRole(SD.Role_customer));
                            await _roleManager.CreateAsync(new IdentityRole(SD.Role_admin));
                        }
                        if (model.Role.ToLower() == SD.Role_admin.ToLower())
                            await _userManger.AddToRoleAsync(NewUser, SD.Role_admin);
                        else
                            await _userManger.AddToRoleAsync(NewUser, SD.Role_customer);

                        _response.IsSuccess = true;
                        _response.StatusCode = HttpStatusCode.OK;
                        return Created("" , _response);
                    }
                }
                catch (Exception ex)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Errors.Add(ex.Message);
                }
                return BadRequest(_response);
            } else
            {
                _response.IsSuccess = false;
                _response.Errors = ModelState.SelectMany(x => x.Value.Errors.Select(p => p.ErrorMessage)).ToList();
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
        }
        [HttpPost("Login")]
        public async Task<ActionResult<ApiResponse>> Login([FromBody] LoginRequestDto model)
        {
            //Console.WriteLine(SecretKey);
            var UserfromDb = _db.ApplicationUsers.FirstOrDefault(x => x.UserName == model.UserName || x.Email == model.Email);
            if (UserfromDb == null)
            {
                _response.IsSuccess = false;
                _response.Result = new LoginResponseDto();
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("UserName Or Email That you provide is not valid");
                return BadRequest(_response);
            }
            var result = await _userManger.CheckPasswordAsync(UserfromDb, model.Password);
            if(result == false)
            {
                _response.IsSuccess = false;
                _response.Result = new LoginResponseDto();
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("Your Password is incorrect Check again");
                return BadRequest(_response);
            }
            //generate the jwt token
            var role = await _userManger.GetRolesAsync(UserfromDb);
            JwtSecurityTokenHandler TokenHandler = new();
            byte[] key = Encoding.ASCII.GetBytes(SecretKey);
            SecurityTokenDescriptor TokenDesc = new()
            {
                Subject= new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, UserfromDb.Name),
                    new Claim(ClaimTypes.Email, UserfromDb.Email),
                    new Claim(ClaimTypes.Role, role.FirstOrDefault()),
                    new Claim("sub", UserfromDb.Id)
                }),
                Expires = DateTime.UtcNow.AddDays(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken Token = TokenHandler.CreateToken(TokenDesc);

            LoginResponseDto response = new()
            {
                Email = UserfromDb.Email ?? "",
                Token = TokenHandler.WriteToken(Token),
            };
            if(response.Email == "" || string.IsNullOrEmpty(response.Token))
            {
                _response.IsSuccess = false;
                _response.Result = new LoginResponseDto();
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.Errors.Add("UserName or Password is incorrect !!");
                return BadRequest(_response);
            }
            _response.IsSuccess = true;
            _response.Result = response;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }
    }
}
