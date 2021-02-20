using System.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Versioning;
using Hotels.API.Controllers;
using Microsoft.AspNetCore.Mvc.Versioning.Conventions;
using Hotels.API.Filters;
using Hotels.API.Models;
using Hotels.API.Models.Derived;
using Hotels.API.Contexts;
using Microsoft.EntityFrameworkCore;
using Hotels.API.Services;
using Hotels.API.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Hotels.API.Workers;
using AspNetCore.IServiceCollection.AddIUrlHelper;

namespace Hotels.API
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

            services.Configure<HotelInfo>(
                Configuration.GetSection("HotelInfo")
            );

            services.AddDbContext<HotelApiDbContext>(options => 
            {
                options.UseInMemoryDatabase("HotelDB");
            });

            services.AddControllers(options => 
            {
                options.Filters.Add(typeof(JsonExceptionFilters));
                options.Filters.Add<AllowOnlyRequireHttps>();
            });

            //services.AddAutoMapper(typeof(MappingProfile));
            services.AddAutoMapper(option => option.AddProfile<MappingProfile>());
            services.AddScoped<IRoomService,RoomService>();
            services.AddScoped<IUserService, UserService>();
            //services.AddSession()
            services.AddUrlHelper();
            services.AddMemoryCache();

            string key  = Configuration.GetValue<string>("JwtTokenKey");
            byte[] keyValue = Encoding.UTF8.GetBytes(key);

            services.AddAuthentication(auth => 
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwt => {
                // development sürecinde false olarak set edilebilir.
                jwt.RequireHttpsMetadata = true;
               // jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(keyValue),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    //ClockSkew = 
                };
            });

            services.AddHostedService<RoomWorkerService>();

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddSwaggerDocument();
           
            services.AddApiVersioning(options => 
            {
                options.DefaultApiVersion = new ApiVersion(1,0);
                options.ApiVersionReader = new MediaTypeApiVersionReader();

                /*
                    options.ApiVersionReader = new HeaderApiVersionReader("api-version");
                    options.ApiVersionReader = new QueryStringApiVersionReader("v");
                */

                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionSelector  = new CurrentImplementationApiVersionSelector(options);
                

                options.Conventions.Controller<TestController>()
                        .HasDeprecatedApiVersion(1,0)
                        .HasApiVersion(1,1)
                        .HasApiVersion(2,0)
                        .Action(a => a.GetCustomers()).MapToApiVersion(1,1)
                        .Action(a => a.GetCustomerV2()).MapToApiVersion(2,0);

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwaggerUi3();
                app.UseOpenApi();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
