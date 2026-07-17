using Application.Exceptions;
using Domain.Exceptions;
using Application.UseCases.Positions;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Application.DTOs.PositionDtos;

namespace WebApi.Endpoints
{
    public static class PositionsEndpoints
    {
        public static void MapPositionsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/positions")
                .WithTags("Positions");

            group.MapGet("/{id:int}", async Task<Results<Ok<PositionResponse>, NotFound<object>, ProblemHttpResult>> (
                int id,
                GetPositionByIdUseCase useCase) =>
            {
                try
                {
                    var position = await useCase.ExecuteAsync(id);
                    return TypedResults.Ok(position);
                }
                catch (ApplicationValidationException ex)
                {
                    return TypedResults.NotFound((object)new { error = ex.Message });
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetPositionById")
            .WithSummary("Get a specific position by its unique identifier");

            group.MapGet("/", async Task<Results<Ok<IEnumerable<PositionResponse>>, ProblemHttpResult>> (
                [FromQuery] string? search,
                GetAllPositionsOrderedByNameUseCase getAllUseCase,
                SearchPositionsByNameUseCase searchUseCase) =>
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(search))
                    {
                        var filteredPositions = await searchUseCase.ExecuteAsync(search);
                        return TypedResults.Ok(filteredPositions);
                    }

                    var allPositions = await getAllUseCase.ExecuteAsync();
                    return TypedResults.Ok(allPositions);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetPositions")
            .WithSummary("Get all positions or search them filtering by name");

            group.MapPost("/", async Task<Results<Created<PositionResponse>, BadRequest<object>, ProblemHttpResult>> (
                [FromBody] PositionCreateRequest request,
                CreatePositionUseCase useCase) =>
            {
                try
                {
                    var createdPosition = await useCase.ExecuteAsync(request);
                    return TypedResults.Created($"/api/positions/{createdPosition.Id}", createdPosition);
                }
                catch (Exception ex) when (ex is ApplicationValidationException || ex is DomainException)
                {
                    return TypedResults.BadRequest((object)new { error = ex.Message });
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("CreatePosition")
            .WithSummary("Create a new job position record");

            group.MapPut("/{id:int}", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                [FromBody] PositionUpdateRequest request,
                UpdatePositionUseCase useCase) =>
            {
                if (id != request.Id)
                {
                    return TypedResults.BadRequest((object)new { error = "Route ID parameter does not match the request body ID." });
                }

                try
                {
                    await useCase.ExecuteAsync(request);
                    return TypedResults.Ok((object)new { message = "Position successfully updated." });
                }
                catch (ApplicationValidationException ex)
                {
                    if (ex.Message.Contains("No se encontró"))
                    {
                        return TypedResults.NotFound((object)new { error = ex.Message });
                    }
                    return TypedResults.BadRequest((object)new { error = ex.Message });
                }
                catch (DomainException ex)
                {
                    return TypedResults.BadRequest((object)new { error = ex.Message });
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("UpdatePosition")
            .WithSummary("Update an existing position details");

            group.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound<object>, ProblemHttpResult>> (
                int id,
                DeletePositionUseCase useCase) =>
            {
                try
                {
                    await useCase.ExecuteAsync(id);
                    return TypedResults.NoContent();
                }
                catch (ApplicationValidationException ex)
                {
                    return TypedResults.NotFound((object)new { error = ex.Message });
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("DeletePosition")
            .WithSummary("Remove an existing position from the system");
        }
    }
}