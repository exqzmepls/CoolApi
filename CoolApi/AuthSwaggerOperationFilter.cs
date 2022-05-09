using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoolApi
{
    public class AuthSwaggerOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var classAuthAttributes = context.MethodInfo.DeclaringType
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Distinct();
            if (classAuthAttributes.Any())
            {
                SetSecurityRequirement(operation);
                return;
            }

            var methodAuthAttributes = context.MethodInfo
                .GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Distinct();
            if (methodAuthAttributes.Any())
            {
                SetSecurityRequirement(operation);
                return;
            }
        }

        private static void SetSecurityRequirement(OpenApiOperation operation)
        {
            operation.Responses.TryAdd($"{StatusCodes.Status401Unauthorized}", new OpenApiResponse { Description = "Unauthorized" });

            var jwtbearerScheme = new OpenApiSecurityScheme
            {
                Name = JwtBearerDefaults.AuthenticationScheme,
                In = ParameterLocation.Header,
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            };

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [ jwtbearerScheme ] = Array.Empty<string>()
                }
            };
        }
    }
}
