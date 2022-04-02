using E2E.Core.BL.Implementation;
using E2E.Core.BL.Interface;
using E2E.Core.Data.DataContext;
using E2E.Core.Data.Repository.Implementation;
using E2E.Core.Data.Repository.Interface;
using E2E.Core.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2E.Core.Api
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
      services.AddScoped<IEntityTypeCatalog, EntityTypeCatalog>();
      services.AddScoped<IEntityTypeRepository, EntityTypeRepository>();

      services.AddControllers();

      #region SWAGGER 
      var contact = new OpenApiContact()
      {
        Name = "Vijay Manral",
        Email = "Vijay@Code.com",
        Url = new Uri("http://www.example.com")
      };

      var license = new OpenApiLicense()
      {
        Name = "My License",
        Url = new Uri("http://www.example.com")
      };

      var info = new OpenApiInfo()
      {
        Version = "v1",
        Title = "Swagger Demo API",
        Description = "Swagger Demo API Description",
        TermsOfService = new Uri("http://www.example.com"),
        Contact = contact,
        License = license
      };

      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", info);

      });
      #endregion

      #region CORS
      services.AddCors(options =>
      {
        options.AddDefaultPolicy(
            builder =>
            {
              builder.WithOrigins("*", "*")
                              .AllowAnyHeader()
                              .AllowAnyMethod();
            });
      });
      #endregion

      services.AddDbContext<E2EDbContext>(c =>
        c.UseSqlServer(Configuration.GetConnectionString("E2EDBConnectionString")));

      //ASPNET IDENTITY 
      #region IdentityServer
      services.AddDbContext<E2EIdentityDbContext>(c =>
        c.UseSqlServer(Configuration.GetConnectionString("E2EDBConnectionString")));

      services.AddIdentity<ApplicationUser, IdentityRole>()
        .AddEntityFrameworkStores<E2EIdentityDbContext>()
        .AddDefaultTokenProviders();
      ///After adding above both line
      ///Run ------"Add-Migration Identity -context E2EIdentityDbContext"
      ///And ------"Update-Database -context E2EIdentityDbContext"
      #endregion

      #region Authentication
      // Adding Authentication
      // Adding Jwt Bearer 
      // services.AddAuthentication(options =>
      // {
      //   options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      //   options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      //   options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
      // }).AddJwtBearer(options =>
      //{
      //  options.SaveToken = true;
      //  options.RequireHttpsMetadata = false;
      //  options.TokenValidationParameters = new TokenValidationParameters()
      //  {
      //    ValidateIssuer = true,
      //    ValidateAudience = true,
      //    ValidAudience = Configuration["JWT:ValidAudience"],
      //    ValidIssuer = Configuration["JWT:ValidIssuer"],
      //    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Secret"]))
      //  };
      //});
      #endregion

      #region Authentication_2
      var audienceConfig = Configuration.GetSection("Audience");

      var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(audienceConfig["Secret"]));
      var tokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ValidateIssuer = true,
        ValidIssuer = audienceConfig["Iss"],
        ValidateAudience = true,
        ValidAudience = audienceConfig["Aud"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RequireExpirationTime = true,
      };

      services.AddAuthentication(o =>
      {
        o.DefaultAuthenticateScheme = "iReadyAuthenticationProviderKey";
      })
      // TestKey Should be the same on OCELOT.JSON File
      .AddJwtBearer("iReadyAuthenticationProviderKey", x =>
      {
        x.RequireHttpsMetadata = false;
        x.TokenValidationParameters = tokenValidationParameters;
      });
      #endregion      
      services.Configure<Audience>(Configuration.GetSection("Audience"));

      services.AddAutoMapper(typeof(Startup));


    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      #region Swagger
      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json",
        "Swagger Demo API v1");
      });
      #endregion

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      // Comment for Microservices
      //app.UseHttpsRedirection();

      app.UseCors();

      app.UseRouting();

      //CODE RELATED TO ASPNET IDENTITY 
      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
