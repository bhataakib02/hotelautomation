namespace StayFlow.Api.Models;
public class Notification{public int Id{get;set;}public int HotelId{get;set;}public string Title{get;set;}="";public string Body{get;set;}="";public string Department{get;set;}="All";public bool IsRead{get;set;}public DateTime At{get;set;}=DateTime.UtcNow;}
public class Feedback{public int Id{get;set;}public int BookingId{get;set;}public int Rating{get;set;}public string Comment{get;set;}="";public DateTime At{get;set;}=DateTime.UtcNow;}
public class LoyaltyAccount{public int Id{get;set;}public int GuestId{get;set;}public int Points{get;set;}public string Level{get;set;}="Bronze";}
public class LoyaltyTransaction{public int Id{get;set;}public int AccountId{get;set;}public int Points{get;set;}public string Reason{get;set;}="";public DateTime At{get;set;}=DateTime.UtcNow;}
public class Staff{public int Id{get;set;}public int HotelId{get;set;}public string Name{get;set;}="";public string Department{get;set;}="";public string Position{get;set;}="";}
public class Shift{public int Id{get;set;}public int StaffId{get;set;}public DateOnly Date{get;set;}public string Type{get;set;}="Morning";}
public class Supplier{public int Id{get;set;}public string Name{get;set;}="";public string Contact{get;set;}="";}
public class MenuItem{public int Id{get;set;}public int HotelId{get;set;}public string Name{get;set;}="";public string Category{get;set;}="";public decimal Price{get;set;}public bool Available{get;set;}=true;}
public class RestaurantOrder{public int Id{get;set;}public int HotelId{get;set;}public int? RoomId{get;set;}public int? TableNo{get;set;}public string Status{get;set;}="New";public decimal Total{get;set;}public List<OrderItem> Items{get;set;}=new();}
public class OrderItem{public int Id{get;set;}public int OrderId{get;set;}public int MenuItemId{get;set;}public int Qty{get;set;}public decimal UnitPrice{get;set;}}
public class Invoice{public int Id{get;set;}public int BookingId{get;set;}public string Number{get;set;}="";public decimal Total{get;set;}public DateTime IssuedAt{get;set;}=DateTime.UtcNow;}
public class AutomationRule{public int Id{get;set;}public int HotelId{get;set;}public string Trigger{get;set;}="";public string Action{get;set;}="";public bool Enabled{get;set;}=true;}
