using Microsoft.Extensions.DependencyInjection;
using Shiny;

namespace ShinyBleApp
{
    public class ShinyAppStartup : Shiny.ShinyStartup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.UseBleCentral();
        }
    }
}
