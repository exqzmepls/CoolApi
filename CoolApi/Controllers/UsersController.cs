using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoolApi.Database;
using CoolApiModels.Users;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace CoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly CoolContext _context;

        public UsersController(CoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Reads users portion.", Description = "Reads users portion according to the query params.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns data portion.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid query parameters values.")]
        public ActionResult<GetUsersModel> GetUsers(
            [SwaggerParameter(Description = "Offset of portion."), FromQuery, Required, Range(0, int.MaxValue)] int offset,
            [SwaggerParameter(Description = "Portion size."), FromQuery, Required, Range(1, 50)] int portion,
            [SwaggerParameter(Description = "String to search by user login."), FromQuery, StringLength(32)] string searchString)
        {
            return new GetUsersModel
            {
                Offset = offset,
                Portion = portion,
                TotalCount = 999,
                Content = new List<GetUserModel>
                {
                    new GetUserModel{Id = Guid.Empty, Login = "user1"},
                    new GetUserModel{Id = Guid.Empty, Login = "user2"}
                }
            };
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Reads user profile info by ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns user profile info.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        public ActionResult<GetUserModel> GetUser([SwaggerParameter(Description = "User ID.")] Guid id)
        {
            return new GetUserModel { Id = id, Login = "user" };
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Updates user details.", Description = "User can change their Login or/and Password.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns user updated profile info.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid operation.")]
        public ActionResult<GetUserModel> PutUser(
            [SwaggerParameter(Description = "User ID.")] Guid id,
            [SwaggerRequestBody(Description = "User new details."), FromBody, Required] PutUserModel user)
        {
            return new GetUserModel { Id = Guid.Empty, Login = "newLogin" };
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates new user.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns created user profile info.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid operation.")]
        public ActionResult<GetUserModel> PostUser([SwaggerRequestBody(Description = "New user details."), FromBody, Required] PostUserModel user)
        {
            return new GetUserModel { Id = Guid.Empty, Login = user.Login };
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes user.", Description = "Deletes user profile, chats and all messages. All messages of other users from chats with this user are also deleted.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, Description = "User is deleted.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        public IActionResult DeleteUser([SwaggerParameter(Description = "User ID.")] Guid id)
        {
            return NoContent();
        }
    }
}
