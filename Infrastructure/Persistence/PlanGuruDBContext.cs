using Domain.Entities;
using Domain.Entities.ECommerce;
using Domain.Entities.WikiEntities;
using Domain.Entities.WikiService;
using Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Infrastructure.Persistence
{
    public class PlanGuruDBContext : DbContext
    {
        private readonly IMediator _mediator;
        
        public PlanGuruDBContext(DbContextOptions options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator;
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostUpvote> PostUpvotes { get; set; }
        public DbSet<PostDevote> PostDevotes { get; set; }
        public DbSet<PostShare> PostShares { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentUpvote> CommentUpvotes { get; set; }
        public DbSet<CommentDevote> CommentDevotes { get; set; }
        public DbSet<ChatRoom> ChatRooms { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Wiki> Wikis { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
        public DbSet<ContentSection> ContentSections { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupUser> GroupUsers { get; set; }
        public DbSet<Contribution> Contributions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<PostImage> PostImages { get; set; }
        public DbSet<MembershipHistory> MembershipsHistory { get; set; }

        public override int SaveChanges()
        {
            SetAuditFields();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetAuditFields();

            // Save changes first
            var result = await base.SaveChangesAsync(cancellationToken);

            // Collect domain events
            var domainEntities = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseDomainEntity<Guid> entity && entity.DomainEvents.Any())
                .Select(e => (BaseDomainEntity<Guid>)e.Entity)
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(e => e.DomainEvents)
                .ToList();

            // Clear after saving
            domainEntities.ForEach(e => e.ClearDomainEvents());

            // Publish events
            foreach (var domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }

            return result;
        }

        private void SetAuditFields()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity<Guid> && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity<Guid>)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                entity.LastModifiedAt = DateTime.UtcNow;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new PostConfiguration());
            modelBuilder.ApplyConfiguration(new CommentConfiguration());
            modelBuilder.ApplyConfiguration(new PostUpvoteConfiguration());
            modelBuilder.ApplyConfiguration(new PostDevoteConfiguration());
            modelBuilder.ApplyConfiguration(new CommentUpvoteConfiguration());
            modelBuilder.ApplyConfiguration(new CommentDevoteConfiguration());
            modelBuilder.ApplyConfiguration(new PostShareConfiguration());
            modelBuilder.ApplyConfiguration(new ContentSectionConfiguration());
            modelBuilder.ApplyConfiguration(new GroupConfiguration());
            modelBuilder.ApplyConfiguration(new GroupUserConfiguration());

            modelBuilder.Entity<ChatRoom>()
                .HasKey(p => p.ChatRoomId);
            modelBuilder.Entity<ChatRoom>()
                .HasMany(p => p.ChatMessages)
                .WithOne(p => p.ChatRoom)
                .HasForeignKey(p => p.ChatRoomId);
            modelBuilder.Entity<ChatMessage>()
                .HasKey(p => p.ChatMessageId);

            modelBuilder.Entity<MembershipHistory>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId);

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(c => c.Post)
                      .WithMany(p => p.PostComments)
                      .HasForeignKey(c => c.PostId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(c => c.ParentComment)
                      .WithMany()
                      .HasForeignKey(c => c.ParentCommentId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(c => c.CommentUpvotes)
                      .WithOne(cu => cu.Comment)
                      .HasForeignKey(cu => cu.CommentId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasMany(c => c.CommentDevotes)
                      .WithOne(cd => cd.Comment)
                      .HasForeignKey(cd => cd.CommentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Wiki)
                .WithMany(w => w.AttachedProducts)
                .HasForeignKey(p => p.WikiId);

            modelBuilder.Entity<Wiki>()
                .HasMany(w => w.AttachedProducts)
                .WithOne(p => p.Wiki)
                .HasForeignKey(p => p.WikiId);

            modelBuilder.Entity<Vote>(entity =>
            {
                entity.HasKey(v => new { v.UserId, v.TargetId, v.TargetType });
                entity.HasOne(v => v.User)
                      .WithMany(u => u.Votes)
                      .HasForeignKey(v => v.UserId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Product>(p =>
            {
                p.HasKey(p => p.Id);
                p.HasMany(p => p.ProductImages)
                    .WithOne(p => p.Product)
                    .HasForeignKey(p => p.ProductId);
                p.HasOne(p => p.Seller)
                    .WithMany()
                    .HasForeignKey(p => p.SellerId)
                    .OnDelete(DeleteBehavior.Restrict);
                p.HasOne(p => p.Wiki)
                    .WithMany()
                    .HasForeignKey(p => p.WikiId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Order>(p =>
            {
                p.HasKey(p => p.Id);
                p.HasOne(p => p.User)
                    .WithMany()
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                p.HasOne(p => p.Product)
                    .WithMany()
                    .HasForeignKey(p => p.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Membership>(p =>
            {
                p.HasKey(p => p.Id);
            });
            // Configure Contribution entity
            modelBuilder.Entity<Contribution>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.HasOne(c => c.Wiki)
                      .WithMany(w => w.Contributions)
                      .HasForeignKey(c => c.WikiId);
                entity.HasOne(c => c.Contributor)
                      .WithMany(u => u.Contributions)
                      .HasForeignKey(c => c.ContributorId);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
