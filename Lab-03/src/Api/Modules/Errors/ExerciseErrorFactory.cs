using Application.Exercises.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Modules.Errors;

public static class ExerciseErrorFactory
{
    public static ObjectResult ToObjectResult(this ExerciseException error)
    {
        return new ObjectResult(error.Message)
        {
            StatusCode = error switch
            {
                ExerciseAlreadyExistsException => StatusCodes.Status409Conflict,
                ExerciseNotFoundException => StatusCodes.Status404NotFound,
                ExerciseUnhandledException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("IUs not implemented."),
            },
        };
    }
}
