using Microsoft.EntityFrameworkCore;
using LRP_Project_Vize_Nazlı_Ceren_Uğur.Models;

namespace LRP_Project_Vize_Nazlı_Ceren_Uğur.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Veritabanındaki tablolarımızı temsil eden setler
    public DbSet<User> Users { get; set; }
    public DbSet<Laboratory> Laboratories { get; set; }
    public DbSet<Computer> Computers { get; set; }
}