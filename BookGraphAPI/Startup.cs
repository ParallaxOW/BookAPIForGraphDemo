using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(BookGraphAPI.Startup))]
namespace BookGraphAPI
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton(s =>
            {
                return new DocumentClient(new Uri(Environment.GetEnvironmentVariable("dbEndpoint")), Environment.GetEnvironmentVariable("dbAuthKey"));
            });
        }
    }
}
