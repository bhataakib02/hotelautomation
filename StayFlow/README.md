# StayFlow: deployable package

## frontend/ (works as-is)
A single static page, `index.html`, with all demo data built in. No build step and no server needed.
Deploy by uploading the `frontend` folder to any static host:
- Netlify: drag the `frontend` folder onto app.netlify.com/drop
- Vercel: `cd frontend && npx vercel --prod`
- GitHub Pages, Cloudflare Pages, or any web server (Nginx, Apache, IIS): serve `index.html`
- Local: open `index.html` in a browser, or run `npx serve frontend`
It loads Google Fonts and a QR library from CDNs, so it needs internet access.
Demo logins (password `stay123`): admin, manager, reception, housekeeping, maintenance, kitchen, guest @aurelia.in

Data lives in memory, so a page refresh resets everything. This is a demo, not a production system.

## backend/ (starter, not connected)
ASP.NET Core 8 API with JWT auth, roles, bookings, check-in/out, housekeeping, maintenance, inventory, restaurant orders and notifications.
It has not been compiled or tested, and the frontend does not call it yet.
1. Install the .NET 8 SDK and SQL Server.
2. Edit `appsettings.json` (connection string and a long random `Jwt:Key`).
3. `dotnet restore && dotnet ef migrations add Init && dotnet ef database update && dotnet run`
4. Seed a Hotel, RoomTypes, Rooms and an AppUser with a BCrypt password hash.
See `backend/README.md` for the endpoint list.
