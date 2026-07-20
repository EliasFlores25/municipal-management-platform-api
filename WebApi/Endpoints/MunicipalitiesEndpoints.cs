using Application.Exceptions;
using Application.UseCases.Municipalities;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Application.DTOs.MunicipalityDtos;

namespace WebApi.Endpoints
{
    public static class MunicipalitiesEndpoints
    {
        public static void MapMunicipalitiesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/municipalities")
                .WithTags("Municipalities")
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrador,Director de Operaciones" });

            group.MapGet("/{id:int}", async Task<Results<Ok<MunicipalityResponse>, NotFound<object>, ProblemHttpResult>> (
                int id,
                GetMunicipalityByIdUseCase useCase) =>
            {
                try
                {
                    var municipality = await useCase.ExecuteAsync(id);
                    return TypedResults.Ok(municipality);
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
            .WithName("GetMunicipalityById")
            .WithSummary("Get a specific municipality by its unique identifier");

            group.MapPost("/", async Task<Results<Created<MunicipalityResponse>, BadRequest<object>, ProblemHttpResult>> (
                [FromBody] MunicipalityCreateRequest request,
                CreateMunicipalityUseCase useCase) =>
            {
                try
                {
                    var createdMunicipality = await useCase.ExecuteAsync(request);
                    return TypedResults.Created($"/api/municipalities/{createdMunicipality.Id}", createdMunicipality);
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
            .WithName("CreateMunicipality")
            .WithSummary("Create a new municipality record");

            group.MapGet("/", async Task<Results<Ok<IEnumerable<MunicipalityResponse>>, ProblemHttpResult>> (
                GetAllMunicipalitiesOrderedByNameUseCase useCase) =>
            {
                try
                {
                    var municipalities = await useCase.ExecuteAsync();
                    return TypedResults.Ok(municipalities);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetAllMunicipalities")
            .WithSummary("Get all registered municipalities ordered alphabetically by name");

            group.MapPut("/{id:int}", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                [FromBody] MunicipalityUpdateRequest request,
                UpdateMunicipalityUseCase useCase) =>
            {
                if (id != request.Id)
                {
                    return TypedResults.BadRequest((object)new { error = "Route ID parameter does not match the request body ID." });
                }

                try
                {
                    await useCase.ExecuteAsync(request);
                    return TypedResults.Ok((object)new { message = "Municipality successfully updated." });
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
            .WithName("UpdateMunicipality")
            .WithSummary("Update an existing municipality details");

            group.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound<object>, ProblemHttpResult>> (
                int id,
                DeleteMunicipalityUseCase useCase) =>
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
            .WithName("DeleteMunicipality")
            .WithSummary("Remove an existing municipality from the system");
        }
    }
}