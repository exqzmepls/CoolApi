using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace CoolApi.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static Guid GetCurrentUserId(this ControllerBase controller)
        {
            var idValue = controller.User.Claims.Single(c => c.Type == "id").Value;

            return Guid.Parse(idValue);
        }
    }
}
