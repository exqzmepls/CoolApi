using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using CoolApi.Database;
using CoolApi.Database.Hashers;
using CoolApi.Database.Identity;
using CoolApi.Database.Models.Extensions;
using CoolApiModels.Users;
using CoolApi.Extensions;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Collections.Generic;

namespace CoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager _userManager;
        private readonly IConfiguration _configuration;

        public UsersController(CoolContext context, IConfiguration configuration)
        {
            _configuration = configuration;
            var passwordHasher = new SHA256PasswordHasher();
            _userManager = new UserManager(context, passwordHasher);
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Reads users portion.", Description = "Reads users portion according to the query params.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns users portion ordered by login.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid query parameters values.")]
        public ActionResult<UsersPortionDetails> GetUsers([SwaggerParameter(Description = "Offset of portion."), FromQuery, Required, Range(0, int.MaxValue)] int offset,
            [SwaggerParameter(Description = "Portion size."), FromQuery, Required, Range(1, 50)] int portion,
            [SwaggerParameter(Description = "String to search by user login."), FromQuery, StringLength(32)] string searchString)
        {
            var usersCollection = _userManager.FindByLogin(offset, portion, searchString);

            var users = usersCollection.Collection;
            var content = users.Select(u => u.GetDto());
            var response = new UsersPortionDetails
            {
                Offset = offset,
                Portion = users.Count(),
                TotalCount = usersCollection.TotalCount,
                Content = content
            };
            return response;
        }

        [HttpGet("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Reads user profile info by ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns user profile info.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        public ActionResult<UserDetails> GetUser([SwaggerParameter(Description = "User ID.")] Guid id)
        {
            var user = _userManager.FindById(id);
            if (user == null)
                return NotFound();

            var response = user.GetDto();
            return response;
        }

        [HttpGet("auth")]
        [SwaggerOperation(Summary = "Performs user authentication.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Successful authentication.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Authentication error (wrong login or password).")]
        public ActionResult<AuthenticatedResult> Authenticate([SwaggerParameter(Description = "User login."), FromQuery, Required] string login,
            [SwaggerParameter(Description = "User password."), FromQuery, Required] string password)
        {
            var isPasswordValid = _userManager.VerifyPassword(login, password, out var userId);
            if (!isPasswordValid)
                return BadRequest();

            var claims = new List<Claim> { new Claim("id", userId.ToString()) };
            var exp = DateTime.UtcNow.Add(TimeSpan.FromHours(6));
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var signingCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                claims: claims,
                expires: exp,
                signingCredentials: signingCredentials);
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            var authenticatedResult = new AuthenticatedResult
            {
                UserId = userId,
                Token = token
            };
            return authenticatedResult;
        }

        [HttpPut]
        [Authorize]
        [SwaggerOperation(Summary = "Updates user details.", Description = "User can change their Login or/and Password.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns user updated profile info.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid operation.", Type = typeof(ValidationProblemDetails))]
        public ActionResult<UserDetails> PutUser([SwaggerRequestBody(Description = "User new details."), FromBody, Required] UserNewDetails userNewDetails)
        {
            var newLogin = userNewDetails.NewLogin;
            var newPassword = userNewDetails.NewPassword;
            var currentPassword = userNewDetails.CurrentPassword;
            if (newLogin == null && newPassword == null)
                return BadRequest();

            var currentUserId = this.GetCurrentUserId();

            if (newLogin != null)
            {
                var isLoginChanged = _userManager.ChangeLogin(currentUserId, newLogin, currentPassword);
                if (!isLoginChanged)
                    return BadRequest();
            }

            if (newPassword != null)
            {
                var isPasswordChanged = _userManager.ChangePassword(currentUserId, newPassword, currentPassword);
                if (!isPasswordChanged)
                    return BadRequest();
            }

            try
            {
                _userManager.SaveChanges();
            }
            catch (Exception exception)
            {
                var error = exception.GetProblemDetails();
                return BadRequest(error);
            }

            var updatedUser = _userManager.FindById(currentUserId);
            if (updatedUser == null)
                return BadRequest("no user");
            var response = updatedUser.GetDto();
            return response;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates new user.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns created user profile info.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid operation.")]
        public ActionResult<UserDetails> PostUser([SwaggerRequestBody(Description = "New user details."), FromBody, Required] NewUserDetails newUserDetails)
        {
            var newUserId = _userManager.Create(newUserDetails.Login, newUserDetails.Password);

            try
            {
                _userManager.SaveChanges();
            }
            catch (Exception exception)
            {
                var error = exception.GetProblemDetails();
                return BadRequest(error);
            }

            var createdUser = _userManager.FindById(newUserId);
            if (createdUser == null)
                return BadRequest("no user");
            var response = createdUser.GetDto();
            return response;
        }

        [HttpDelete]
        [Authorize]
        [SwaggerOperation(Summary = "Deletes user.", Description = "Deletes user profile, chats and all messages. All messages of other users from chats with this user are also deleted.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, Description = "User is deleted.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid operation.", Type = typeof(ValidationProblemDetails))]
        public IActionResult DeleteUser(UserConfirmationDetails confirmationDetails)
        {
            var currentUserId = this.GetCurrentUserId();

            var isDeleted = _userManager.Delete(currentUserId, confirmationDetails.CurrentPassword);
            if (!isDeleted)
                return BadRequest();

            try
            {
                _userManager.SaveChanges();
            }
            catch (Exception exception)
            {
                var error = exception.GetProblemDetails();
                return BadRequest(error);
            }

            return NoContent();
        }
    }
}
