using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoolApi.Database;
using CoolApiModels.Messages;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace CoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly CoolContext _context;

        public MessagesController(CoolContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Read messages portion sorted by sending time.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns data portion.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid query parameters values.")]
        public ActionResult<GetMessagesModel> GetMessages(
            [SwaggerParameter(Description = "Chat ID to take messages from."), FromQuery, Required] Guid chatId,
            [SwaggerParameter(Description = "Offset of portion."), FromQuery, Required, Range(0, int.MaxValue)] int offset,
            [SwaggerParameter(Description = "Portion size."), FromQuery, Required, Range(1, 25)] int portion,
            [SwaggerParameter(Description = "Time to take messages from."), FromQuery] DateTime? timeFrom,
            [SwaggerParameter(Description = "Time to take messages to."), FromQuery] DateTime? timeTo,
            [SwaggerParameter(Description = "String to search by text of messages."), FromQuery, StringLength(32)] string searchString)
        {
            return new GetMessagesModel
            {
                Offset = offset,
                Portion = portion,
                TotalCount = 999,
                Content = new List<ShortGetMessageModel>
                {
                    new ShortGetMessageModel{Id = Guid.Empty, Text = "m1"},
                    new ShortGetMessageModel{Id = Guid.Empty, Text = "m2"}
                }
            };
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Read message description by ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns message description.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        public ActionResult<GetMessageModel> GetMessage([SwaggerParameter(Description = "Message ID.")] Guid id)
        {
            return new GetMessageModel { Id = id, Text = "some text" };
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update message info.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns message updated description.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid operation.")]
        public ActionResult<GetMessageModel> PutMessage(
            [SwaggerParameter(Description = "Message ID.")] Guid id,
            [SwaggerRequestBody(Description = "Message new description."), FromBody, Required] PutMessageModel message)
        {
            return new GetMessageModel { Id = Guid.Empty, Text = "some text" };
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create new message.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns created message description.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid operation.")]
        public ActionResult<GetMessageModel> PostMessage([SwaggerRequestBody(Description = "New message info."), FromBody, Required] PostMessageModel message)
        {
            return new GetMessageModel { Text = message.Text, SendingTimeUtc = DateTime.UtcNow };
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete message.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, Description = "Message was deleted.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        public IActionResult DeleteMessage(
            [SwaggerParameter(Description = "Message ID.")] Guid id,
            [SwaggerParameter(Description = "Must message be deleted for all chat members."), FromQuery, Required] bool isForAll)
        {
            return NoContent();
        }
    }
}
