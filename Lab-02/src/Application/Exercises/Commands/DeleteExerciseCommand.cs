using Application.Common.Interfaces.Repositories;
using Application.Exercises.Exceptions;
using Domain.Exercises;
using LanguageExt;
using MediatR;

namespace Application.Exercises.Commands;

public record DeleteExerciseCommand : IRequest<Either<ExerciseException, Exercise>>
{
    public required Guid ExerciseId { get; init; }
}

public class DeleteExerciseCommandHandler(IExerciseRepository exerciseRepository)
    : IRequestHandler<DeleteExerciseCommand, Either<ExerciseException, Exercise>>
{
    public async Task<Either<ExerciseException, Exercise>> Handle(
        DeleteExerciseCommand request,
        CancellationToken cancellationToken
    )
    {
        var exerciseId = new ExerciseId(request.ExerciseId);
        var exercise = await exerciseRepository.GetById(exerciseId, cancellationToken);

        return await exercise.MatchAsync(
            e => DeleteEntity(e, cancellationToken),
            () => new ExerciseNotFoundException(exerciseId)
        );
    }

    private async Task<Either<ExerciseException, Exercise>> DeleteEntity(
        Exercise exercise,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await exerciseRepository.Delete(exercise, cancellationToken);

            return exercise;
        }
        catch (Exception exception)
        {
            return new ExerciseUnhandledException(exercise.Id, exception);
        }
    }
}
