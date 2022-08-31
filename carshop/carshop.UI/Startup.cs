using carshop.carshop.UI.Mappings;
using carshop.Infrastructure.DataAccess;
using carshop.Infrastructure.Repositories;
using casrshop.Core.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Autofac;
using carshop.Infrastructure.Services;
using casrshop.Core.IServices;
using Microsoft.AspNetCore.Authentication;
using carshop.carshop.UI.AutheticationHandler;

namespace carshop
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
            services.AddAutoMapper(x =>
            {
                x.AddProfile(new MappingCar());
            });
            services.AddAutoMapper(x =>
            {
                x.AddProfile(new MappingOrder());
            });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CarShop" });
                c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
                {
                    Name="Authorization",
                    Type=SecuritySchemeType.Http,
                    Scheme="basic",
                    In=ParameterLocation.Header,
                    Description="Authorization"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference=new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="basic"
                            }

                        },
                        new string[]{}
                    }
                });
            });

            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, AuthenticationHandler>("BasicAuthentication", null);
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<CarShopContext>().AsSelf();
            builder.RegisterType<OrderRepository>().As<IOrderRepository>();
            builder.RegisterType<CarRepository>().As<ICarRepository>();
            builder.RegisterType<PriceCalculator>().As<IPriceCalculator>();
            builder.RegisterType<MessageCreator>().As<IMessageCreator>();
            builder.RegisterType<EmailSender>().As<IEmailSender>();
            builder.RegisterType<AdminRepository>().As<IAdminRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "carshop"));
            }

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern:"{controller=User}"
                    );
            });
        }
    }
}
