using System.Text;using Microsoft.AspNetCore.Authentication.JwtBearer;using Microsoft.EntityFrameworkCore;using Microsoft.IdentityModel.Tokens;using StayFlow.Api.Data;using StayFlow.Api.Middleware;using StayFlow.Api.Services;
var b=WebApplication.CreateBuilder(args);
b.Services.AddDbContext<StayFlowDbContext>(o=>o.UseSqlServer(b.Configuration.GetConnectionString("Default")));
b.Services.AddScoped<AuthService>();b.Services.AddScoped<BookingService>();b.Services.AddScoped<OperationsService>();b.Services.AddControllers();b.Services.AddCors(o=>o.AddDefaultPolicy(p=>p.WithOrigins(b.Configuration["Cors:Origin"]??"http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));
b.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o=>o.TokenValidationParameters=new(){ValidateIssuer=true,ValidateAudience=true,ValidIssuer=b.Configuration["Jwt:Issuer"],ValidAudience=b.Configuration["Jwt:Issuer"],IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(b.Configuration["Jwt:Key"]!))});
b.Services.AddAuthorization();
var app=b.Build();app.UseMiddleware<ErrorHandlingMiddleware>();app.UseCors();app.UseAuthentication();app.UseAuthorization();app.MapControllers();app.Run();
