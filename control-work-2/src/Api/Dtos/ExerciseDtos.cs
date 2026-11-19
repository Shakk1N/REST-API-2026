using Domain.Exercises;

namespace Api.Dtos;

public record ExerciseDto(
    Guid Id,
    string Name,
    decimal CaloriesPerHour,
    bool RequiresEquipment,
    MuscleGroup MuscleGroup,
    DateTime CreatedAt
)
{
    public static ExerciseDto FromDomainModel(Exercise exercise) =>
        new(
            exercise.Id.Value,
            exercise.Name,
            exercise.CaloriesPerHour,
            exercise.RequiresEquipment,
            exercise.MuscleGroup,
            exercise.CreatedAt
        );
}

public record CreateExerciseDto(
    string Name,
    decimal CaloriesPerHour,
    bool RequiresEquipment,
    MuscleGroup MuscleGroup
);

public record UpdateExerciseDto(
    string Name,
    decimal CaloriesPerHour,
    bool RequiresEquipment,
    MuscleGroup MuscleGroup
);
