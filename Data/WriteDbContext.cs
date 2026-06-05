using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrdersAPI.Models;

namespace OrdersAPI.Data
{
    public class WriteDbContext : DbContext
    {
        public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options)
        {
            
        }

        public DbSet<Order> Orders { get; set; } = null!;
    }
}


// dotnet ef migrations add WriteDbMigrations --context WriteDbContext --output-dir Migrations/Write    // this command is used to create a new migration for the WriteDbContext and specify the output directory for the migration files. The migration will be named "WriteDbMigrations". This command is part of the Entity Framework Core tools and is used to manage database schema changes in a code-first approach.
// dotnet ef database update --context WriteDbContext  // this command is used to apply the pending migrations for the WriteDbContext to the database. It will create or update the database schema based on the migrations that have been defined for the WriteDbContext. This command is also part of the Entity Framework Core tools and is used to keep the database schema in sync with the application's data model.