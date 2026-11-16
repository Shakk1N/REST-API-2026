using Domain.Exercises;
using LanguageExt;

namespace Application.Common.Interfaces.Repositories;

public interface IExerciseRepository
{
    Task<Option<Exercise>> GetById(ExerciseId id, CancellationToken cancellationToken);
    Task<Option<Exercise>> GetByName(string name, CancellationToken cancellationToken);
    Task<Exercise> Add(Exercise exercise, CancellationToken cancellationToken);
    Task<Exercise> Update(Exercise exercise, CancellationToken cancellationToken);
}
