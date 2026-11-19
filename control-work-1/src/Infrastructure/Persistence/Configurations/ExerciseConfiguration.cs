using Domain.Exercises;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.Persistence.Converters;

namespace Infrastructure.Persistence.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("exercises");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, x => new ExerciseId(x))
            .HasColumnType("uuid");

        builder.Property(x => x.Name)
            .HasColumnType("varchar(255)")
            .HasMaxLength(255)
            .IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();

        builder.Property(x => x.CaloriesPerHour)
            .HasColumnType("numeric(10,2)")
            .IsRequired();

        builder.Property(x => x.RequiresEquipment).IsRequired();

        builder.Property(x => x.MuscleGroup).IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasConversion(new DateTimeUtcConverter())
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("timezone('utc', now())")
            .IsRequired();
    }
}