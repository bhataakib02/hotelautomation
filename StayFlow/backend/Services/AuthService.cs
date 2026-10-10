using System.IdentityModel.Tokens.Jwt;using System.Security.Claims;using System.Text;using Microsoft.EntityFrameworkCore;using Microsoft.IdentityModel.Tokens;using StayFlow.Api.Data;
namespace StayFlow.Api.Services;
public record LoginDto(string Email,string Password);
public class AuthService(StayFlowDbContext db,IConfiguration cfg){
 public async Task<string?> LoginAsync(LoginDto d){
  var email=d.Email.Trim().ToLower();var u=await db.Users.FirstOrDefaultAsync(x=>x.Email==email);
  if(u==null||!BCrypt.Net.BCrypt.Verify(d.Password,u.PasswordHash))return null;
  var key=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]!));
  var t=new JwtSecurityToken(cfg["Jwt:Issuer"],cfg["Jwt:Issuer"],new[]{new Claim(ClaimTypes.NameIdentifier,u.Id.ToString()),new Claim(ClaimTypes.Role,u.Role),new Claim("hotel",u.HotelId.ToString())},expires:DateTime.UtcNow.AddHours(8),signingCredentials:new SigningCredentials(key,SecurityAlgorithms.HmacSha256));
  return new JwtSecurityTokenHandler().WriteToken(t);}}
