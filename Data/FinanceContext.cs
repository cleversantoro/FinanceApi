using Microsoft.EntityFrameworkCore;
using FinanceApi.Models;

namespace FinanceApi.Data;

public class FinanceContext : DbContext
{
    public FinanceContext(DbContextOptions<FinanceContext> options) : base(options) { }

    public DbSet<RegistroFinanceiro> Registros { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

}
