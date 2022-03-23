using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using tp_nt1.Models;

namespace tp_nt1.Database
{
    public class CarritoDbContext : DbContext
    {
        public CarritoDbContext(DbContextOptions<CarritoDbContext> opciones) : base(opciones)
        { }
        
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Administrador> Administradores { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Carrito> Carritos { get; set; }
        public DbSet<CarritoItem> CarritoItems { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<StockItem> StockItems { get; set; }
        public DbSet<Sucursal> Sucursales { get; set; }
        
    }
}
