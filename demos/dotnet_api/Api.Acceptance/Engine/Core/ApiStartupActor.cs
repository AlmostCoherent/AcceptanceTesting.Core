using NorthStandard.Testing.ScreenPlayFramework.Domain.Abstractions;

namespace AcceptanceTesting.Api.Acceptance.Engine.Core
{
    public class ApiStartupActor : IActor
    {
        private readonly ApiContextProvider apiContext;

        public ApiStartupActor(ApiContextProvider apiContext)
        {
            this.apiContext = apiContext;
        }
        public async Task Start() {
            await apiContext.Initialise();
            await apiContext.GetContextAsync();
        }
    }
}
