using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Test
{
    class E1 : Celin.AIS.Server
    {
        public E1(IConfiguration config, ILogger logger)
            : base(config["e1Url"], logger)
        {
            AuthRequest.username = config["e1Username"];
            AuthRequest.password = config["e1Password"];
        }
    }
}
