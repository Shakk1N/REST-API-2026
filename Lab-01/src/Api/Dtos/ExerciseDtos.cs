using Domain.Exercises;

namespace Api.Dtos;

public record ExerciseDto(
    Guid Id,
    string Name,
    decimal CaloriesPerHour,
    bool RerquiresEquipment,
    MuscleGroup muscleGroup,
    DateTime CreatedAt)
{
    
}
