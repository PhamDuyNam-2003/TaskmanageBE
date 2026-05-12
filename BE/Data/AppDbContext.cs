using Microsoft.EntityFrameworkCore;
using BE.Models;

namespace BE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(
            DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<Project> Projects => Set<Project>();

        public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();

        public DbSet<TaskItem> TaskItems => Set<TaskItem>();

        public DbSet<TaskCollaborator> TaskCollaborators
            => Set<TaskCollaborator>();

        public DbSet<Comment> Comments => Set<Comment>();

        public DbSet<SubTask> SubTasks => Set<SubTask>();


        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Username)
                .IsUnique();


            modelBuilder.Entity<Project>()
                .HasOne(x => x.Owner)
                .WithMany(x => x.OwnedProjects)
                .HasForeignKey(x => x.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<ProjectMember>()
                .HasKey(x => new
                {
                    x.ProjectId,
                    x.UserId
                });

            modelBuilder.Entity<ProjectMember>()
                .HasOne(x => x.Project)
                .WithMany(x => x.Members)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProjectMember>()
                .HasOne(x => x.User)
                .WithMany(x => x.ProjectMembers)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<TaskItem>()
                .HasOne(x => x.Project)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskItem>()
                .HasOne(x => x.CreatedBy)
                .WithMany(x => x.CreatedTasks)
                .HasForeignKey(x => x.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TaskItem>()
                .HasOne(x => x.AssignedTo)
                .WithMany(x => x.AssignedTasks)
                .HasForeignKey(x => x.AssignedToId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<TaskCollaborator>()
                .HasKey(x => new
                {
                    x.TaskItemId,
                    x.UserId
                });

            modelBuilder.Entity<TaskCollaborator>()
                .HasOne(x => x.TaskItem)
                .WithMany(x => x.TaskCollaborators)
                .HasForeignKey(x => x.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TaskCollaborator>()
                .HasOne(x => x.User)
                .WithMany(x => x.TaskCollaborators)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Comment>()
                .HasOne(x => x.TaskItem)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Comment>()
                .HasOne(x => x.User)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);



            modelBuilder.Entity<SubTask>()
                .HasOne(x => x.TaskItem)
                .WithMany(x => x.SubTasks)
                .HasForeignKey(x => x.TaskItemId)
                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<TaskItem>()
                .Property(x => x.RowVersion)
                .IsRowVersion();
        }
    }
}