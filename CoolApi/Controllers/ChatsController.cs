using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoolApi.Database;
using CoolApiModels.Chats;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using CoolApi.Database.Repositories;
using CoolApi.Database.Models;
using CoolApi.Database.Models.Extensions;
using CoolApi.Extensions;

namespace CoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatsController : ControllerBase
    {
        private readonly IRepository<Chat> _repository;

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
            var userId = this.GetCurrentUserId();
            var result = _repository.ReadPortion(offset, portion, (chat) => true, userId);

            var response = result.GetDto();
            return response;
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Reads chat description by chat ID (or other user in chat ID).")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns chat description.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        public ActionResult<ChatDetails> GetChat([SwaggerParameter(Description = "Chat ID.")] Guid id)
        {
            var userId = this.GetCurrentUserId();
            var chat = _repository.Read(id, userId);

            if (chat == null)
                return NotFound();

            var response = chat.GetDto();
            return response;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates new chat.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns created chat description.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid operation.")]
        public ActionResult<ChatDetails> PostChat([SwaggerRequestBody(Description = "New chat details."), FromBody, Required] NewChatDetails newChatDetails)
        {
            var userId = this.GetCurrentUserId();
            var newChat = new Chat
            {
                ChatMembers = new ChatMember[]
                {
                    new ChatMember {UserId = userId},
                    new ChatMember {UserId = newChatDetails.ReceiverId}
                }
            };

            try
            {
                var newChatId = _repository.Create(newChat, userId);
                var createdChat = _repository.Read(newChatId, userId);
                if (createdChat == null)
                    return BadRequest("no chat");

                var response = createdChat.GetDto();
                return response;
            }
            catch (Exception exception)
            {
                var error = exception.GetProblemDetails();
                return BadRequest(error);
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes chat.", Description = "Deletes chat and all messages in this chat.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, Description = "Chat is deleted.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        public IActionResult DeleteChat([SwaggerParameter(Description = "Chat ID.")] Guid id,
            [SwaggerParameter(Description = "Must chat be deleted for all chat members."), FromQuery, Required] bool isForAll)
        {
            var userId = this.GetCurrentUserId();
            try
            {
                if (isForAll)
                    _repository.Delete(id, userId);
                else
                    _repository.Hide(id, userId);
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
