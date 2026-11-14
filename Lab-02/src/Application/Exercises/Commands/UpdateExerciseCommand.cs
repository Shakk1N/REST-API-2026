using Application.Common.Interfaces.Repositories;
using Domain.Exercises;
using MediatR;

namespace Application.Exercises.Commands;

public record UpdateExerciseCommand : IRequest<Exercise?>
{
    public required Guid ExerciseId { get; init; }
    public required string Name { get; init; }
    public required MuscleGroup MuscleGroup { get; init; }
}

public class UpdateExerciseCommandHandler(IExerciseRepository exerciseRepository)
    : IRequestHandler<UpdateExerciseCommand, Exercise?>
{
    public async Task<Exercise?> Handle(
        UpdateExerciseCommand request,
        CancellationToken cancellationToken)
    {
        var exercise = await exerciseRepository.GetById(request.ExerciseId, cancellationToken);
        if (exercise is null)
        {
            return null;
        }

        var existing = await exerciseRepository.GetByName(request.Name, cancellationToken);
        if (existing is not null && existing.Id != exercise.Id)
        {
            throw new ArgumentException($"Exercise with name '{request.Name}' already exists");
        }

        exercise.UpdateDetails(request.Name, request.MuscleGroup);
        return await exerciseRepository.Update(exercise, cancellationToken);
    }
}