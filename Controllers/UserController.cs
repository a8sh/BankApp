using AutoMapper;
using BankApp.DTOs;
using BankApp.Exceptions;
using BankApp.Models;
using BankApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace BankApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IConfiguration configuration, IMapper mapper)
        {
            _userService = userService;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAllUser()
        {
            List<User> users = _userService.GetAllUsers();
            List<UserSendDTO> userSendDTOs = _mapper.Map<List<UserSendDTO>>(users);
            return Ok(userSendDTOs);
        }
        [HttpGet("{id:Guid}")]
        public IActionResult GetUserById(Guid id)
        {
            try
            {
                var getUser = _userService.GetUserById(id);
                UserSendDTO userSendDTO = _mapper.Map<UserSendDTO>(getUser);
                return Ok(getUser);
            }
            catch (UserNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message, statusCode = (int)HttpStatusCode.NotFound });
            }
        }
        [HttpGet("{name}")]
        public IActionResult GetUserByName(string name)
        {
            try
            {
                var getUser = _userService.GetUserByName(name);
                UserSendDTO userSendDTO = _mapper.Map<UserSendDTO>(getUser);
                return Ok(getUser);
            }
            catch (UserNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message, statusCode = (int)HttpStatusCode.NotFound });
            }
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddUser([FromForm] UserSignUpDTO userSignUp)
        {
            try
            {
                if (userSignUp.ImageFile == null)
                {
                    userSignUp.FilePath = "images/default.jpg";
                }
                else if (userSignUp.ImageFile.Length == 0)
                {
                    return BadRequest("Please upload valid user profile photo");
                }
                else
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(userSignUp.ImageFile.FileName)}";
                    var imagePath = Path.Combine("wwwroot", "images", fileName);

                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await userSignUp.ImageFile.CopyToAsync(fileStream);
                    }
                    userSignUp.FilePath = $"images/{fileName}";
                }
                var user = _mapper.Map<User>(userSignUp);
                _userService.AddUser(user);
                return CreatedAtAction(nameof(GetAllUser), new { id = user.UserId },
                    new { message = $"User account created successfully with user id : {user.UserId}" });
            }
            catch (InvalidUserException ex)
            {
                return BadRequest(new { message = ex.Message, statusCode = (int)HttpStatusCode.BadRequest });
            }
            catch (UserEmailAlreadyExistException ex)
            {
                return Conflict(new { message = ex.Message, statusCode = (int)HttpStatusCode.Conflict });
            }
            catch (UserPhoneNumberAlreadyExistException ex)
            {
                return Conflict(new { message = ex.Message, statusCode = (int)HttpStatusCode.Conflict });
            }
        }
        [HttpDelete("{id:Guid}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _userService.DeleteUser(id);
                return Ok("User deleted successfully");
            }
            catch (UserNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message, statusCode = (int)HttpStatusCode.NotFound });
            }
        }
        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateProfile(Guid id, [FromForm] UserUpdateDto userUpdateDto)
        {
            try
            {
                // Check if the user exists
                var existingUser = _userService.GetUserById(id);

                if (existingUser == null)
                {
                    throw new UserNotFoundException($"User with ID {id} not found.");
                }

                // Handle file upload if provided
                if (userUpdateDto.ImageFile != null && userUpdateDto.ImageFile.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(userUpdateDto.ImageFile.FileName)}";
                    var imagePath = Path.Combine("wwwroot", "images", fileName);

                    using (var fileStream = new FileStream(imagePath, FileMode.Create))
                    {
                        await userUpdateDto.ImageFile.CopyToAsync(fileStream);
                    }

                    userUpdateDto.FilePath = $"images/{fileName}";
                }
                else
                {
                    userUpdateDto.FilePath = existingUser.FilePath;  // Keep the existing file path if no new image is provided
                }

                // Map the updated fields to the existing user
                _mapper.Map(userUpdateDto, existingUser);

                // Save the updated user
                _userService.UpdateUser(existingUser);

                return Ok(new { message = "User profile updated successfully." });
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { message = ex.Message, statusCode = (int)HttpStatusCode.NotFound });
            }
            catch (InvalidUserException ex)
            {
                return BadRequest(new { message = ex.Message, statusCode = (int)HttpStatusCode.BadRequest });
            }
            catch (Exception ex)
            {
                // Log the exception (you should log exceptions in a real application)
                Console.WriteLine(ex);
                return StatusCode((int)HttpStatusCode.InternalServerError, new { message = "An error occurred while updating the user.", statusCode = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost("login")]
        public IActionResult Login(UserLoginDTO userLogInDto)
        {
            try
            {
                User getUser = _userService.GetUserByEmail(userLogInDto.Email);
                if (BCrypt.Net.BCrypt.Verify(userLogInDto.Password, getUser.Password))
                {
                    var token = CreateToken(getUser);
                    Response.Headers.Add("JWT", JsonConvert.SerializeObject(token));
                    var userSendDto = _mapper.Map<UserSendDTO>(getUser);
                    return Ok(userSendDto);
                }
                else
                {
                    return BadRequest("username/password does not match");
                }
            }
            catch (UserNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message, statusCode = (int)HttpStatusCode.NotFound });
            }
        }
        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User")
            };

            var t = Encoding.UTF32.GetBytes(_configuration.GetSection("AppSettings:Key").Value);
            var key = new SymmetricSecurityKey(t);

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;

        }
        [HttpPut("{id:Guid}")]
        public IActionResult ActivateUser(Guid id)
        {
            User getuser = _userService.ActiveUser(id);
            return Ok(getuser);
        }
    }
}
