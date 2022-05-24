using System;

namespace CoolApi.Database.Options
{
    /// <summary>
    /// Base class for DB action options with authorization info.
    /// </summary>
    public abstract class AuthorizedActionOptions
    {
        /// <summary>
        /// ID of the user who performs action. 
        /// </summary>
        public Guid ActorId { get; set; }
    }
}
