using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<TeamEntity> Teams => Set<TeamEntity>();
        public DbSet<TaskItem> TaskItems => Set<TaskItem>();
        public DbSet<TaskComment> TaskComments => Set<TaskComment>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();

                entity.HasOne(u => u.Team)
                      .WithMany(t => t.Members)
                      .HasForeignKey(u => u.TeamId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<TeamEntity>(entity =>
            {
                entity.HasOne(t => t.Manager)
                      .WithMany()
                      .HasForeignKey(t => t.ManagerId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.HasOne(t => t.Team)
                      .WithMany(team => team.Tasks)
                      .HasForeignKey(t => t.TeamId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(t => t.AssignedTo)
                      .WithMany(u => u.AssignedTasks)
                      .HasForeignKey(t => t.AssignedToId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(t => t.CreatedBy)
                      .WithMany(u => u.CreatedTasks)
                      .HasForeignKey(t => t.CreatedById)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TaskComment>(entity =>
            {
                entity.HasOne(c => c.TaskItem)
                      .WithMany(t => t.Comments)
                      .HasForeignKey(c => c.TaskItemId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                      .WithMany(u => u.Notifications)
                      .HasForeignKey(n => n.UserId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
