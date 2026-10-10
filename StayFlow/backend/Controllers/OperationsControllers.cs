using System.Security.Claims;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using StayFlow.Api.Data;using StayFlow.Api.Services;
namespace StayFlow.Api.Controllers;
public abstract class HotelController:ControllerBase{protected int Hotel=>int.Parse(User.FindFirstValue("hotel")!);protected int UserId=>int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);}
public record BookingRef(int BookingId);public record ResolveDto(decimal Cost);
[ApiController,Authorize]public class OperationsController(OperationsService s):HotelController{
 [HttpPost("api/checkin"),Authorize(Roles="Manager,Receptionist")]public async Task<IActionResult> CheckIn(BookingRef d){await s.CheckInAsync(Hotel,UserId,d.BookingId);return NoContent();}
 [HttpPost("api/checkout"),Authorize(Roles="Manager,Receptionist")]public async Task<IActionResult> CheckOut(BookingRef d){await s.CheckOutAsync(Hotel,UserId,d.BookingId);return NoContent();}
 [HttpGet("api/dashboard"),Authorize(Roles="SuperAdmin,Manager,Receptionist")]public async Task<IActionResult> Dashboard()=>Ok(await s.DashboardAsync(Hotel));
 [HttpPost("api/restaurant/orders"),Authorize(Roles="Manager,Restaurant,Guest")]public async Task<IActionResult> Order(CreateOrderDto d)=>Ok(await s.CreateOrderAsync(Hotel,UserId,d));
 [HttpPost("api/maintenance"),Authorize(Roles="Manager,Housekeeping,Receptionist,Guest")]public async Task<IActionResult> Ticket(TicketDto d)=>Ok(await s.CreateTicketAsync(Hotel,UserId,d));
 [HttpPut("api/maintenance/{id}/resolve"),Authorize(Roles="Manager,Maintenance")]public async Task<IActionResult> Resolve(int id,ResolveDto d){await s.ResolveTicketAsync(Hotel,UserId,id,d.Cost);return NoContent();}
 [HttpPut("api/housekeeping/tasks/{id}/complete"),Authorize(Roles="Manager,Housekeeping")]public async Task<IActionResult> Clean(int id){await s.CompleteCleaningAsync(Hotel,UserId,id);return NoContent();}}
[ApiController,Authorize]public class ReadControllers(StayFlowDbContext db):HotelController{
 [HttpGet("api/housekeeping/tasks"),Authorize(Roles="Manager,Housekeeping")]public async Task<IActionResult> Tasks()=>Ok(await db.HousekeepingTasks.Where(t=>t.Status!="Done").ToListAsync());
 [HttpGet("api/maintenance"),Authorize(Roles="Manager,Maintenance")]public async Task<IActionResult> Tickets()=>Ok(await db.MaintenanceTickets.OrderByDescending(t=>t.Id).Take(200).ToListAsync());
 [HttpGet("api/inventory"),Authorize(Roles="Manager,Restaurant")]public async Task<IActionResult> Inventory()=>Ok(await db.Inventory.Select(i=>new{i.Id,i.Name,i.Category,i.Stock,i.MinStock,i.MaxStock,lowStock=i.Stock<i.MinStock}).ToListAsync());
 [HttpGet("api/notifications")]public async Task<IActionResult> Notes()=>Ok(await db.Notifications.Where(n=>n.HotelId==Hotel).OrderByDescending(n=>n.At).Take(100).ToListAsync());
 [HttpPut("api/notifications/{id}/read")]public async Task<IActionResult> Read(int id){var n=await db.Notifications.FirstOrDefaultAsync(x=>x.Id==id&&x.HotelId==Hotel);if(n==null)return NotFound();n.IsRead=true;await db.SaveChangesAsync();return NoContent();}}
