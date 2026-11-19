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

public class ExercisesFilterTests : BaseIntegrationTest, IAsyncLifetime
{
    private Exercise _first = null!;
    private Exercise _second = null!;
    private Exercise _third = null!;
    private Exercise _fourth = null!;

    public ExercisesFilterTests(IntegrationTestWebFactory factory) : base(factory)
    {
    }

    public async Task InitializeAsync()
    {
        _first = ExercisesTestData.FirstTestExercise(); 
        _second = ExercisesTestData.SecondTestExercise(); 
        _third = ExercisesTestData.ThirdTestExercise();    
        _fourth = ExercisesTestData.FourthTestExercise();  

        await Context.Exercises.AddRangeAsync(_first, _second, _third, _fourth);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();

        _first = await Context.Exercises.FirstAsync(x => x.Id == _first.Id);
        _second = await Context.Exercises.FirstAsync(x => x.Id == _second.Id);
        _third = await Context.Exercises.FirstAsync(x => x.Id == _third.Id);
        _fourth = await Context.Exercises.FirstAsync(x => x.Id == _fourth.Id);
    }

    public async Task DisposeAsync()
    {
        Context.Exercises.RemoveRange(Context.Exercises);
        await Context.SaveChangesAsync();
        Context.ChangeTracker.Clear();
    }

    [Fact]
    public async Task Filter_ByName_ReturnsMatchingExercises()
    {
        var filter = new SearchExerciseDto(
            Name: "Up",
            CaloriesPerHourFrom: null,
            CaloriesPerHourTo: null,
            RequiresEquipment: null,
            MuscleGroup: null,
            CreatedAtFrom: null,
            CreatedAtTo: null
        );

        var response = await Client.PostAsJsonAsync("/exercises/filter", filter);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<ExerciseDto>>();
        result.Should().NotBeNull();
        result!.Select(x => x.Id).Should().BeEquivalentTo([_first.Id.Value, _second.Id.Value]);
    }

    [Fact]
    public async Task Filter_ByName_CaseInsensitive_ReturnsSameMatches()
    {
        var filter = new SearchExerciseDto(
            Name: "up",
            CaloriesPerHourFrom: null,
            CaloriesPerHourTo: null,
            RequiresEquipment: null,
            MuscleGroup: null,
            CreatedAtFrom: null,
            CreatedAtTo: null
        );

        var response = await Client.PostAsJsonAsync("/exercises/filter", filter);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<ExerciseDto>>();
        result.Should().NotBeNull();
        result!.Select(x => x.Id).Should().BeEquivalentTo([_first.Id.Value, _second.Id.Value]);
    }

    [Fact]
    public async Task Filter_ByCaloriesPerHourFrom_ReturnsMatchingExercises()
    {           
        var filter = new SearchExerciseDto(
            Name: null,
            CaloriesPerHourFrom: 400m,
            CaloriesPerHourTo: null,
            RequiresEquipment: null,
            MuscleGroup: null,
            CreatedAtFrom: null,
            CreatedAtTo: null
        );

        var response = await Client.PostAsJsonAsync("/exercises/filter", filter);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<ExerciseDto>>();
        result.Should().NotBeNull();
        result!.Select(x => x.Id).Should().BeEquivalentTo([_second.Id.Value, _third.Id.Value]);
    }

    [Fact]
    public async Task Filter_ByCaloriesPerHourTo_ReturnsMatchingExercises()
    {
        var filter = new SearchExerciseDto(
            Name: null,
            CaloriesPerHourFrom: null,
            CaloriesPerHourTo: 300m,
            RequiresEquipment: null,
            MuscleGroup: null,
            CreatedAtFrom: null,
            CreatedAtTo: null
        );

        var response = await Client.PostAsJsonAsync("/exercises/filter", filter);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<ExerciseDto>>();
        result.Should().NotBeNull();
        result!.Select(x => x.Id).Should().BeEquivalentTo([_first.Id.Value, _fourth.Id.Value]);
    }

    [Fact]
    public async Task Filter_ByCaloriesPerHour_BothBounds_ReturnsMatchingExercises()
    {
        var filter = new SearchExerciseDto(
            Name: null,
            CaloriesPerHourFrom: 300m,
            CaloriesPerHourTo: 400m,
            RequiresEquipment: null,
            MuscleGroup: null,
            CreatedAtFrom: null,
            CreatedAtTo: null
        );

        var response = await Client.PostAsJsonAsync("/exercises/filter", filter);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<ExerciseDto>>();
        result.Should().NotBeNull();
        result!.Select(x => x.Id).Should().BeEquivalentTo([_first.Id.Value, _second.Id.Value]);
    }

    [Fact]
    public async Task Filter_ByRequiresEquipment_ReturnsMatchingExercises()
    {
        var filter = new SearchExerciseDto(
            Name: null,
            CaloriesPerHourFrom: null,
            CaloriesPerHourTo: null,
            RequiresEquipment: false,
            MuscleGroup: null,
            CreatedAtFrom: null,
            CreatedAtTo: null
        );

        var response = await Client.PostAsJsonAsync("/exercises/filter", filter);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<ExerciseDto>>();
        result.Should().NotBeNull();
        result!.Select(x => x.Id).Should().BeEquivalentTo([_first.Id.Value]);
    }

    [Fact]
    public async Task Filter_ByMuscleGroup_ReturnsMatchingExercises()
    {
        var filter = new SearchExerciseDto(
            Name: null,
            CaloriesPerHourFrom: null,
            CaloriesPerHourTo: null,
            RequiresEquipment: null,
            MuscleGroup: MuscleGroup.Legs,
            CreatedAtFrom: null,
            CreatedAtTo: null
        );

        var response = await Client.PostAsJsonAsync("/exercises/filter", filter);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<ExerciseDto>>();
        result.Should().NotBeNull();
        result!.Select(x => x.Id).Should().BeEquivalentTo([_third.Id.Value]);
    }

    [Fact]
    public async Task Filter_ByCreatedAtFrom_ReturnsMatchingExercises()
    {
        var filter = new SearchExerciseDto(
            Name: null,
            CaloriesPerHourFrom: null,
            CaloriesPerHourTo: null,
            RequiresEquipment: null,
            MuscleGroup: null,
            CreatedAtFrom: _second.CreatedAt,
            CreatedAtTo: null
        );

        var response = await Client.PostAsJsonAsync("/exercises/filter", filter);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<ExerciseDto>>();
        result.Should().NotBeNull();
        result!.Should().Contain(x => x.Id == _second.Id.Value);
    }

    [Fact]
    public async Task Filter_ByCreatedAtTo_ReturnsMatchingExercises()
    {
        var filter = new SearchExerciseDto(
            Name: null,
            CaloriesPerHourFrom: null,
            CaloriesPerHourTo: null,
            RequiresEquipment: null,
            MuscleGroup: null,
            CreatedAtFrom: null,
            CreatedAtTo: _third.CreatedAt
        );

        var response = await Client.PostAsJsonAsync("/exercises/filter", filter);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<ExerciseDto>>();
        result.Should().NotBeNull();
        result!.Should().Contain(x => x.Id == _third.Id.Value);
    }

    [Fact]
    public async Task Filter_CombinationOfParameters_ReturnsOnlyFullyMatchingExercises()
    {
        var filter = new SearchExerciseDto(
            Name: null,
            CaloriesPerHourFrom: 350m,
            CaloriesPerHourTo: null,
            RequiresEquipment: true,
            MuscleGroup: MuscleGroup.Back,
            CreatedAtFrom: null,
            CreatedAtTo: null
        );

        var response = await Client.PostAsJsonAsync("/exercises/filter", filter);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<ExerciseDto>>();
        result.Should().NotBeNull();
        result!.Select(x => x.Id).Should().BeEquivalentTo([_second.Id.Value]);
    }

    [Fact]
    public async Task Filter_WithoutParameters_ReturnsAllExercises()
    {
        var filter = new SearchExerciseDto(
            Name: null,
            CaloriesPerHourFrom: null,
            CaloriesPerHourTo: null,
            RequiresEquipment: null,
            MuscleGroup: null,
            CreatedAtFrom: null,
            CreatedAtTo: null
        );

        var response = await Client.PostAsJsonAsync("/exercises/filter", filter);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<ExerciseDto>>();
        result.Should().NotBeNull();
        result!.Select(x => x.Id).Should().BeEquivalentTo([
            _first.Id.Value,
            _second.Id.Value,
            _third.Id.Value,
            _fourth.Id.Value
        ]);
    }

    [Fact]
    public async Task Filter_NoMatchingRecords_ReturnsOkWithEmptyList()
    {
        var filter = new SearchExerciseDto(
            Name: "NonExistentExerciseName123",
            CaloriesPerHourFrom: null,
            CaloriesPerHourTo: null,
            RequiresEquipment: null,
            MuscleGroup: null,
            CreatedAtFrom: null,
            CreatedAtTo: null
        );

        var response = await Client.PostAsJsonAsync("/exercises/filter", filter);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IReadOnlyList<ExerciseDto>>();
        result.Should().NotBeNull();
        result!.Should().BeEmpty();
    }
}
