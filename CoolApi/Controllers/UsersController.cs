using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoolApi.Database;
using CoolApiModels.Users;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using CoolApi.Database.Models;
using System.Linq;
using CoolApi.Database.Hashers;
using CoolApi.Database.Identity;

namespace CoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager _userManager;

        public UsersController(CoolContext context)
        {
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
            var content = users.Select(u => GetDto(u));
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

            var response = GetDto(user);
            return response;
        }

        [HttpGet("auth")]
        [SwaggerOperation(Summary = "Performs user authentication.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, Description = "Successful authentication.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Authentication error (wrong login or password).")]
        public IActionResult Authenticate([SwaggerParameter(Description = "User login."), FromQuery, Required] string login,
            [SwaggerParameter(Description = "User password."), FromQuery, Required] string password)
        {
            var isPasswordValid = _userManager.VerifyPassword(login, password);
            if (isPasswordValid)
                return NoContent();

            return BadRequest();
        }

        [HttpPut("{id}")]
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

            var currentUserId = GetCurrentUserId();

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
                var error = GetProblemDetails(exception);
                return BadRequest(error);
            }

            var updatedUser = _userManager.FindById(currentUserId);
            if (updatedUser == null)
                return BadRequest("no user");
            var response = GetDto(updatedUser);
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
                var error = GetProblemDetails(exception);
                return BadRequest(error);
            }

            var createdUser = _userManager.FindById(newUserId);
            if (createdUser == null)
                return BadRequest("no user");
            var response = GetDto(createdUser);
            return response;
        }

        [HttpDelete("{id}")]
        [Authorize]
        [SwaggerOperation(Summary = "Deletes user.", Description = "Deletes user profile, chats and all messages. All messages of other users from chats with this user are also deleted.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, Description = "User is deleted.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid operation.", Type = typeof(ValidationProblemDetails))]
        public IActionResult DeleteUser(UserConfirmationDetails confirmationDetails)
        {
            var currentUserId = GetCurrentUserId();

            var isDeleted = _userManager.Delete(currentUserId, confirmationDetails.CurrentPassword);
            if (!isDeleted)
                return BadRequest();

            try
            {
                _userManager.SaveChanges();
            }
            catch (Exception exception)
            {
                var error = GetProblemDetails(exception);
                return BadRequest(error);
            }

            return NoContent();
        }

        private static UserDetails GetDto(User user)
        {
            var dto = new UserDetails
            {
                Id = user.Id,
                Login = user.Login
            };

            return dto;
        }

        private static ProblemDetails GetProblemDetails(Exception exception)
        {
            var details = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Detail = exception.Message,
                Status = StatusCodes.Status400BadRequest
            };

            return details;
        }

        private Guid GetCurrentUserId()
        {
            var idValue = User.Claims.Single(c => c.Type == "id").Value;

            return Guid.Parse(idValue);
        }
    }
}
