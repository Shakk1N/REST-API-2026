using System.Net;
using System.Net.Http.Json;
using Api.Dtos;
using Domain.Exercises;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Common;
using Tests.Data;
using Xunit;

namespace Api.Tests.Integration;

public class ExercisesControllerTests : BaseIntegrationTest, IAsyncLifetime
{
    private Exercise _firstExercise = null!;
    private Exercise _secondExercise = null!;

    public ExercisesControllerTests(IntegrationTestWebFactory factory) : base(factory)
    {
    }

    public async Task InitializeAsync()
    {
        _firstExercise = ExercisesTestData.FirstTestExercise();
        _secondExercise = ExercisesTestData.SecondTestExercise();

        await Context.Exercises.AddRangeAsync(_firstExercise, _secondExercise);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    public async Task DisposeAsync()
    {
        Context.Exercises.RemoveRange(Context.Exercises);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task CreateExercise_WithValidData_ReturnsCreatedAndPersistsToDb()
    {
        // Arrange
        var request = new CreateExerciseDto(
            Name: "Unique Deadlift",
            CaloriesPerHour: 600m,
            RequiresEquipment: true,
            MuscleGroup: MuscleGroup.Back
        );

        // Act
        var response = await Client.PostAsJsonAsync("/exercises", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<ExerciseDto>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.Name.Should().Be(request.Name);
        result.CaloriesPerHour.Should().Be(request.CaloriesPerHour);
        result.RequiresEquipment.Should().Be(request.RequiresEquipment);
        result.MuscleGroup.Should().Be(request.MuscleGroup);

        var dbExercise = await Context.Exercises.FirstOrDefaultAsync(x => x.Id == new ExerciseId(result.Id));
        dbExercise.Should().NotBeNull();
        dbExercise!.Name.Should().Be(request.Name);
        dbExercise.CaloriesPerHour.Should().Be(request.CaloriesPerHour);
        dbExercise.RequiresEquipment.Should().Be(request.RequiresEquipment);
        dbExercise.MuscleGroup.Should().Be(request.MuscleGroup);
    }

    [Fact]
    public async Task CreateExercise_WithDuplicateName_ReturnsConflict()
    {
        // Arrange
        var request = new CreateExerciseDto(
            Name: _firstExercise.Name,
            CaloriesPerHour: 500m,
            RequiresEquipment: false,
            MuscleGroup: MuscleGroup.Chest
        );

        // Act
        var response = await Client.PostAsJsonAsync("/exercises", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var exercisesWithName = await Context.Exercises
            .Where(x => x.Name == _firstExercise.Name)
            .ToListAsync();

        exercisesWithName.Should().HaveCount(1);
        exercisesWithName[0].Id.Should().Be(_firstExercise.Id);
    }

    [Fact]
    public async Task CreateExercise_WithInvalidData_ReturnsBadRequestAndDoesNotPersist()
    {
        // Arrange - empty name and negative calories
        var request = new CreateExerciseDto(
            Name: string.Empty,
            CaloriesPerHour: -100m,
            RequiresEquipment: false,
            MuscleGroup: MuscleGroup.Chest
        );

        // Act
        var response = await Client.PostAsJsonAsync("/exercises", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();

        var dbExercise = await Context.Exercises.FirstOrDefaultAsync(x => x.CaloriesPerHour == -100m);
        dbExercise.Should().BeNull();
    }

    [Fact]
    public async Task UpdateExercise_WithValidData_ReturnsOkAndUpdatesInDb()
    {
        // Arrange
        var request = new UpdateExerciseDto(
            Name: "Updated Push Up Name",
            CaloriesPerHour: 350m,
            RequiresEquipment: true,
            MuscleGroup: MuscleGroup.Chest
        );

        // Act
        var response = await Client.PutAsJsonAsync($"/exercises/{_firstExercise.Id.Value}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ExerciseDto>();
        result.Should().NotBeNull();
        result!.Name.Should().Be(request.Name);
        result.CaloriesPerHour.Should().Be(request.CaloriesPerHour);
        result.RequiresEquipment.Should().Be(request.RequiresEquipment);

        Context.ChangeTracker.Clear();
        var dbExercise = await Context.Exercises.FirstOrDefaultAsync(x => x.Id == _firstExercise.Id);
        dbExercise.Should().NotBeNull();
        dbExercise!.Name.Should().Be(request.Name);
        dbExercise.CaloriesPerHour.Should().Be(request.CaloriesPerHour);
        dbExercise.RequiresEquipment.Should().Be(request.RequiresEquipment);
    }

    [Fact]
    public async Task UpdateExercise_WithoutChangingUniqueName_ReturnsOk()
    {
        // Arrange
        var request = new UpdateExerciseDto(
            Name: _firstExercise.Name,
            CaloriesPerHour: 320m,
            RequiresEquipment: false,
            MuscleGroup: MuscleGroup.Chest
        );

        // Act
        var response = await Client.PutAsJsonAsync($"/exercises/{_firstExercise.Id.Value}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        Context.ChangeTracker.Clear();
        var dbExercise = await Context.Exercises.FirstOrDefaultAsync(x => x.Id == _firstExercise.Id);
        dbExercise.Should().NotBeNull();
        dbExercise!.CaloriesPerHour.Should().Be(320m);
    }

    [Fact]
    public async Task UpdateExercise_WithDuplicateNameFromAnotherExercise_ReturnsConflict()
    {
        // Arrange - attempting to update firstExercise with secondExercise's name
        var request = new UpdateExerciseDto(
            Name: _secondExercise.Name,
            CaloriesPerHour: 300m,
            RequiresEquipment: false,
            MuscleGroup: MuscleGroup.Chest
        );

        // Act
        var response = await Client.PutAsJsonAsync($"/exercises/{_firstExercise.Id.Value}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        Context.ChangeTracker.Clear();
        var dbExercise = await Context.Exercises.FirstOrDefaultAsync(x => x.Id == _firstExercise.Id);
        dbExercise.Should().NotBeNull();
        dbExercise!.Name.Should().Be(_firstExercise.Name);
    }

    [Fact]
    public async Task UpdateExercise_NonExistent_ReturnsNotFound()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();
        var request = new UpdateExerciseDto(
            Name: "Non Existent",
            CaloriesPerHour: 300m,
            RequiresEquipment: false,
            MuscleGroup: MuscleGroup.Chest
        );

        // Act
        var response = await Client.PutAsJsonAsync($"/exercises/{nonExistentId}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateExercise_WithInvalidData_ReturnsBadRequestAndDoesNotChangeDb()
    {
        // Arrange
        var request = new UpdateExerciseDto(
            Name: string.Empty,
            CaloriesPerHour: -50m,
            RequiresEquipment: false,
            MuscleGroup: MuscleGroup.Chest
        );

        // Act
        var response = await Client.PutAsJsonAsync($"/exercises/{_firstExercise.Id.Value}", request);

        // Assert
        response.IsSuccessStatusCode.Should().BeFalse();

        Context.ChangeTracker.Clear();
        var dbExercise = await Context.Exercises.FirstOrDefaultAsync(x => x.Id == _firstExercise.Id);
        dbExercise.Should().NotBeNull();
        dbExercise!.Name.Should().Be(_firstExercise.Name);
    }

    [Fact]
    public async Task GetExercises_ReturnsAllExercises()
    {
        // Act
        var response = await Client.GetAsync("/exercises");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<ExerciseDto>>();
        result.Should().NotBeNull();
        result!.Should().HaveCount(2);
        result.Select(x => x.Id).Should().Contain([_firstExercise.Id.Value, _secondExercise.Id.Value]);
    }

    [Fact]
    public async Task GetExercise_ExistingId_ReturnsOkWithExercise()
    {
        // Act
        var response = await Client.GetAsync($"/exercises/{_firstExercise.Id.Value}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<ExerciseDto>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(_firstExercise.Id.Value);
        result.Name.Should().Be(_firstExercise.Name);
    }

    [Fact]
    public async Task GetExercise_NonExistentId_ReturnsNotFound()
    {
        // Act
        var response = await Client.GetAsync($"/exercises/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
