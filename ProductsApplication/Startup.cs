using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace ProductsApplication
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
            JwtSettings settings;
            settings = GetJwtSettings();
            services.AddSingleton<JwtSettings>(settings);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
           .AddJwtBearer("JwtBearer", jwtBearerOptions =>
           {
               jwtBearerOptions.TokenValidationParameters =
                   new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(
                           Encoding.UTF8.GetBytes(settings.Key)),
                       ValidateIssuer = true,
                       ValidIssuer = settings.Issuer,

                       ValidateAudience = true,
                       ValidAudience = settings.Audience,

                       ValidateLifetime = true,
                       ClockSkew = TimeSpan.FromMinutes(
                           settings.MinutesToExpiration)
                   };
           });

            services.AddDbContext<ApplicationContext>(opts =>
                opts.UseSqlServer(Configuration.GetConnectionString("DBContext")));

            services.AddIdentity<User, IdentityRole>(opt =>
            {
                opt.Password.RequiredLength = 7;
                opt.Password.RequireDigit = false;
                opt.Password.RequireUppercase = false;

                opt.User.RequireUniqueEmail = true;
            })
             .AddEntityFrameworkStores<ApplicationContext>();

            services.AddAutoMapper(typeof(Startup));
            services.AddCors(options => options.AddPolicy("MyCorsPolicy",
             builder => builder
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyMethod()
                 .SetIsOriginAllowed(_ => true)
                 .AllowCredentials().AllowAnyHeader()
                 .WithExposedHeaders("X-Pagination")
                 .Build()
         ));
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
            services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
               // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("MyCorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(x => x.MapControllers());           
        }
        public JwtSettings GetJwtSettings()
        {
            JwtSettings settings = new JwtSettings();

            settings.Key = Configuration["JwtSettings:key"];
            settings.Audience = Configuration["JwtSettings:audience"];
            settings.Issuer = Configuration["JwtSettings:issuer"];
            settings.MinutesToExpiration =Convert.ToInt32(Configuration["JwtSettings:minutesToExpiration"]);

            return settings;
        }
    }
}
