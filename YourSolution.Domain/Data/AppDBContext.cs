using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YourSolution.Domain.Models;
using YourSolution.Infrastructure.Models;

namespace YourSolution.Domain.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<RequestLog> RequestLogs { get; set; }
        public DbSet<SysCode> SysCodes { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<IpWhitelist> IpWhitelists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SysCode>(entity =>
            {
                entity.Property(b => b.CreateTime)
                      .HasDefaultValueSql("getdate()"); //執行指定的SQL日期函式

                entity.Property(b => b.UpdateTime)
                      .HasDefaultValueSql("getdate()"); //執行指定的SQL日期函式
            });

            modelBuilder.Entity<UserAccount>(entity =>
            {
                // 設定主鍵，但不使用叢級索引
                entity.HasKey(e => e.FlowId).IsClustered(false);

                // 為另一個欄位建立叢級索引
                entity.HasIndex(e => e.Id).IsClustered();

                entity.Property(b => b.Id)
                      .ValueGeneratedOnAdd() //設定自動產生值
                      .HasPrecision(18, 0);

                entity.Property(b => b.Role)
                      .HasConversion<byte>();

                entity.Property(b => b.CreateTime)
                      .HasDefaultValueSql("getdate()"); //執行指定的SQL日期函式

                entity.Property(b => b.UpdateTime)
                      .HasDefaultValueSql("getdate()"); //執行指定的SQL日期函式
            });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //以下這段要刪除，連線字串放到appsettings.json
                //optionsBuilder.UseSqlServer("Server=.;Database=FreewaySouthern.FaceRecognition;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True;");
            }
        }
    }
}
