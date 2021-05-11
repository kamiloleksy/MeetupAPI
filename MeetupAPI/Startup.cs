using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using MeetupAPI.Authorization;
using MeetupAPI.Entities;
using MeetupAPI.Filters;
using MeetupAPI.Identity;
using MeetupAPI.Models;
using MeetupAPI.Services;
using MeetupAPI.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace MeetupAPI
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
            var jwtOptions = new JwtOptions();
            Configuration.GetSection("jwt").Bind(jwtOptions);
            //pobranie sekcji z appsettings.json i po��czenie(bindowanie) ich JwtOptions

            services.AddSingleton(jwtOptions);

            //autentykacja = uwierztelnianie
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = jwtOptions.JwtIssuer,//wydawca token�w
                    ValidAudience = jwtOptions.JwtIssuer, //odbiorcy token�w
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.JwtKey))
                };
            });
            //w�asna polityka autoryzacji
            services.AddAuthorization(options =>
            {
                //polityka narodowo�� u�ytkownika
                options.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality"));

                //w�asna polityka dot. wieku
                options.AddPolicy("AtLeast18", builder => builder.AddRequirements(new MinimumAgeRecuirement(18)));
            });
            services.AddScoped<TimeTrackFilter>();
            services.AddScoped<IMath,MathService>();
            services.AddScoped<IAuthorizationHandler, MeetupResourceOperationHandler>();
            services.AddScoped<IAuthorizationHandler, MinimumAgeHandler>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddControllers(options => options.Filters.Add(typeof(ExceptionFilter))).AddFluentValidation(); //w �rodku AddControllers na�o�ony globalny filter
            services.AddScoped<IValidator<RegisterUserDto>, RegisterUserValidator>();
            services.AddDbContext<MeetupContext>();
            services.AddScoped<MeetupSeeder>();
            services.AddAutoMapper(this.GetType().Assembly);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo() { Title = "MeetupAPI", Version = "v1" });
            });

            services.AddCors(option =>
            {
                option.AddPolicy("FrontendClient", builder => builder.AllowAnyHeader()
                .AllowAnyMethod().WithOrigins("http://localhost:3000"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MeetupSeeder meetupSeeder)
        {
            app.UseResponseCaching();
            app.UseStaticFiles();
            app.UseCors("FrontendClient");
            app.UseSwagger();
            app.UseSwaggerUI(c=>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MeetupAPI v1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization(); //autoryzacja, wa�ne �eby by�a po routning

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //meetupSeeder.Seed();
        }
    }
}
