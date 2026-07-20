using Application.Exceptions;
using Application.UseCases.Projects;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Application.DTOs.ProjectDtos;

namespace WebApi.Endpoints
{
    public static class ProjectsEndpoints
    {
        public static void MapProjectsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/projects")
                .WithTags("Projects")
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrador,Director de Operaciones,Asistente Operativo" });

            group.MapGet("/{id:int}", async Task<Results<Ok<ProjectResponse>, NotFound<object>, ProblemHttpResult>> (
                int id,
                GetProjectQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var project = await queriesUseCase.GetByIdAsync(id);
                    return TypedResults.Ok(project);
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
            .WithName("GetProjectById")
            .WithSummary("Get a specific project by its identifier");

            group.MapGet("/", async Task<Results<Ok<IEnumerable<ProjectResponse>>, ProblemHttpResult>> (
                GetProjectQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var projects = await queriesUseCase.GetAllOrderedByNameAsync();
                    return TypedResults.Ok(projects);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetAllProjects")
            .WithSummary("Get all projects ordered alphabetically by name");

            group.MapGet("/municipality/{municipalityId:int}", async Task<Results<Ok<IEnumerable<ProjectResponse>>, ProblemHttpResult>> (
                int municipalityId,
                GetProjectQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var projects = await queriesUseCase.GetByMunicipalityAsync(municipalityId);
                    return TypedResults.Ok(projects);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetProjectsByMunicipality")
            .WithSummary("Get all projects belonging to a specific municipality");

            group.MapGet("/municipality/{municipalityId:int}/status/{status}", async Task<Results<Ok<IEnumerable<ProjectResponse>>, BadRequest<object>, ProblemHttpResult>> (
                int municipalityId,
                string status,
                GetProjectQueriesUseCase queriesUseCase) =>
            {
                if (!Enum.TryParse<ProjectStatus>(status, true, out var parsedStatus))
                {
                    return TypedResults.BadRequest((object)new { error = $"El estado proporcionado no es válido. Los valores permitidos son: {string.Join(", ", Enum.GetNames<ProjectStatus>())}." });
                }

                try
                {
                    var projects = await queriesUseCase.GetByMunicipalityAndStatusAsync(municipalityId, parsedStatus);
                    return TypedResults.Ok(projects);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetProjectsByMunicipalityAndStatus")
            .WithSummary("Get projects belonging to a municipality filtered by status");

            group.MapGet("/high-budget", async Task<Results<Ok<IEnumerable<ProjectResponse>>, ProblemHttpResult>> (
                [FromQuery] decimal minBudget,
                GetProjectQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var projects = await queriesUseCase.GetProjectsWithHighBudgetAsync(minBudget);
                    return TypedResults.Ok(projects);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetHighBudgetProjects")
            .WithSummary("Get all projects whose budget meets or exceeds the minimum specified limit");

            group.MapGet("/overdue", async Task<Results<Ok<IEnumerable<ProjectResponse>>, ProblemHttpResult>> (
                [FromQuery] DateTime? referenceDate,
                GetProjectQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var projects = await queriesUseCase.GetOverdueProjectsAsync(referenceDate);
                    return TypedResults.Ok(projects);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetOverdueProjects")
            .WithSummary("Get all projects that have passed their deadline relative to a reference date");

            group.MapPost("/", async Task<Results<Created<ProjectResponse>, BadRequest<object>, ProblemHttpResult>> (
                [FromBody] ProjectCreateRequest request,
                CreateProjectUseCase useCase) =>
            {
                try
                {
                    var createdProject = await useCase.ExecuteAsync(request);
                    return TypedResults.Created($"/api/projects/{createdProject.Id}", createdProject);
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
            .WithName("CreateProject")
            .WithSummary("Register a new project into planning status linked to a specific municipality");

            group.MapPut("/{id:int}", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                [FromBody] ProjectUpdatePlanningRequest request,
                UpdateProjectPlanningUseCase useCase) =>
            {
                if (id != request.Id)
                {
                    return TypedResults.BadRequest((object)new { error = "El ID del parámetro de ruta no coincide con el ID del cuerpo de la solicitud." });
                }

                try
                {
                    await useCase.ExecuteAsync(request);
                    return TypedResults.Ok((object)new { message = "Planificación del proyecto actualizada correctamente." });
                }
                catch (ApplicationValidationException ex)
                {
                    return TypedResults.NotFound((object)new { error = ex.Message });
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
            .WithName("UpdateProjectPlanning")
            .WithSummary("Update the basic planning details of an existing project");

            group.MapPatch("/{id:int}/start", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                ChangeProjectStatusUseCase useCase) =>
            {
                try
                {
                    await useCase.StartExecutionAsync(id);
                    return TypedResults.Ok((object)new { message = "El proyecto ha iniciado su fase de ejecución correctamente." });
                }
                catch (ApplicationValidationException ex)
                {
                    return TypedResults.NotFound((object)new { error = ex.Message });
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
            .WithName("StartProjectExecution")
            .WithSummary("Transition the project state to execution mode");

            group.MapPatch("/{id:int}/complete", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                ChangeProjectStatusUseCase useCase) =>
            {
                try
                {
                    await useCase.CompleteProjectAsync(id);
                    return TypedResults.Ok((object)new { message = "El proyecto se ha completado y finalizado correctamente." });
                }
                catch (ApplicationValidationException ex)
                {
                    return TypedResults.NotFound((object)new { error = ex.Message });
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
            .WithName("CompleteProject")
            .WithSummary("Transition the project state to completed status");

            group.MapPatch("/{id:int}/cancel", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                ChangeProjectStatusUseCase useCase) =>
            {
                try
                {
                    await useCase.CancelProjectAsync(id);
                    return TypedResults.Ok((object)new { message = "El proyecto ha sido cancelado correctamente." });
                }
                catch (ApplicationValidationException ex)
                {
                    return TypedResults.NotFound((object)new { error = ex.Message });
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
            .WithName("CancelProject")
            .WithSummary("Transition the project state to canceled status");
        }
    }
}