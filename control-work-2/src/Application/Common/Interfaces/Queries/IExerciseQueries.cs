using Domain.Exercises;
using LanguageExt;

namespace Application.Common.Interfaces.Queries;

public interface IExerciseQueries
{
    Task<IReadOnlyList<Exercise>> GetAll(CancellationToken cancellationToken);
    Task<Option<Exercise>> GetById(ExerciseId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Exercise>> Filter(
        string? name,
        decimal? caloriesPerHourFrom,
        decimal? caloriesPerHourTo,
        bool? requiresEquipment,
        MuscleGroup? muscleGroup,
        DateTime? createdAtFrom,
        DateTime? createdAtTo,
        CancellationToken cancellationToken
    );
}
