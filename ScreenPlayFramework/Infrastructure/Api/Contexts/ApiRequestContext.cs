using System.Net;
using Microsoft.AspNetCore.Mvc;
using NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions;

namespace NorthStandard.Testing.ScreenPlayFramework.Infrastructure.Api.Contexts
{
    public class ApiRequestContext : IContext
    {
        public StatusCodeResult StatusCodeResult { get; set; } = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
    }
}
