using Application.Exceptions;
using Application.UseCases.Notices;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Application.DTOs.NoticeDtos;

namespace WebApi.Endpoints
{
    public static class NoticesEndpoints
    {
        public static void MapNoticesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/notices")
                .WithTags("Notices")
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrador,Director de Operaciones,RRHH,Asistente Operativo" });

            group.MapGet("/{id:int}", async Task<Results<Ok<NoticeResponse>, NotFound<object>, ProblemHttpResult>> (
                int id,
                GetNoticeQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var notice = await queriesUseCase.GetByIdAsync(id);
                    return TypedResults.Ok(notice);
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
            .WithName("GetNoticeById")
            .WithSummary("Get a specific notice by its identifier");

            group.MapGet("/", async Task<Results<Ok<IEnumerable<NoticeResponse>>, ProblemHttpResult>> (
                GetNoticeQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var notices = await queriesUseCase.GetAllAsync();
                    return TypedResults.Ok(notices);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetAllNotices")
            .WithSummary("Get all registered notices");

            group.MapGet("/municipality/{municipalityId:int}/active", async Task<Results<Ok<IEnumerable<NoticeResponse>>, ProblemHttpResult>> (
                int municipalityId,
                GetNoticeQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var notices = await queriesUseCase.GetActiveByMunicipalityAsync(municipalityId);
                    return TypedResults.Ok(notices);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetActiveNoticesByMunicipality")
            .WithSummary("Get active (non-archived) notices for a specific municipality");

            group.MapGet("/category/{category}", async Task<Results<Ok<IEnumerable<NoticeResponse>>, BadRequest<object>, ProblemHttpResult>> (
                string category,
                GetNoticeQueriesUseCase queriesUseCase) =>
            {
                if (!Enum.TryParse<NoticeCategory>(category, true, out var parsedCategory))
                {
                    return TypedResults.BadRequest((object)new { error = $"La categoría proporcionada no es válida. Los valores permitidos son: {string.Join(", ", Enum.GetNames<NoticeCategory>())}." });
                }

                try
                {
                    var notices = await queriesUseCase.GetByCategoryAsync(parsedCategory);
                    return TypedResults.Ok(notices);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetNoticesByCategory")
            .WithSummary("Get all notices matching a specific category");

            group.MapGet("/category/{category}/active", async Task<Results<Ok<IEnumerable<NoticeResponse>>, BadRequest<object>, ProblemHttpResult>> (
                string category,
                GetNoticeQueriesUseCase queriesUseCase) =>
            {
                if (!Enum.TryParse<NoticeCategory>(category, true, out var parsedCategory))
                {
                    return TypedResults.BadRequest((object)new { error = $"La categoría proporcionada no es válida. Los valores permitidos son: {string.Join(", ", Enum.GetNames<NoticeCategory>())}." });
                }

                try
                {
                    var notices = await queriesUseCase.GetActiveByCategoryAsync(parsedCategory);
                    return TypedResults.Ok(notices);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetActiveNoticesByCategory")
            .WithSummary("Get active notices matching a specific category");

            group.MapGet("/range", async Task<Results<Ok<IEnumerable<NoticeResponse>>, BadRequest<object>, ProblemHttpResult>> (
                [FromQuery] DateTime startDate,
                [FromQuery] DateTime endDate,
                GetNoticeQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var notices = await queriesUseCase.GetByRegistrationDateRangeAsync(startDate, endDate);
                    return TypedResults.Ok(notices);
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
            .WithName("GetNoticesByDateRange")
            .WithSummary("Get notices created within a specific registration date range");

            group.MapPost("/", async Task<Results<Created<NoticeResponse>, BadRequest<object>, ProblemHttpResult>> (
                [FromBody] NoticeCreateRequest request,
                CreateNoticeUseCase useCase) =>
            {
                try
                {
                    var createdNotice = await useCase.ExecuteAsync(request);
                    return TypedResults.Created($"/api/notices/{createdNotice.Id}", createdNotice);
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
            .WithName("CreateNotice")
            .WithSummary("Register a new notice linked to a specific municipality");

            group.MapPut("/{id:int}", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                [FromBody] NoticeUpdateContentRequest request,
                UpdateNoticeContentUseCase useCase) =>
            {
                if (id != request.Id)
                {
                    return TypedResults.BadRequest((object)new { error = "El ID del parámetro de ruta no coincide con el ID del cuerpo de la solicitud." });
                }

                try
                {
                    await useCase.ExecuteAsync(request);
                    return TypedResults.Ok((object)new { message = "El contenido del aviso ha sido actualizado correctamente." });
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
            .WithName("UpdateNoticeContent")
            .WithSummary("Update the main content details of an existing notice");

            group.MapPatch("/{id:int}/archive", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                ArchiveNoticeUseCase useCase) =>
            {
                try
                {
                    await useCase.ExecuteAsync(id);
                    return TypedResults.Ok((object)new { message = "El aviso ha sido archivado exitosamente." });
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
            .WithName("ArchiveNotice")
            .WithSummary("Change the status of a notice to archived");
        }
    }
}