using Api.Dtos;
using Application.Exercises.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("exercises")]
[ApiController]
public class ExercisesController(IExerciseService exerciseService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ExerciseDto>>> GetExercises(CancellationToken cancellationToken)
    {
        var exercises = await exerciseService.GetExercises(cancellationToken);
        return Ok(exercises.Select(ExerciseDto.FromDomainModel).ToList());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ExerciseDto>> GetExercise(Guid id, CancellationToken cancellationToken)
    {
        var exercise = await exerciseService.GetExercise(id, cancellationToken);
        if (exercise is null)
        {
            return NotFound();
        }

        return Ok(ExerciseDto.FromDomainModel(exercise));
    }

    [HttpPost]
    public async Task<ActionResult<ExerciseDto>> CreateExercise(
        [FromBody] CreateExerciseDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var exercise = await exerciseService.Add(
                request.Name,
                request.CaloriesPerHour,
                request.RequiresEquipment,
                request.MuscleGroup,
                cancellationToken);

            var dto = ExerciseDto.FromDomainModel(exercise);
            return CreatedAtAction(nameof(GetExercise), new { id = dto.Id }, dto);
        }
        catch (ArgumentException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ExerciseDto>> UpdateExercise(
        Guid id,
        [FromBody] UpdateExerciseDto request,
        CancellationToken cancellationToken)
    {
        try
        {
            var exercise = await exerciseService.Update(
                id,
                request.Name,
                request.CaloriesPerHour,
                request.RequiresEquipment,
                request.MuscleGroup,
                cancellationToken);

            if (exercise is null)
            {
                return NotFound();
            }

            return Ok(ExerciseDto.FromDomainModel(exercise));
        }
        catch (ArgumentException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteExercise(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await exerciseService.Delete(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}

