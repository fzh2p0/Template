using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using WebAPITemplate;

namespace WebAPITemplateTests
{
    internal class ServerFactory : WebApplicationFactory<Program>
    {
        public
            ServerFactory(Uri applicationRootRoute)
        {
            ClientOptions.BaseAddress = applicationRootRoute;
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Isolated");
        }
    }
}