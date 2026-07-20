using OrderFlow_App.Models.DomainModels.OrderAggregates;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow_App.Models.Configurations
{
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.ToTable("OrderDetails", "Sales");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.GuidKey).IsRequired();
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.Amount).HasColumnType("decimal(18,2)").IsRequired();
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);
            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.HasOne<OrderHeader>()
                   .WithMany()
                   .HasForeignKey(x => x.OrderHeaderId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
