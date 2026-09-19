using Chatting.Api.Application.Data;
using Chatting.Api.Application.Services;
using Chatting.Api.Domain.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;

namespace Chatting.Api;

public static class BuilderExtensions
{
    public static WebApplicationBuilder BuildAbstractions(this WebApplicationBuilder builder)
    {

        builder.Services.AddControllers();

        builder.Services.AddLogging();
        builder.Services.AddHttpContextAccessor();


        return builder;
    }

    public static WebApplicationBuilder BuildDbContext(this WebApplicationBuilder builder)
    {

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"))


            );
        builder.Services.AddScoped<ApplicationDbContext>();
        builder.Services.AddScoped<ApplicationUnitOfWork>();

        return builder;
    }
    public static WebApplicationBuilder BuildIdentity(this WebApplicationBuilder builder)
    {

        builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(op =>
        {
            //op.SignIn.RequireConfirmedPhoneNumber = true;
            op.SignIn.RequireConfirmedEmail = true;
            op.SignIn.RequireConfirmedAccount = true;


            op.Password.RequireNonAlphanumeric = false;
            op.Password.RequiredLength = 2;
            op.Password.RequireUppercase = false;
            op.Password.RequireLowercase = false;
            op.Password.RequireDigit = false;




            op.User.RequireUniqueEmail = true;





            op.Lockout.AllowedForNewUsers = true;
            op.Lockout.MaxFailedAccessAttempts = 5;
            op.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        ;





        return builder;
    }

    public static WebApplicationBuilder BuildJwt(this WebApplicationBuilder builder)
    {

        JwtOptions jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
        builder.Services.AddSingleton(jwt);

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwt.Issuer,
                ValidAudience = jwt.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwt.SigningKey)),



                ClockSkew = TimeSpan.Zero,
            };

        });


        return builder;
    }



}
