using ImageStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Emit;

namespace ImageStorage.Infrastructure.DbAccess.Configuration;

internal class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.ToTable("Images");

        builder.Property(p => p.Id).ValueGeneratedNever();

        builder.Property(x => x.FileName).HasMaxLength(256);
    }
}
