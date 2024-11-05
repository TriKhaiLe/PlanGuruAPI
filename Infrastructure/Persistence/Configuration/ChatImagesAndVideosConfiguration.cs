﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configuration
{
    public class ChatImagesAndVideosConfiguration : IEntityTypeConfiguration<ChatImageAndVideo>
    {
        public void Configure(EntityTypeBuilder<ChatImageAndVideo> builder)
        {
            builder.HasKey(p => p.Id);
        }
    }
}
