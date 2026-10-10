# StayFlow API (not yet compiled or tested)
Covers: JWT login, role-based controllers, rooms, availability, bookings, audit log, error middleware.
Run: `dotnet restore && dotnet ef migrations add Init && dotnet ef database update && dotnet run`
Set Jwt:Key via user-secrets. Roles: SuperAdmin, Manager, Receptionist, Housekeeping, Maintenance, Restaurant, Guest.
Not yet included: restaurant, loyalty, feedback, notification and automation entities/endpoints (same pattern as Booking).

## Endpoints
POST /api/auth/login · GET /api/rooms · GET /api/rooms/available · GET/POST /api/bookings · POST /api/checkin · POST /api/checkout · GET /api/dashboard
GET /api/housekeeping/tasks · PUT /api/housekeeping/tasks/{id}/complete · GET/POST /api/maintenance · PUT /api/maintenance/{id}/resolve
GET /api/inventory · POST /api/restaurant/orders · GET /api/notifications · PUT /api/notifications/{id}/read
Checkout requires payment in full, sets the room to Cleaning, creates a housekeeping task and invoice, queues a feedback notification and awards loyalty points.
SQL: run `dotnet ef migrations script -o schema.sql` to generate the schema from the models.
Seed at least one Hotel, RoomTypes, Rooms and an AppUser (BCrypt hash) before logging in.
