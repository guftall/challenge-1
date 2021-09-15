##Migrations

Add *Postgres* connection string to appsettings.json file as DefaultConnection.

`dotnet ef migrations add App -c ApplicationDbContext -p Auth`

`dotnet ef migrations add Per -c PersistedGrantDbContext -p Auth`

`dotnet ef migrations add Conf -c ConfigurationDbContext -p Auth`

`dotnet ef database update -p Auth`

`dotnet ef migrations add Ads -c AdsDbContext -p HiliTechChallenge`

`dotnet ef database update -p HiliTechChallenge`

## Run
Set these dns records to *hosts* file (Windows):
- hili.guftall.ir 127.0.0.1
- admin.hili.guftall.ir 127.0.0.1
- user.hili.guftall.ir 127.0.0.1

launch projects and login as admin by username: `admin@hili.guftall.ir` and password `admin`.