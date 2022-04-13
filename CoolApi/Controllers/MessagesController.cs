using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoolApi.Database;
using CoolApi.Database.Models;
using CoolApiModels.Messages;

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

        // GET: api/Messages
        [HttpGet]
        public ActionResult<GetMessagesModel> GetMessages(
            [FromQuery] Guid chatId,
            int offset,
            int portion,
            DateTime? timeFrom,
            DateTime? timeTo,
            string searchString)
        {
            // пользователь должен быть в чате

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

            //return await _context.Messages.ToListAsync();
        }

        // GET: api/Messages/5
        [HttpGet("{id}")]
        public ActionResult<GetMessageModel> GetMessage(Guid id)
        {
            // найти пользователя и сравнить сообщение => оно должно быть или его, или из с чата с его участием
            /*var message = await _context.Messages.FindAsync(id);

            if (message == null)
            {
                return NotFound();
            }

            return message;*/

            return new GetMessageModel { Id = id, Text = "some text" };
        }

        // PUT: api/Messages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public ActionResult<GetMessageModel> PutMessage(Guid id, PutMessageModel message)
        {
            // найти пользователя и сравнить сообщение => оно должно быть или его (может изменять текст и вложения), или из с чата с его участием (тогда изменяемо только просмотр)

            /*if (id != message.Id)
            {
                return BadRequest();
            }

            _context.Entry(message).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MessageExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }*/

            return new GetMessageModel { Id = Guid.Empty, Text = "some text" };
        }

        // POST: api/Messages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<GetMessageModel> PostMessage(PostMessageModel message)
        {
            /*var messageDbModel = new Message
            {
                Text = message.Text
            };
            var addedMessage = _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMessage", new { id = message.Id }, message);*/

            return new GetMessageModel { Text = message.Text, SendingDateUtc = DateTime.UtcNow };
        }

        // DELETE: api/Messages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMessage(Guid id, [FromQuery] bool isForAll)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message == null)
            {
                return NotFound();
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MessageExists(Guid id)
        {
            return _context.Messages.Any(e => e.Id == id);
        }
    }
}
