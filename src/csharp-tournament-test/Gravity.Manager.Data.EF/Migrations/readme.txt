Use the following command to create migrations:
dotnet ef --startup-project ../Gravity.Manager.Web/ migrations add TODO_MyNewMigrationName

Current project is a class library and thus requires a startup project.
Make sure that startup project uses our DbContext in some way (options builder with some connection is required):
services.AddDbContext<GravityManagerDbContext>(builder => builder.UseMySQL("..."));

To apply migrations to an existing DB or create tables in an empty DB use:
dotnet ef --startup-project ../Gravity.Manager.Web/ database update