using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configuration
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasMany(p => p.PostInGroup)
                .WithOne(p => p.Group)
                .HasForeignKey(p => p.GroupId);

            builder.HasOne(p => p.MasterUser)
                .WithMany(x => x.ListOwnGroup)
                .HasForeignKey(p => p.MasterUserId);
        }
    }
}
