using Microsoft.EntityFrameworkCore;using StayFlow.Api.Data;using StayFlow.Api.Models;
namespace StayFlow.Api.Services;
public record OrderLineDto(int MenuItemId,int Qty);public record CreateOrderDto(int? RoomId,int? TableNo,List<OrderLineDto> Lines);public record TicketDto(int RoomId,string Issue,string Category);
public class OperationsService(StayFlowDbContext db){
 async Task<Booking> Get(int hotel,int id)=>await db.Bookings.FirstOrDefaultAsync(b=>b.Id==id&&b.HotelId==hotel)??throw new KeyNotFoundException("Booking not found.");
 void Audit(int u,string a,string e,string? p=null,string? n=null)=>db.AuditLogs.Add(new AuditLog{UserId=u,Action=a,Entity=e,Previous=p,New=n});
 public async Task CheckInAsync(int hotel,int user,int id){var b=await Get(hotel,id);if(b.Status!=BookingStatus.Confirmed)throw new InvalidOperationException("Only confirmed bookings can be checked in.");
  var r=(await db.Rooms.FindAsync(b.RoomId))!;b.Status=BookingStatus.CheckedIn;r.Status=RoomStatus.Occupied;db.Notifications.Add(new Notification{HotelId=hotel,Title="Guest checked in",Body="Room "+r.Number,Department="Reception"});Audit(user,"Check-in","Booking "+b.Id);await db.SaveChangesAsync();}
 public async Task CheckOutAsync(int hotel,int user,int id){var b=await Get(hotel,id);if(b.Status!=BookingStatus.CheckedIn)throw new InvalidOperationException("Only checked-in guests can be checked out.");
  var paid=await db.Payments.Where(p=>p.BookingId==id).SumAsync(p=>p.Amount);if(paid<b.Total)throw new InvalidOperationException("Payment could not be completed: balance is outstanding.");
  var r=(await db.Rooms.FindAsync(b.RoomId))!;b.Status=BookingStatus.CheckedOut;r.Status=RoomStatus.Cleaning;
  db.HousekeepingTasks.Add(new HousekeepingTask{RoomId=r.Id,Priority="High"});db.Invoices.Add(new Invoice{BookingId=id,Number="INV-"+DateTime.UtcNow.Ticks%1000000,Total=b.Total});
  db.Notifications.Add(new Notification{HotelId=hotel,Title="Feedback request queued",Body="Booking "+id,Department="Reception"});
  var a=await db.LoyaltyAccounts.FirstOrDefaultAsync(x=>x.GuestId==b.GuestId);if(a==null){a=new LoyaltyAccount{GuestId=b.GuestId};db.LoyaltyAccounts.Add(a);}
  var pts=(int)(b.Total/100*10);a.Points+=pts;a.Level=a.Points>=6000?"Platinum":a.Points>=3000?"Gold":a.Points>=1500?"Silver":"Bronze";
  Audit(user,"Check-out","Booking "+id,"CheckedIn","CheckedOut");await db.SaveChangesAsync();}
 public async Task CompleteCleaningAsync(int hotel,int user,int taskId){var t=await db.HousekeepingTasks.FindAsync(taskId)??throw new KeyNotFoundException("Task not found.");var r=(await db.Rooms.FirstOrDefaultAsync(x=>x.Id==t.RoomId&&x.HotelId==hotel))??throw new KeyNotFoundException("Room not found.");
  t.Status="Done";r.Status=RoomStatus.Available;db.Notifications.Add(new Notification{HotelId=hotel,Title="Room ready",Body="Room "+r.Number+" is available",Department="Reception"});Audit(user,"Room status changed","Room "+r.Number,"Cleaning","Available");await db.SaveChangesAsync();}
 public async Task<MaintenanceTicket> CreateTicketAsync(int hotel,int user,TicketDto d){if(d.Issue.Trim().Length<3)throw new ArgumentException("Describe the issue.");var r=await db.Rooms.FirstOrDefaultAsync(x=>x.Id==d.RoomId&&x.HotelId==hotel)??throw new KeyNotFoundException("Room not found.");
  var t=new MaintenanceTicket{RoomId=r.Id,Issue=d.Issue.Trim(),Category=d.Category,Status="Assigned"};db.MaintenanceTickets.Add(t);if(r.Status==RoomStatus.Available)r.Status=RoomStatus.Maintenance;Audit(user,"Ticket created","Room "+r.Number);await db.SaveChangesAsync();return t;}
 public async Task ResolveTicketAsync(int hotel,int user,int id,decimal cost){var t=await db.MaintenanceTickets.FindAsync(id)??throw new KeyNotFoundException("Ticket not found.");var r=(await db.Rooms.FirstOrDefaultAsync(x=>x.Id==t.RoomId&&x.HotelId==hotel))??throw new KeyNotFoundException("Room not found.");
  t.Status="Resolved";t.Cost=cost;if(r.Status==RoomStatus.Maintenance)r.Status=RoomStatus.Available;Audit(user,"Ticket resolved","Room "+r.Number);await db.SaveChangesAsync();}
 public async Task<RestaurantOrder> CreateOrderAsync(int hotel,int user,CreateOrderDto d){if(d.Lines.Count==0||d.Lines.Any(l=>l.Qty<1))throw new ArgumentException("Add at least one item.");
  var ids=d.Lines.Select(l=>l.MenuItemId).ToList();var menu=await db.MenuItems.Where(m=>m.HotelId==hotel&&ids.Contains(m.Id)).ToDictionaryAsync(m=>m.Id);
  if(d.Lines.Any(l=>!menu.TryGetValue(l.MenuItemId,out var m)||!m.Available))throw new InvalidOperationException("An item is unavailable.");
  var o=new RestaurantOrder{HotelId=hotel,RoomId=d.RoomId,TableNo=d.TableNo,Items=d.Lines.Select(l=>new OrderItem{MenuItemId=l.MenuItemId,Qty=l.Qty,UnitPrice=menu[l.MenuItemId].Price}).ToList()};o.Total=o.Items.Sum(i=>i.Qty*i.UnitPrice);
  db.RestaurantOrders.Add(o);db.Notifications.Add(new Notification{HotelId=hotel,Title="New food order",Body="Total "+o.Total,Department="Kitchen"});Audit(user,"Order placed","Order");await db.SaveChangesAsync();return o;}
 public async Task<object> DashboardAsync(int hotel){var rooms=await db.Rooms.Where(r=>r.HotelId==hotel).ToListAsync();var occ=rooms.Count(r=>r.Status==RoomStatus.Occupied);var today=DateOnly.FromDateTime(DateTime.UtcNow);
  var rev=await db.Payments.Where(p=>p.PaidAt.Date==DateTime.UtcNow.Date).SumAsync(p=>p.Amount);
  return new{totalRooms=rooms.Count,occupied=occ,available=rooms.Count(r=>r.Status==RoomStatus.Available),occupancyPct=rooms.Count==0?0:Math.Round(100.0*occ/rooms.Count,1),revenueToday=rev,
   checkInsToday=await db.Bookings.CountAsync(b=>b.HotelId==hotel&&b.CheckIn==today),checkOutsToday=await db.Bookings.CountAsync(b=>b.HotelId==hotel&&b.CheckOut==today),lowStock=await db.Inventory.CountAsync(i=>i.Stock<i.MinStock)};}}
