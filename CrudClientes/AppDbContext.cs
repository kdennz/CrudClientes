using Microsoft.EntityFrameworkCore;

namespace CrudClientes;

public class AppDbContext : DbContext
{
    // Cada DbSet equivale a una tabla
    public DbSet<Cliente> Clientes => Set<Cliente>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
        optionsBuilder.UseSqlServer(
            @"Server=KDEN\SQLEXPRESS;Database=CrudDB;Trusted_Connection=True;TrustServerCertificate=True;");
    }
}