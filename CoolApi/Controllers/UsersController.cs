using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoolApi.Database;
using CoolApiModels.Users;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using CoolApi.Database.Repositories;
using CoolApi.Database.Models;
using CoolApi.Database.Options;
using System.Linq;
using CoolApi.Database.Hashers;

namespace CoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IPasswordHasher _passwordHasher = new SHA256PasswordHasher();

        private readonly IRepository<User, ReadPortionUserOptions, ReadUserOptions, CreateUserOptions, UpdateUserOptions, DeleteUserOptions> _repository;

        public UsersController(CoolContext context)
        {
            _repository = new UserRepository(context);
        }

        [HttpGet]
        [Authorize]
        [SwaggerOperation(Summary = "Reads users portion.", Description = "Reads users portion according to the query params.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns data portion.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid query parameters values.")]
        public ActionResult<UsersPortionDetails> GetUsers([SwaggerParameter(Description = "Offset of portion."), FromQuery, Required, Range(0, int.MaxValue)] int offset,
            [SwaggerParameter(Description = "Portion size."), FromQuery, Required, Range(1, 50)] int portion,
            [SwaggerParameter(Description = "String to search by user login."), FromQuery, StringLength(32)] string searchString)
        {
            var options = new ReadPortionUserOptions
            {
                Offset = offset,
                Portion = portion,
                LoginSubstring = searchString
            };
            var result = _repository.ReadPortion(options);
            var users = result.DataCollection;

            var responseContent = users.Select(u => GetDto(u));
            var response = new UsersPortionDetails
            {
                Offset = offset,
                Portion = users.Count(),
                TotalCount = result.TotalCount,
                Content = responseContent
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
            var options = new ReadUserOptions
            {
                EntityId = id
            };
            var user = _repository.Read(options);
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
            var user = _repository.GetSingleOrDefault(u => u.Login == login);
            if (user == null)
                return BadRequest();

            var isPasswordValid = _passwordHasher.VerifyPassword(user.PasswordHash, password);
            if (!isPasswordValid)
                return BadRequest();

            return NoContent();
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
            if (newLogin == null && newPassword == null)
                return BadRequest();

            var currentUserId = GetCurrentUserId();
            var readOptions = new ReadUserOptions
            {
                EntityId = currentUserId
            };
            var user = _repository.Read(readOptions);
            if (user == null)
                return BadRequest();

            var isPasswordValid = _passwordHasher.VerifyPassword(user.PasswordHash, userNewDetails.CurrentPassword);
            if (!isPasswordValid)
                return BadRequest();

            if (newLogin != null)
                user.Login = newLogin;

            if (newPassword != null)
            {
                var newPasswordHash = _passwordHasher.GetPasswordHash(newPassword);
                user.PasswordHash = newPasswordHash;
            }

            var updateOptions = new UpdateUserOptions
            {
                Entity = user
            };
            try
            {
                _repository.Update(updateOptions);
                _repository.Save();
            }
            catch (Exception exception)
            {
                var error = GetProblemDetails(exception);
                return BadRequest(error);
            }

            var updatedUser = _repository.Read(readOptions);
            var response = GetDto(updatedUser);
            return response;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates new user.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns created user profile info.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid operation.")]
        public ActionResult<UserDetails> PostUser([SwaggerRequestBody(Description = "New user details."), FromBody, Required] NewUserDetails newUserDetails)
        {
            var newUserId = Guid.NewGuid();
            var passwordHash = _passwordHasher.GetPasswordHash(newUserDetails.Password);
            var newUser = new User
            {
                Id = newUserId,
                Login = newUserDetails.Login,
                PasswordHash = passwordHash
            };

            var options = new CreateUserOptions
            {
                Entity = newUser
            };
            try
            {
                _repository.Create(options);
                _repository.Save();
            }
            catch (Exception exception)
            {
                var error = GetProblemDetails(exception);
                return BadRequest(error);
            }

            var readOptions = new ReadUserOptions
            {
                EntityId = newUserId
            };
            var createdUser = _repository.Read(readOptions);
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
            var readOptions = new ReadUserOptions
            {
                EntityId = currentUserId
            };
            var user = _repository.Read(readOptions);
            if (user == null)
                return BadRequest();

            var isPasswordValid = _passwordHasher.VerifyPassword(user.PasswordHash, confirmationDetails.CurrentPassword);
            if (!isPasswordValid)
                return BadRequest();

            var deleteOptions = new DeleteUserOptions
            {
                Id = currentUserId
            };
            try
            {
                _repository.Delete(deleteOptions);
                _repository.Save();
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
