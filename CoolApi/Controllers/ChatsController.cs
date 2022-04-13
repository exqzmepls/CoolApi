using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoolApi.Database;
using CoolApi.Database.Models;
using CoolApiModels.Chats;

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

        // GET: api/Chats
        [HttpGet]
        public ActionResult<GetChatsModel> GetChats([FromQuery] int offset = 0, [FromQuery] int portion = 10)
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
            //return await _context.Chats.ToListAsync();
        }

        // GET: api/Chats/5
        [HttpGet("{id}")]
        public ActionResult<GetChatModel> GetChat(Guid id)
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

            /*var chat = await _context.Chats.FindAsync(id);

            if (chat == null)
            {
                return NotFound();
            }

            return chat;*/
        }

        // POST: api/Chats
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<GetChatModel> PostChat([FromBody] PostChatModel chat)
        {
            return new GetChatModel { Id = chat.ReceiverId, CreationTimeUtc = DateTime.UtcNow };

            /*_context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetChat", new { id = chat.Id }, chat);*/
        }

        // DELETE: api/Chats/5
        [HttpDelete("{id}")]
        public IActionResult DeleteChat(Guid id, [FromQuery] bool isForAll)
        {
            /*var chat = await _context.Chats.FindAsync(id);
            if (chat == null)
            {
                return NotFound();
            }

            _context.Chats.Remove(chat);
            await _context.SaveChangesAsync();*/

            return NoContent();
        }

        private bool ChatExists(Guid id)
        {
            return _context.Chats.Any(e => e.Id == id);
        }
    }
}
