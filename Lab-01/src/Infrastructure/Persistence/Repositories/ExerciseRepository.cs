using Application.Common.Interfaces;
using Domain.Exercises;

namespace Infrastructure.Persistence.Repositories;

public class ExerciseRepository : IExerciseRepository
{
    private readonly List<Exercise> _exercises = [];

    public Task<IReadOnlyList<Exercise>> GetAll(CancellationToken cancellationToken)
    {
        lock (_exercises)
        {
            return Task.FromResult<IReadOnlyList<Exercise>>(_exercises.ToList());
        }
    }

    public Task<Exercise?> GetById(Guid id, CancellationToken cancellationToken)
    {
        lock (_exercises)
        {
            return Task.FromResult(_exercises.FirstOrDefault(x => x.Id == id));
        }
    }

    public Task<Exercise?> GetByName(string name, CancellationToken cancellationToken)
    {
        lock (_exercises)
        {
            var exercise = _exercises.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
            return Task.FromResult(exercise);
        }
    }

    public Task<Exercise> Add(Exercise exercise, CancellationToken cancellationToken)
    {
        lock (_exercises)
        {
            _exercises.Add(exercise);
            return Task.FromResult(exercise);
        }
    }

    public Task<Exercise> Update(Exercise exercise, CancellationToken cancellationToken)
    {
        lock (_exercises)
        {
            var index = _exercises.FindIndex(x => x.Id == exercise.Id);
            if (index != -1)
            {
                _exercises[index] = exercise;
            }
            return Task.FromResult(exercise);
        }
    }

    public Task Delete(Exercise exercise, CancellationToken cancellationToken)
    {
        lock (_exercises)
        {
            _exercises.RemoveAll(x => x.Id == exercise.Id);
        }
        return Task.CompletedTask;
    }
}