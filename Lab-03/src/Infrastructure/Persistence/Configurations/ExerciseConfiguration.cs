using Domain.Exercises;
using Infrastructure.Persistence.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new ExerciseId(x));

        builder.Property(x => x.Name).HasColumnType("varchar(255)").IsRequired();

        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.CaloriesPerHour).HasColumnType("numeric(10,2)").IsRequired();

        builder.Property(x => x.RequiresEquipment).IsRequired();

        builder
            .Property(x => x.MuscleGroup)
            .HasConversion<string>()
            .HasColumnType("varchar(50)")
            .IsRequired();

        builder
            .Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())")
            .IsRequired();
    }
}
