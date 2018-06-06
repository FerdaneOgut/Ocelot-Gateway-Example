using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Gateway.API
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
            builder.SetBasePath(env.ContentRootPath)
                           .AddJsonFile("appsettings.json", true, true)
                           .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                           .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var jwtAppSettingOptions = Configuration.GetSection("JwtIssuerOptions");
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtAppSettingOptions["Secret"]));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions["Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true,
            };

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = "ApiTest";
             
            })
            .AddJwtBearer("ApiTest", x =>
            {
                x.RequireHttpsMetadata = false;
                x.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddOcelot().AddCacheManager(x =>
                       {
                           x.WithDictionaryHandle();
                       });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();
            await app.UseOcelot();
        }
    }
}
