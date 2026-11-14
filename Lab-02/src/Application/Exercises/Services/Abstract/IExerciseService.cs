using Domain.Exercises;

namespace Application.Exercises.Services.Abstract;

public interface IExerciseService
{
    Task<IReadOnlyList<Exercise>> GetExercises(CancellationToken cancellationToken);
    Task<Exercise?> GetExercise(Guid id, CancellationToken cancellationToken);
    Task<Exercise> Add(string name, decimal caloriesPerHour, bool requiresEquipment, MuscleGroup muscleGroup, CancellationToken cancellationToken);
    Task<Exercise?> Update(Guid id, string name, decimal caloriesPerHour, bool requiresEquipment, MuscleGroup muscleGroup, CancellationToken cancellationToken);
    Task<bool> Delete(Guid id, CancellationToken cancellationToken);
}