namespace StayFlow.Api.Models;
public enum RoomStatus{Available,Occupied,Cleaning,Maintenance,OutOfService,Reserved}
public enum BookingStatus{Pending,Confirmed,CheckedIn,CheckedOut,Cancelled,NoShow}
public class Hotel{public int Id{get;set;}public string Name{get;set;}="";public string City{get;set;}="";}
public class RoomType{public int Id{get;set;}public string Name{get;set;}="";public decimal BaseRate{get;set;}}
public class Room{public int Id{get;set;}public int HotelId{get;set;}public int RoomTypeId{get;set;}public string Number{get;set;}="";public int Floor{get;set;}public RoomStatus Status{get;set;}public RoomType? RoomType{get;set;}}
public class Guest{public int Id{get;set;}public string Name{get;set;}="";public string Phone{get;set;}="";public string Email{get;set;}="";public string Tier{get;set;}="Regular";}
public class Booking{public int Id{get;set;}public int HotelId{get;set;}public int GuestId{get;set;}public int RoomId{get;set;}public DateOnly CheckIn{get;set;}public DateOnly CheckOut{get;set;}public BookingStatus Status{get;set;}public decimal Total{get;set;}}
public class Payment{public int Id{get;set;}public int BookingId{get;set;}public decimal Amount{get;set;}public string Method{get;set;}="";public DateTime PaidAt{get;set;}}
public class HousekeepingTask{public int Id{get;set;}public int RoomId{get;set;}public string Status{get;set;}="Pending";public string Priority{get;set;}="Normal";public int? StaffId{get;set;}}
public class MaintenanceTicket{public int Id{get;set;}public int RoomId{get;set;}public string Issue{get;set;}="";public string Category{get;set;}="Other";public string Status{get;set;}="Open";public decimal Cost{get;set;}}
public class InventoryItem{public int Id{get;set;}public string Name{get;set;}="";public string Category{get;set;}="";public int Stock{get;set;}public int MinStock{get;set;}public int MaxStock{get;set;}}
public class AppUser{public int Id{get;set;}public int HotelId{get;set;}public string Email{get;set;}="";public string PasswordHash{get;set;}="";public string Role{get;set;}="Receptionist";}
public class AuditLog{public int Id{get;set;}public int UserId{get;set;}public string Action{get;set;}="";public string Entity{get;set;}="";public string? Previous{get;set;}public string? New{get;set;}public DateTime At{get;set;}=DateTime.UtcNow;}
