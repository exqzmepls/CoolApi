using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CoolApi.Database;
using CoolApiModels.Messages;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using CoolApi.Database.Repositories;
using CoolApi.Database.Models;
using System.Linq;

namespace CoolApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly IRepository<Message> _repository;

        public MessagesController(CoolContext context)
        {
            _repository = new MessageRepository(context);
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Reads messages portion.", Description = "Reads messages portion according to the query params and sorted by sending time.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns data portion.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid query parameters values.")]
        public ActionResult<MessagesPortionDetails> GetMessages([SwaggerParameter(Description = "Chat ID to take messages from."), FromQuery, Required] Guid chatId,
            [SwaggerParameter(Description = "Offset of portion."), FromQuery, Required, Range(0, int.MaxValue)] int offset,
            [SwaggerParameter(Description = "Portion size."), FromQuery, Required, Range(1, 25)] int portion,
            [SwaggerParameter(Description = "Time to take messages from."), FromQuery] DateTime? timeFrom,
            [SwaggerParameter(Description = "Time to take messages to."), FromQuery] DateTime? timeTo,
            [SwaggerParameter(Description = "String to search by text of messages."), FromQuery, StringLength(32)] string searchString)
        {
            var userId = GetCurrentUserId();
            var result = _repository.ReadPortion(offset, portion, (m) => m.ChatMemberMessages.First().ChatMember.ChatId == chatId && m.SendingTimeUtc >= timeFrom && m.SendingTimeUtc <= timeTo && m.Text.Contains(searchString), userId);

            var messages = result.DataCollection;
            var response = new MessagesPortionDetails
            {
                Offset = offset,
                Portion = messages.Count(),
                TotalCount = result.TotalCount,
                Content = messages.Select(m => new MessageShortDetails
                {
                    Id = m.Id,
                    IsViewed = m.IsViewed,
                    SendingTimeUtc = m.SendingTimeUtc,
                    SenderId = m.ChatMemberMessages.First().ChatMember.UserId,
                    ModificationTimeUtc = m.ModificationTimeUtc,
                    Text = m.Text,
                    AttachmentsCount = m.Attachments.Count()
                })
            };
            return response;
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Reads message description by ID.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns message description.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        public ActionResult<MessageDetails> GetMessage([SwaggerParameter(Description = "Message ID.")] Guid id)
        {
            var currentUserId = GetCurrentUserId();
            var message = _repository.Read(id, currentUserId);

            if (message == null)
                return NotFound();

            var response = GetDto(message);
            return response;
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Updates message details.", Description = "Message sender can change its content. Message receiver can change the view status.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns message updated details.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid operation.")]
        public ActionResult<MessageDetails> PutMessage([SwaggerParameter(Description = "Message ID.")] Guid id,
            [SwaggerRequestBody(Description = "Message new details."), FromBody, Required] MessageNewDetails messageNewDetails)
        {
            var currentUserId = GetCurrentUserId();
            // todo logic

            var updatedMessage = _repository.Read(id, currentUserId);
            if (updatedMessage == null)
                return NotFound();
            var response = GetDto(updatedMessage);
            return response;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Creates new message.")]
        [SwaggerResponse(StatusCodes.Status200OK, Description = "Returns created message description.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, Description = "Invalid operation.")]
        public ActionResult<MessageDetails> PostMessage([SwaggerRequestBody(Description = "New message details."), FromBody, Required] NewMessageDetails newMessageDetails)
        {
            var currentUserId = GetCurrentUserId();
            var newMessage = new Message
            {
                Text = newMessageDetails.Text,
                Attachments = newMessageDetails.Attachments.Select(a => new Attachment { Content = a }),
                ChatMemberMessages = new ChatMemberMessage[]
                {
                    new ChatMemberMessage
                    {
                        ChatMember = new ChatMember{UserId = currentUserId, ChatId = newMessageDetails.ChatId}
                    }
                }
            };
            try
            {
                var newMessageId = _repository.Create(newMessage, currentUserId);
                var createdMessage = _repository.Read(newMessageId, currentUserId);
                if (createdMessage == null)
                    return BadRequest("no chat");

                var response = GetDto(createdMessage);
                return response;
            }
            catch (Exception exception)
            {
                var error = GetProblemDetails(exception);
                return BadRequest(error);
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Deletes message.", Description = "Deletes message and its attachments from chat.")]
        [SwaggerResponse(StatusCodes.Status204NoContent, Description = "Message is deleted.")]
        [SwaggerResponse(StatusCodes.Status404NotFound, Description = "ID does not exist.")]
        public IActionResult DeleteMessage([SwaggerParameter(Description = "Message ID.")] Guid id,
            [SwaggerParameter(Description = "Must message be deleted for all chat members."), FromQuery, Required] bool isForAll)
        {
            var currentUserId = GetCurrentUserId();
            try
            {
                if (isForAll)
                    _repository.Delete(id, currentUserId);
                else
                    _repository.Hide(id, currentUserId);
            }
            catch (Exception exception)
            {
                var error = GetProblemDetails(exception);
                return BadRequest(error);
            }

            return NoContent();
        }

        private static MessageDetails GetDto(Message message)
        {
            var dto = new MessageDetails
            {
                Id = message.Id,
                SenderId = message.ChatMemberMessages.First().ChatMember.UserId,
                SendingTimeUtc = message.SendingTimeUtc,
                IsViewed = message.IsViewed,
                ModificationTimeUtc = message.ModificationTimeUtc,
                Text = message.Text,
                Attachments = message.Attachments.Select(a => a.Content)
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
