using core.domain.entity.structureModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace core.infrastructure.Data.persist
{
    internal class ComplexConfiguration : IEntityTypeConfiguration<ComplexModel>
    {
        public void Configure(EntityTypeBuilder<ComplexModel> builder)
        {
            builder.HasIndex(c => c.Title).IsUnique();

        }
    }
}
