using Application.Exceptions;
using Application.UseCases.Problems;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Application.DTOs.ProblemDtos;

namespace WebApi.Endpoints
{
    public static class ProblemsEndpoints
    {
        public static void MapProblemsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/problems")
                .WithTags("Problems")
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrador,Director de Operaciones,Asistente Operativo" });

            group.MapGet("/{id:int}", async Task<Results<Ok<ProblemResponse>, NotFound<object>, ProblemHttpResult>> (
                int id,
                GetProblemQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var problem = await queriesUseCase.GetByIdAsync(id);
                    return TypedResults.Ok(problem);
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
            .WithName("GetProblemById")
            .WithSummary("Get a specific problem report by its identifier");

            group.MapGet("/", async Task<Results<Ok<IEnumerable<ProblemResponse>>, ProblemHttpResult>> (
                GetProblemQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var problems = await queriesUseCase.GetAllAsync();
                    return TypedResults.Ok(problems);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetAllProblems")
            .WithSummary("Get all registered problem reports");

            group.MapGet("/municipality/{municipalityId:int}", async Task<Results<Ok<IEnumerable<ProblemResponse>>, ProblemHttpResult>> (
                int municipalityId,
                GetProblemQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var problems = await queriesUseCase.GetByMunicipalityAsync(municipalityId);
                    return TypedResults.Ok(problems);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetProblemsByMunicipality")
            .WithSummary("Get all problem reports belonging to a specific municipality");

            group.MapGet("/municipality/{municipalityId:int}/status/{status}", async Task<Results<Ok<IEnumerable<ProblemResponse>>, BadRequest<object>, ProblemHttpResult>> (
                int municipalityId,
                string status,
                GetProblemQueriesUseCase queriesUseCase) =>
            {
                if (!Enum.TryParse<ProblemStatus>(status, true, out var parsedStatus))
                {
                    return TypedResults.BadRequest((object)new { error = $"El estado proporcionado no es válido. Los valores permitidos son: {string.Join(", ", Enum.GetNames<ProblemStatus>())}." });
                }

                try
                {
                    var problems = await queriesUseCase.GetByMunicipalityAndStatusAsync(municipalityId, parsedStatus);
                    return TypedResults.Ok(problems);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetProblemsByMunicipalityAndStatus")
            .WithSummary("Get problem reports belonging to a municipality filtered by status");

            group.MapGet("/severity/{severity}/status/{status}", async Task<Results<Ok<IEnumerable<ProblemResponse>>, BadRequest<object>, ProblemHttpResult>> (
                string severity,
                string status,
                GetProblemQueriesUseCase queriesUseCase) =>
            {
                if (!Enum.TryParse<ProblemSeverity>(severity, true, out var parsedSeverity))
                {
                    return TypedResults.BadRequest((object)new { error = $"La severidad proporcionada no es válido. Los valores permitidos son: {string.Join(", ", Enum.GetNames<ProblemSeverity>())}." });
                }

                if (!Enum.TryParse<ProblemStatus>(status, true, out var parsedStatus))
                {
                    return TypedResults.BadRequest((object)new { error = $"El estado proporcionado no es válido. Los valores permitidos son: {string.Join(", ", Enum.GetNames<ProblemStatus>())}." });
                }

                try
                {
                    var problems = await queriesUseCase.GetBySeverityAndStatusAsync(parsedSeverity, parsedStatus);
                    return TypedResults.Ok(problems);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetProblemsBySeverityAndStatus")
            .WithSummary("Get problem reports filtered by severity and operational status globally");

            group.MapGet("/range", async Task<Results<Ok<IEnumerable<ProblemResponse>>, BadRequest<object>, ProblemHttpResult>> (
                [FromQuery] DateTime startDate,
                [FromQuery] DateTime endDate,
                GetProblemQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var problems = await queriesUseCase.GetByRegistrationDateRangeAsync(startDate, endDate);
                    return TypedResults.Ok(problems);
                }
                catch (ApplicationValidationException ex)
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
            .WithName("GetProblemsByDateRange")
            .WithSummary("Get problem reports registered within a specific date range");

            group.MapPost("/", async Task<Results<Created<ProblemResponse>, BadRequest<object>, ProblemHttpResult>> (
                [FromBody] ProblemCreateRequest request,
                CreateProblemUseCase useCase) =>
            {
                try
                {
                    var createdProblem = await useCase.ExecuteAsync(request);
                    return TypedResults.Created($"/api/problems/{createdProblem.Id}", createdProblem);
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
            .WithName("CreateProblem")
            .WithSummary("Register a new citizen problem report linked to a specific municipality");

            group.MapPut("/{id:int}", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                [FromBody] ProblemUpdateReportRequest request,
                UpdateProblemReportUseCase useCase) =>
            {
                if (id != request.Id)
                {
                    return TypedResults.BadRequest((object)new { error = "El ID del parámetro de ruta no coincide con el ID del cuerpo de la solicitud." });
                }

                try
                {
                    await useCase.ExecuteAsync(request);
                    return TypedResults.Ok((object)new { message = "El reporte del problema ha sido actualizado correctamente." });
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
            .WithName("UpdateProblemReport")
            .WithSummary("Update details of an existing problem report if it remains in reported status");

            group.MapPatch("/{id:int}/process", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                ChangeProblemStatusUseCase useCase) =>
            {
                try
                {
                    await useCase.MarkAsInProcessAsync(id);
                    return TypedResults.Ok((object)new { message = "El reporte de problema ha cambiado a estado en proceso." });
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
            .WithName("MarkProblemAsInProcess")
            .WithSummary("Transition the problem status to in-process mode");

            group.MapPatch("/{id:int}/resolve", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                ChangeProblemStatusUseCase useCase) =>
            {
                try
                {
                    await useCase.ResolveAsync(id);
                    return TypedResults.Ok((object)new { message = "El problema ha sido marcado como solucionado exitosamente." });
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
            .WithName("ResolveProblem")
            .WithSummary("Transition the problem status to resolved");

            group.MapPatch("/{id:int}/reject", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                ChangeProblemStatusUseCase useCase) =>
            {
                try
                {
                    await useCase.RejectAsync(id);
                    return TypedResults.Ok((object)new { message = "El reporte de problema ha sido rechazado." });
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
            .WithName("RejectProblem")
            .WithSummary("Transition the problem status to rejected");
        }
    }
}