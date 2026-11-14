using System.ComponentModel.DataAnnotations;
using Domain.Exercises;

namespace Api.Dtos;

public record ExerciseDto(
    Guid Id,
    string Name,
    decimal CaloriesPerHour,
    bool RequiresEquipment,
    MuscleGroup MuscleGroup,
    DateTime CreatedAt)
{
    public static ExerciseDto FromDomainModel(Exercise exercise) =>
        new(
            exercise.Id,
            exercise.Name,
            exercise.CaloriesPerHour,
            exercise.RequiresEquipment,
            exercise.MuscleGroup,
            exercise.CreatedAt
        );
}

public record CreateExerciseDto(
    [Required, MaxLength(255)] string Name,
    [Range(0.01, 10000)] decimal CaloriesPerHour,
    bool RequiresEquipment,
    MuscleGroup MuscleGroup
);

public record UpdateExerciseDto(
    [Required, MaxLength(255)] string Name,
    [Range(0.01, 10000)] decimal CaloriesPerHour,
    bool RequiresEquipment,
    MuscleGroup MuscleGroup
);
