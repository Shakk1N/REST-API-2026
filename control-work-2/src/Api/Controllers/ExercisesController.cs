using Api.Dtos;
using Api.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Exercises.Commands;
using Domain.Exercises;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("exercises")]
[ApiController]
public class ExercisesController(ISender sender, IExerciseQueries exerciseQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ExerciseDto>>> GetExercises(
        CancellationToken cancellationToken
    )
    {
        var exercises = await exerciseQueries.GetAll(cancellationToken);

        return Ok(exercises.Select(ExerciseDto.FromDomainModel).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExerciseDto>> GetExercise(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var exercise = await exerciseQueries.GetById(new ExerciseId(id), cancellationToken);

        return exercise.Match<ActionResult<ExerciseDto>>(
            e => Ok(ExerciseDto.FromDomainModel(e)),
            () => NotFound()
        );
    }

    [HttpPost]
    public async Task<ActionResult<ExerciseDto>> CreateExercise(
        [FromBody] CreateExerciseDto request,
        CancellationToken cancellationToken
    )
    {
        var command = new CreateExerciseCommand
        {
            Name = request.Name,
            CaloriesPerHour = request.CaloriesPerHour,
            RequiresEquipment = request.RequiresEquipment,
            MuscleGroup = request.MuscleGroup,
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ExerciseDto>>(
            e =>
                CreatedAtAction(
                    nameof(GetExercise),
                    new { id = e.Id.Value },
                    ExerciseDto.FromDomainModel(e)
                ),
            error => error.ToObjectResult()
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ExerciseDto>> UpdateExercise(
        Guid id,
        [FromBody] UpdateExerciseDto request,
        CancellationToken cancellationToken
    )
    {
        var command = new UpdateExerciseCommand
        {
            ExerciseId = id,
            Name = request.Name,
            CaloriesPerHour = request.CaloriesPerHour,
            RequiresEquipment = request.RequiresEquipment,
            MuscleGroup = request.MuscleGroup,
        };

        var result = await sender.Send(command, cancellationToken);

        return result.Match<ActionResult<ExerciseDto>>(
            e => Ok(ExerciseDto.FromDomainModel(e)),
            error => error.ToObjectResult()
        );
    }

    [HttpPost("filter")]
    public async Task<ActionResult<IReadOnlyList<ExerciseDto>>> Filter(
        [FromBody] SearchExerciseDto request,
        CancellationToken cancellationToken
    )
    {
        var exercises = await exerciseQueries.Filter(
            request.Name,
            request.CaloriesPerHourFrom,
            request.CaloriesPerHourTo,
            request.RequiresEquipment,
            request.MuscleGroup,
            request.CreatedAtFrom,
            request.CreatedAtTo,
            cancellationToken
        );

        return Ok(exercises.Select(ExerciseDto.FromDomainModel).ToList());
    }
}
