using Microsoft.EntityFrameworkCore;using StayFlow.Api.Data;using StayFlow.Api.Models;
namespace StayFlow.Api.Services;
public record CreateBookingDto(int GuestId,int RoomId,DateOnly CheckIn,DateOnly CheckOut);
public class BookingService(StayFlowDbContext db){
 public Task<List<Room>> AvailableAsync(int hotelId,DateOnly from,DateOnly to)=>db.Rooms.Include(r=>r.RoomType).Where(r=>r.HotelId==hotelId&&r.Status!=RoomStatus.OutOfService&&!db.Bookings.Any(b=>b.RoomId==r.Id&&b.Status!=BookingStatus.Cancelled&&b.CheckIn<to&&b.CheckOut>from)).ToListAsync();
 public async Task<Booking> CreateAsync(int hotelId,int userId,CreateBookingDto d){
  if(d.CheckOut<=d.CheckIn)throw new ArgumentException("Check-out must be after check-in.");
  var room=await db.Rooms.Include(r=>r.RoomType).FirstOrDefaultAsync(r=>r.Id==d.RoomId&&r.HotelId==hotelId)??throw new KeyNotFoundException("Room not found.");
  if((await AvailableAsync(hotelId,d.CheckIn,d.CheckOut)).All(r=>r.Id!=room.Id))throw new InvalidOperationException("Unable to create booking. Please check room availability.");
  var nights=d.CheckOut.DayNumber-d.CheckIn.DayNumber;var b=new Booking{HotelId=hotelId,GuestId=d.GuestId,RoomId=room.Id,CheckIn=d.CheckIn,CheckOut=d.CheckOut,Status=BookingStatus.Confirmed,Total=nights*room.RoomType!.BaseRate};
  db.Bookings.Add(b);db.AuditLogs.Add(new AuditLog{UserId=userId,Action="Booking created",Entity="Room "+room.Number});await db.SaveChangesAsync();return b;}}
