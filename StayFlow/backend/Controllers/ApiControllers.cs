using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using StayFlow.Api.Data;using StayFlow.Api.Services;
namespace StayFlow.Api.Controllers;
[ApiController,Route("api/auth")]public class AuthController(AuthService a):ControllerBase{
 [HttpPost("login")]public async Task<IActionResult> Login(LoginDto d){var t=await a.LoginAsync(d);return t==null?Unauthorized(new{message="Email or password is incorrect."}):Ok(new{token=t});}}
[ApiController,Route("api/rooms"),Authorize]public class RoomsController(StayFlowDbContext db,BookingService s):ControllerBase{
 int Hotel=>int.Parse(User.FindFirstValue("hotel")!);
 [HttpGet]public async Task<IActionResult> All()=>Ok(await db.Rooms.Include(r=>r.RoomType).Where(r=>r.HotelId==Hotel).ToListAsync());
 [HttpGet("available")]public async Task<IActionResult> Available(DateOnly from,DateOnly to)=>Ok(await s.AvailableAsync(Hotel,from,to));}
[ApiController,Route("api/bookings"),Authorize(Roles="Manager,Receptionist")]public class BookingsController(BookingService s,StayFlowDbContext db):ControllerBase{
 int Hotel=>int.Parse(User.FindFirstValue("hotel")!);
 [HttpGet]public async Task<IActionResult> All()=>Ok(await db.Bookings.Where(b=>b.HotelId==Hotel).OrderByDescending(b=>b.Id).Take(200).ToListAsync());
 [HttpPost]public async Task<IActionResult> Create(CreateBookingDto d)=>Ok(await s.CreateAsync(Hotel,int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),d));}
