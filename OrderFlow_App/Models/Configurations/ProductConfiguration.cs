using OrderFlow_App.Models.DomainModels.ProductAggregates;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow_App.Models.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products", "Basic");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.GuidKey).IsRequired();
            builder.Property(p => p.Title).IsRequired().HasMaxLength(200);
            builder.Property(p => p.RecordDescription).HasMaxLength(500);
            builder.Property(p => p.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
