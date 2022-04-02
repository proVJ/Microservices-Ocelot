using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway
{
  public class AppConstants
  {
    public const string Authority = "http://localhost:56046"; //44384
    public const string ApiResourceName = "";

  }
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      #region JWT
      // Adding Authentication
      // Adding Jwt Bearer 
      //services.AddAuthentication(options =>
      //{
      //  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      //  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      //  options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
      //}).AddJwtBearer(options =>
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

      var secret = "Thisismytestprivatekey";
      var key = Encoding.ASCII.GetBytes(secret);
      services.AddAuthentication(option =>
      {
        option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      }).AddJwtBearer(options =>
      {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuerSigningKey = true,
          ValidateIssuer = false,
          ValidateAudience = false
        };
      });

      #endregion

      #region Authentication
      //var authenticationProviderKey = "Bearer";
      //Action<JwtBearerOptions> options = o =>
      //{
      //  o.Authority = AppConstants.Authority;
      //  o.RequireHttpsMetadata = false;
      //  // etc
      //};

      //services.AddAuthentication()
      //    .AddJwtBearer(authenticationProviderKey, options);

      #endregion


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
        o.DefaultAuthenticateScheme = "TestKey";
      })
      // TestKey Should be the same on OCELOT.JSON File
      .AddJwtBearer("iReadyAuthenticationProviderKey", x =>
      {
        x.RequireHttpsMetadata = false;
        x.TokenValidationParameters = tokenValidationParameters;
      });

      services.AddOcelot();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();
      app.UseAuthentication();
      //app.UseAuthorization();


      app.UseEndpoints(endpoints =>
      {
        endpoints.MapGet("/", async context =>
        {
          await context.Response.WriteAsync("Microservice Running......");
        });
        endpoints.MapControllers();

      });

      // OCELOT
      await app.UseOcelot();
    }
  }
}
