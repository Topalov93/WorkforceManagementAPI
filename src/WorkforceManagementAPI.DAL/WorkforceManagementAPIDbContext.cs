using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Collections.Generic;
using WorkforceManagementAPI.DAL.Entities;

namespace WorkforceManagementAPI.DAL
{
    public class WorkforceManagementAPIDbContext : IdentityDbContext<User>
    {
        public WorkforceManagementAPIDbContext(DbContextOptions<WorkforceManagementAPIDbContext> options) : base(options)
        {

        }

        public DbSet<Team> Teams { get; set; }

        public DbSet<TimeOffRequest> TimeOffRequests { get; set; }

        public DbSet<OfficialBulgarianHolidays> Holidays { get; set; }

        public DbSet<Approval> Approvals { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Team>()
                .HasOne(t => t.TeamLeader)
                .WithMany(l => l.LeadTeams);

            builder.Entity<Team>()
                .HasMany(t => t.TeamMembers)
                .WithMany(m => m.Teams);

            builder.Entity<TimeOffRequest>()
                .HasOne(r => r.Creator)
                .WithMany(c => c.CreatedRequests);

            builder.Entity<TimeOffRequest>()
                .HasMany(r => r.Approvers);

            builder.Entity<TimeOffRequest>()
                .HasMany(r => r.Approvals)
                .WithOne(a => a.TimeOffRequest);

            var splitStringConverter = new ValueConverter<IEnumerable<string>, string>(v => string.Join(";", v), v => v.Split(new[] { ';' }));
            builder.Entity<TimeOffRequest>().Property(nameof(TimeOffRequest.LeadersEmails)).HasConversion(splitStringConverter);


            builder.Entity<User>()
                .Property(u => u.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<User>()
                .Property(u => u.ModifiedOn)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<Team>()
                .Property(u => u.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<Team>()
                .Property(u => u.ModifiedOn)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<TimeOffRequest>()
                .Property(u => u.CreatedOn)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<TimeOffRequest>()
                .Property(u => u.ModifiedOn)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
