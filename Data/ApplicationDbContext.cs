using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TodoApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
       public DbSet<TodoApp.Models.Todo>? Todos { get; set; }
       public DbSet<TodoApp.Models.AppUser>? AppUsers { get; set; }
    }
}