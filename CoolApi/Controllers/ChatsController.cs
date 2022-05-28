using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoolApi.Database;
using CoolApiModels.Chats;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using CoolApiModels.Users;
using Microsoft.AspNetCore.Authorization;
using CoolApi.Database.Repositories;
using CoolApi.Database.Models;
using CoolApi.Database.Options;

namespace CoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatsController : ControllerBase
    {
        private readonly IRepository<Chat, ReadPortionChatOptions, ReadChatOptions, CreateChatOptions, UpdateChatOptions, DeleteChatOptions> _repository;

        public ChatsController(CoolContext context)
        {
            _repository = new ChatRepository(context);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Reads chats portion.", Description = "Reads chats descriptions sorted by last message sending time.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns data portion.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid query parameters values.")]
        public ActionResult<ChatsPortionDetails> GetChats([SwaggerParameter(Description = "Offset of portion."), FromQuery, Required, Range(0, int.MaxValue)] int offset,
            [SwaggerParameter(Description = "Portion size."), FromQuery, Required, Range(1, 30)] int portion)
        {
            var userId = GetCurrentUserId();
            var readPortionOptions = new ReadPortionChatOptions
            {
                UserId = userId,
                Offset = offset,
                Portion = portion
            };
            var result = _repository.ReadPortion(readPortionOptions);
            var chats = result.DataCollection;

            var response = new ChatsPortionDetails
            {
                Offset = offset,
                Portion = chats.Count(),
                TotalCount = result.TotalCount,
                Content = chats.Select(c => new ChatShortDetails
                {
                    Id = c.Id,
                    CreationTimeUtc = c.CreationTimeUtc
                })
            };
            return response;
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Reads chat description by ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns chat description.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        public ActionResult<ChatDetails> GetChat([SwaggerParameter(Description = "Chat ID.")] Guid id)
        {
            var userId = GetCurrentUserId();
            var readOptions = new ReadChatOptions
            {
                UserId = userId,
                EntityId = id
            };
            var chat = _repository.Read(readOptions);

            if (chat == null)
                return NotFound();

            var response = new ChatDetails
            {
                Id = chat.Id,
                CreationTimeUtc = chat.CreationTimeUtc,
                ChatMembers = chat.ChatMembers.Select(m => new UserDetails
                {
                    Id = m.UserId,
                    Login = m.User.Login
                })
            };
            return response;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates new chat.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns created chat description.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid operation.")]
        public ActionResult<ChatDetails> PostChat([SwaggerRequestBody(Description = "New chat details."), FromBody, Required] NewChatDetails chat)
        {
            var userId = GetCurrentUserId();
            // ???
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes chat.", Description = "Deletes chat and all messages in this chat.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, Description = "Chat is deleted.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        public IActionResult DeleteChat([SwaggerParameter(Description = "Chat ID.")] Guid id,
            [SwaggerParameter(Description = "Must chat be deleted for all chat members."), FromQuery, Required] bool isForAll)
        {
            var userId = GetCurrentUserId();
            var deleteOptions = new DeleteChatOptions
            {
                UserId = userId,
                Id = id,
                IsForAll = isForAll
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
