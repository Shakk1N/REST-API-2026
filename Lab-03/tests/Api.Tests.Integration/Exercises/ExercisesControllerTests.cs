using System.Net.Http.Json;
using Api.Dtos;
using Domain.Exercises;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Xunit;

namespace Api.Tests.Integration.Exercises;

public class ExercisesControllerTests : BaseIntegrationTest
{
    public ExercisesControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task ShouldCreateExercise()
    {
        // Arrange
        var request = new CreateExerciseDto(
            Name: "Test Exercise",
            CaloriesPerHour: 450m,
            RequiresEquipment: true,
            MuscleGroup: MuscleGroup.Chest);

        // Act
        var response = await Client.PostAsJsonAsync("exercises", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeTrue();
        var responseDto = await response.Content.ReadFromJsonAsync<ExerciseDto>();
        responseDto.Should().NotBeNull();
        responseDto!.Name.Should().Be(request.Name);
        responseDto.CaloriesPerHour.Should().Be(request.CaloriesPerHour);
        responseDto.RequiresEquipment.Should().Be(request.RequiresEquipment);
        responseDto.MuscleGroup.Should().Be(request.MuscleGroup);

        var dbExercise = await Context.Exercises.FirstOrDefaultAsync(x => x.Id == new ExerciseId(responseDto.Id));
        dbExercise.Should().NotBeNull();
        dbExercise!.Name.Should().Be(request.Name);
        dbExercise.CaloriesPerHour.Should().Be(request.CaloriesPerHour);
        dbExercise.RequiresEquipment.Should().Be(request.RequiresEquipment);
        dbExercise.MuscleGroup.Should().Be(request.MuscleGroup);
    }
}
