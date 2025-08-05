using KapadaModel.Item;
using KapadaModel.MainItem;
using KapadaModel.Model;
using KapadaModel.Room;
using KapadaModel.SalesEntity;
using Microsoft.EntityFrameworkCore;

namespace MyApp.Api.DataContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<RegisterUser> RegsiterUser { get; set; }

        public DbSet<ItemCategory> Items { get; set; }

        public DbSet<RoomItem> RoomName { get; set; }

        public DbSet<Inventory> Inventory { get; set; }

        public DbSet<Sale> Sales { get; set; }
    }
}
