
# Migration database

Execute migration The sample start with SQLite

```powershell
dotnet ef migrations add Initail -c ApplicationDbContext -v 
dotnet ef database update -c ApplicationDbContext -v
```

