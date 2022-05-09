using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoolApi.Database;
using CoolApiModels.Messages;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;

namespace CoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly CoolContext _context;

        public MessagesController(CoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Reads messages portion.", Description = "Reads messages portion according to the query params and sorted by sending time.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns data portion.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid query parameters values.")]
        public ActionResult<MessagesPortionDetails> GetMessages(
            [SwaggerParameter(Description = "Chat ID to take messages from."), FromQuery, Required] Guid chatId,
            [SwaggerParameter(Description = "Offset of portion."), FromQuery, Required, Range(0, int.MaxValue)] int offset,
            [SwaggerParameter(Description = "Portion size."), FromQuery, Required, Range(1, 25)] int portion,
            [SwaggerParameter(Description = "Time to take messages from."), FromQuery] DateTime? timeFrom,
            [SwaggerParameter(Description = "Time to take messages to."), FromQuery] DateTime? timeTo,
            [SwaggerParameter(Description = "String to search by text of messages."), FromQuery, StringLength(32)] string searchString)
        {
            return new MessagesPortionDetails
            {
                Offset = offset,
                Portion = portion,
                TotalCount = 999,
                Content = new List<MessageShortDetails>
                {
                    new MessageShortDetails{Id = Guid.Empty, Text = "m1"},
                    new MessageShortDetails{Id = Guid.Empty, Text = "m2"}
                }
            };
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Reads message description by ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns message description.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        public ActionResult<MessageDetails> GetMessage([SwaggerParameter(Description = "Message ID.")] Guid id)
        {
            return new MessageDetails { Id = id, Text = "some text" };
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Updates message details.", Description = "Message sender can change its content. Message receiver can change the view status.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns message updated details.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid operation.")]
        public ActionResult<MessageDetails> PutMessage(
            [SwaggerParameter(Description = "Message ID.")] Guid id,
            [SwaggerRequestBody(Description = "Message new details."), FromBody, Required] MessageNewDetails message)
        {
            return new MessageDetails { Id = Guid.Empty, Text = "some text" };
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates new message.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns created message description.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid operation.")]
        public ActionResult<MessageDetails> PostMessage([SwaggerRequestBody(Description = "New message details."), FromBody, Required] NewMessageDetails message)
        {
            return new MessageDetails { Text = message.Text, SendingTimeLocal = DateTime.UtcNow };
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes message.", Description = "Deletes message and its attachments from chat.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, Description = "Message is deleted.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        public IActionResult DeleteMessage(
            [SwaggerParameter(Description = "Message ID.")] Guid id,
            [SwaggerParameter(Description = "Must message be deleted for all chat members."), FromQuery, Required] bool isForAll)
        {
            return NoContent();
        }
    }
}
