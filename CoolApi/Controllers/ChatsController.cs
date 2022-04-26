using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoolApi.Database;
using CoolApiModels.Chats;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace CoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatsController : ControllerBase
    {
        private readonly CoolContext _context;

        public ChatsController(CoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Reads chats portion.", Description = "Reads chats descriptions sorted by last message sending time.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns data portion.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid query parameters values.")]
        public ActionResult<GetChatsModel> GetChats(
            [SwaggerParameter(Description = "Offset of portion."), FromQuery, Required, Range(0, int.MaxValue)] int offset,
            [SwaggerParameter(Description = "Portion size."), FromQuery, Required, Range(1, 30)] int portion)
        {
            const int totalCount = 322;
            var responsePortion = offset < totalCount ? (offset + portion < totalCount ? portion : totalCount - offset) : 0;

            var result = new GetChatsModel
            {
                Offset = offset,
                TotalCount = totalCount,
                Portion = responsePortion,
                Content = new List<ShortGetChatModel>(Enumerable
                    .Range(0, responsePortion)
                    .Select(x => new ShortGetChatModel
                    {
                        Id = Guid.Empty,
                        CreationTimeUtc = DateTime.MinValue
                    }))
            };

            return result;
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Reads chat description by ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns chat description.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        public ActionResult<GetChatModel> GetChat([SwaggerParameter(Description = "Chat ID.")] Guid id)
        {
            var result = new GetChatModel
            {
                Id = id,
                CreationTimeUtc = DateTime.MinValue,
                ChatMembersIds = new List<Guid>
                {
                    Guid.Empty,
                    Guid.Empty,
                    Guid.Empty
                }
            };

            return result;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates new chat.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns created chat description.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid operation.")]
        public ActionResult<GetChatModel> PostChat([SwaggerRequestBody(Description = "New chat details."), FromBody, Required] PostChatModel chat)r
        {
            return new GetChatModel { Id = chat.ReceiverId, CreationTimeUtc = DateTime.UtcNow };
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes chat.", Description = "Deletes chat and all messages in this chat.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, Description = "Chat is deleted.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        public IActionResult DeleteChat(
            [SwaggerParameter(Description = "Chat ID.")] Guid id,
            [SwaggerParameter(Description = "Must chat be deleted for all chat members."), FromQuery, Required] bool isForAll)
        {
            return NoContent();
        }
    }
}
