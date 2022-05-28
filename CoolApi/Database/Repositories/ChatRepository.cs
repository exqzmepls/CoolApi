using CoolApi.Database.Models;
using CoolApi.Database.Options;
using CoolApi.Database.Repositories.Results;
using System;

namespace CoolApi.Database.Repositories
{
    public class ChatRepository : BaseRepository<Chat, ReadPortionChatOptions, ReadChatOptions, CreateChatOptions, UpdateChatOptions, DeleteChatOptions>
    {
        public ChatRepository(CoolContext context) : base(context)
        {
        }

        public override void Create(CreateChatOptions options)
        {
            // todo
        }

        public override void Delete(DeleteChatOptions options)
        {
            // todo
        }

        public override Chat Read(ReadChatOptions options)
        {
            // todo
        }

        public override PortionResult<Chat> ReadPortion(ReadPortionChatOptions options)
        {
            // todo
        }

        public override void Update(UpdateChatOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
