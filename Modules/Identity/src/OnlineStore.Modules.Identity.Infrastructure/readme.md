# Migration

For Generating migration files we can use bellow command or for init database based on our DBContext Models we can use of `await identityContext.Database.EnsureCreatedAsync()` that onlt create database base on our model and if database exists before it dosn't do anything.

``` bash
 dotnet ef migrations add Init --context IdentityDbContext
```
Now for doing Migration on our database we can use bellow command or we can use `await identityContext.Database.MigrateAsync()` instruction in our code 

``` bash
dotnet ef database update
```