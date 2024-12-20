﻿using Domain.Entities;
using Domain.Entities.ECommerce;
using Domain.Entities.WikiService;
using Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class PlanGuruDBContext : DbContext
    {
        public PlanGuruDBContext(DbContextOptions options) : base(options) { }
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
        public DbSet<Order> Orders { get; set; }
        public DbSet<Membership> Memeberships { get; set; }     

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

            modelBuilder.Entity<Comment>(entity =>
            {
                // Thiết lập khóa chính cho Comment
                entity.HasKey(c => c.Id);

                // Quan hệ 1-n với User (1 User có nhiều Comment)
                entity.HasOne(c => c.User)
                      .WithMany(u => u.Comments)  // Assumes User entity has ICollection<Comment> Comments
                      .HasForeignKey(c => c.UserId)
                      .OnDelete(DeleteBehavior.Restrict); // Tránh xóa cascade

                // Quan hệ 1-n với Post (1 Post có nhiều Comment)
                entity.HasOne(c => c.Post)
                      .WithMany(p => p.PostComments)  // Assumes Post entity has ICollection<Comment> Comments
                      .HasForeignKey(c => c.PostId)
                      .OnDelete(DeleteBehavior.Restrict); // Tránh xóa cascade

                // Quan hệ đệ quy với ParentComment (1 Comment có thể là cha của nhiều Comment)
                entity.HasOne(c => c.ParentComment)
                      .WithMany() // Không cần collection ở Comment cho ParentComment
                      .HasForeignKey(c => c.ParentCommentId)
                      .OnDelete(DeleteBehavior.Restrict); // Ngăn xóa cascade để tránh vòng lặp

                // Cấu hình ICollection<CommentUpvote> và ICollection<CommentDevote>
                entity.HasMany(c => c.CommentUpvotes)
                      .WithOne(cu => cu.Comment)  // Assumes CommentUpvote has Comment navigation property
                      .HasForeignKey(cu => cu.CommentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.CommentDevotes)
                      .WithOne(cd => cd.Comment)  // Assumes CommentDevote has Comment navigation property
                      .HasForeignKey(cd => cd.CommentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Cấu hình quan hệ giữa Wiki và Product: 1-n
            modelBuilder.Entity<Product>()
                        .HasOne(p => p.Wiki)
                        .WithMany(w => w.AttachedProducts)
                        .HasForeignKey(p => p.WikiId);
            //.OnDelete(DeleteBehavior.SetNull); // Khi xóa Wiki, WikiId trong Product được đặt thành null

            modelBuilder.Entity<Wiki>()
            .HasMany(w => w.AttachedProducts) // Wiki có nhiều Product
            .WithOne(p => p.Wiki) // Product có một Wiki
            .HasForeignKey(p => p.WikiId); // Khóa ngoại trong Product
            //.OnDelete(DeleteBehavior.SetNull); // Set WikiId = null nếu Wiki bị xóa nếu có Product đang tham chiếu


            // Cấu hình quan hệ giữa Vote và User: 1-n
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

            base.OnModelCreating(modelBuilder);
        }
    }
}
