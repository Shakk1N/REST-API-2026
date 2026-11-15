using Domain.Exercises;

namespace Application.Exercises.Exceptions;

public abstract class ExerciseException(
    ExerciseId exerciseId,
    string message,
    Exception? innerException = null
) : Exception(message, innerException)
{
    public ExerciseId ExerciseId { get; } = exerciseId;
}

public class ExerciseAlreadyExistsException(ExerciseId exerciseId)
    : ExerciseException(exerciseId, $"Exercise already exists under id {exerciseId}");

public class ExerciseNotFoundException(ExerciseId exerciseId)
    : ExerciseException(exerciseId, $"Exercise not found under id {exerciseId}");

public class ExerciseUnhandledException(ExerciseId exerciseId, Exception? innerException = null)
    : ExerciseException(exerciseId, $"Unhandled exception under id {exerciseId}", innerException);
