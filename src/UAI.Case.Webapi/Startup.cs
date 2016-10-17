using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using UAI.Case.EFProvider;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;

using Microsoft.AspNetCore.Http;
using StructureMap;
using UAI.Case.Boot;
using UAI.Case.Security;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using System.Linq;


namespace UAI.Case.Webapi
{
    public class Startup
    {
       
        
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            


            if (env.IsEnvironment("Development")) {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
               // builder.AddApplicationInsightsSettings(developerMode: true);
            }



            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

        }

        public IConfigurationRoot Configuration { get; set; }
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

          
            //var connection = Configuration["Data:DefaultConnection:ConnectionStringSQL"];
            //services.AddDbContext<UaiCaseContext>(options => options.UseSqlServer(connection));




            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser().Build());
            });

            services.AddMvc().AddJsonOptions(opt => {
                opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                opt.SerializerSettings.Converters.Add(new StringEnumConverter());
                
                var resolver = opt.SerializerSettings.ContractResolver;

                
                if (resolver != null) {
                    var res = resolver as DefaultContractResolver;
                    res.NamingStrategy = null;  // <<!-- this removes the camelcasing
                }
            });
            services.AddSignalR(options => {
                options.Hubs.EnableDetailedErrors = true;
                options.EnableJSONP = true;
                
                
            });

            services.Configure<MvcOptions>(options => {
                var s = new JsonSerializerSettings();
            });



            var cs= Configuration["Data:DefaultConnection:ConnectionStringSQL"];
            




            var container = Booter.Run(cs);
            container.Populate(services);
                      

            return container.GetInstance<IServiceProvider>();


     


        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app,
         IHostingEnvironment env,
         ILoggerFactory loggerFactory,
         IServiceProvider serviceProvider)
        {


            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //app.UseNHSessionMiddleware();
            app.UseDefaultFiles();
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Use(async (context, next) =>
                {
                    var error = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;
                    // This should be much more intelligent - at the moment only expired 
                    // security tokens are caught - might be worth checking other possible 
                    // exceptions such as an invalid signature.
                    if (error != null && (error.Error is SecurityTokenExpiredException || error.Error is SecurityTokenInvalidSignatureException))
                    {
                        context.Response.StatusCode = 401;
                        // What you choose to return here is up to you, in this case a simple 
                        // bit of JSON to say you're no longer authenticated.
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(
                            JsonConvert.SerializeObject(
                                new { authenticated = false, tokenExpired = true }));
                    }
                    else if (error != null && error.Error != null)
                    {
                        context.Response.StatusCode = 500;
                        
                        if (error.Error.Source== "Microsoft.AspNet.Authentication.JwtBearer")
                            context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        // TODO: Shouldn't pass the exception message straight out, change this.
                        await context.Response.WriteAsync(
                            JsonConvert.SerializeObject
                            (new { success = false, error = error.Error.Message }));
                    }
                    // We're not trying to handle anything else so just let the default 
                    // handler handle.
                    else await next();
                });
            });




            var tokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                // Basic settings - signing key to validate with, audience and issuer.
                IssuerSigningKey = Keys.RSAKey, //TODO: hacer la key constante, sino cada vez q inicia la app genera una nueva y el token anterior no funca

                ValidAudience = TokenHandler.JWT_TOKEN_AUDIENCE,
                ValidIssuer = TokenHandler.JWT_TOKEN_ISSUER,

                // When receiving a token, check that we've signed it.
                //ValidateSignature = true,

                // When receiving a token, check that it is still valid.
                ValidateLifetime = true,

                // This defines the maximum allowable clock skew - i.e. provides a tolerance on the token expiry time 
                // when validating the lifetime. As we're creating the tokens locally and validating them on the same 
                // machines which should have synchronised time, this can be set to zero. Where external tokens are
                // used, some leeway here could be useful.
                ClockSkew = TimeSpan.FromMinutes(0) //TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });

            app.UseStaticFiles();
            app.UseWebSockets();

            app.Map("/signalr", map =>
            {
                map.UseCors(opt =>
            opt.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );
                map.RunSignalR();
            });

            app.UseCors(opt =>
            opt.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            );

            app.UseSignalR();
            // Add MVC to the request pipeline.
            app.UseMvc();
             

            
            
        }

    




    }



}