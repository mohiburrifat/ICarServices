using Microsoft.EntityFrameworkCore;
using I_Car_Services.Models;

namespace I_Car_Services.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        // DbSets representing tables in your database
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<MyServiceProvider> MyServiceProviders { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Balance> Balances { get; set; }
         public DbSet<ChatMessage> ChatMessages { get; set; }

    }
}

