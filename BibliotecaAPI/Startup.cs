using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Reflection;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BibliotecaAPI.Services.Contracts;

namespace BibliotecaAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {            
            services.AddScoped(typeof(IAuthService), typeof(AuthService));//Inyectamos servicio que hemos creado AuthService

            services.AddControllers().AddNewtonsoftJson(x =>
                x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("default")));

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xlmPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            services.AddSwaggerGen(config =>
                {
                    config.SwaggerDoc("v1",
                            new Microsoft.OpenApi.Models.OpenApiInfo()
                            {
                                Title = "Documentación del API Biblioteca",
                                Description = "Esta es la documentación oficial de esta api experimental y didáctica"
                            }
                        );
                    config.IncludeXmlComments(xlmPath);
                    //config.ResolveConflictingActions(apidescriptions => apidescriptions.First());
                }
            );

            //***** Configuracion de servicios para JWT *****
            //autorizacion
            services.AddAuthorization(options =>
                options.DefaultPolicy =
                new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build()
            );

            var issuer = Configuration["AuthenticationSettings:Issuer"];
            var audience = Configuration["AuthenticationSettings:Audience"];
            var signinKey = Configuration["AuthenticationSettings:SigningKey"];
            
            //autenticacion
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                {
                    options.Audience = audience;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signinKey))
                    };
                }
            );

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //Configurando la aplicación para JWT
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(config =>
                {
                    config.SwaggerEndpoint("/swagger/v1/swagger.json", "API OFICIAL V1");
                    config.RoutePrefix = "";
                }
            );
        }
    }
}
