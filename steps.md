### Terminal commands
```
// only need to do this once on our computer.
dotnet tool install --global dotnet-ef

//Adding packages
dotnet add package Pomelo.EntityFrameworkCore.MySql --version 3.1.1
dotnet add package Microsoft.EntityFrameworkCore.Design --version 3.1.5
```
#### After create the classes we need to create the migration.
```
// Create the migration - Change the YourMigrationName to the name of the migration
dotnet ef migrations add YourMigrationName

// Apply the migration
dotnet ef database update
```

### Files to Modify
- modify appsettings.json
-- change the databasename

- modify startup.cs
-- change the connection string
<pre><code>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MyContext>(options => options.UseMySql(Configuration["DBInfo:ConnectionString"]));
            
            services.AddControllersWithViews();
        }
</code></pre>

- create MyContext.cs file
-- Example bellow
<pre><code>
using Microsoft.EntityFrameworkCore;

namespace CRUDelicious.Models
{ 
    // the MyContext class representing a session with our MySQL 
    // database allowing us to query for or save data
    public class MyContext : DbContext 
    { 
        public MyContext(DbContextOptions options) : base(options) { }
        // the "Monsters" table name will come from the DbSet variable name
        public DbSet<Dish> Dishes { get; set; }
    }
}
</code></pre>

- modify HomeController.cs
-- Example bellow
<pre><code>
 public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private MyContext dbContext;
    
        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context,ILogger<HomeController> logger)
        {
            _logger = logger;
            dbContext = context;
        }
        // the Index method will query the database bellow
</code></pre>