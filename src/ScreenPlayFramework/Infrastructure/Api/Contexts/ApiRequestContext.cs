using System.Net;
using Microsoft.AspNetCore.Mvc;
using AlmostCoherent.Testing.ScreenPlayFramework.Domain.Abstractions;

namespace AlmostCoherent.Testing.ScreenPlayFramework.Infrastructure.Api.Contexts
{
    public class ApiRequestContext : IContext
    {
        public StatusCodeResult StatusCodeResult { get; set; } = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
    }
}
