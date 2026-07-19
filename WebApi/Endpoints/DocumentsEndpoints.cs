using Application.Exceptions;
using Domain.Exceptions;
using Domain.Enums;
using Application.UseCases.Documents;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Application.DTOs.DocumentDtos;

namespace WebApi.Endpoints
{
    public static class DocumentsEndpoints
    {
        public static void MapDocumentsEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/documents")
                .WithTags("Documents");

            group.MapGet("/", async Task<Results<Ok<IEnumerable<DocumentResponse>>, ProblemHttpResult>> (
                GetDocumentQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var documents = await queriesUseCase.GetAllAsync();
                    return TypedResults.Ok(documents);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetAllDocuments")
            .WithSummary("Retrieve all registered legal or administrative documents");

            group.MapGet("/{id:int}", async Task<Results<Ok<DocumentResponse>, NotFound<object>, ProblemHttpResult>> (
                int id,
                GetDocumentQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var document = await queriesUseCase.GetByIdAsync(id);
                    return TypedResults.Ok(document);
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
            .WithName("GetDocumentById")
            .WithSummary("Get an individual document record using its unique database identifier");

            group.MapGet("/number/{documentNumber}", async Task<Results<Ok<DocumentResponse>, NotFound<object>, ProblemHttpResult>> (
                string documentNumber,
                GetDocumentQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var document = await queriesUseCase.GetByDocumentNumberAsync(documentNumber);
                    return TypedResults.Ok(document);
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
            .WithName("GetDocumentByNumber")
            .WithSummary("Find a unique document searching by its official business or government serial number");

            group.MapGet("/municipality/{municipalityId:int}/status/{status}", async Task<Results<Ok<IEnumerable<DocumentResponse>>, ProblemHttpResult>> (
                int municipalityId,
                DocumentStatus status,
                GetDocumentQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var documents = await queriesUseCase.GetByMunicipalityAndStatusAsync(municipalityId, status);
                    return TypedResults.Ok(documents);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetDocumentsByMunicipalityAndStatus")
            .WithSummary("Filter documents by an administrative regional assignment and their validation state");

            group.MapGet("/type/{documentTypeId:int}", async Task<Results<Ok<IEnumerable<DocumentResponse>>, ProblemHttpResult>> (
                int documentTypeId,
                GetDocumentQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var documents = await queriesUseCase.GetByDocumentTypeAsync(documentTypeId);
                    return TypedResults.Ok(documents);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetDocumentsByType")
            .WithSummary("Group and retrieve document records matching a specific category layout");

            group.MapGet("/range", async Task<Results<Ok<IEnumerable<DocumentResponse>>, BadRequest<object>, ProblemHttpResult>> (
                [FromQuery] DateTime startDate,
                [FromQuery] DateTime endDate,
                GetDocumentQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var documents = await queriesUseCase.GetByEmissionDateRangeAsync(startDate, endDate);
                    return TypedResults.Ok(documents);
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
            .WithName("GetDocumentsByDateRange")
            .WithSummary("Query a transactional block of documents based on custom temporal boundaries");

            group.MapPost("/", async Task<Results<Created<DocumentResponse>, BadRequest<object>, ProblemHttpResult>> (
                [FromBody] DocumentCreateRequest request,
                CreateDocumentUseCase useCase) =>
            {
                try
                {
                    var createdDocument = await useCase.ExecuteAsync(request);
                    return TypedResults.Created($"/api/documents/{createdDocument.Id}", createdDocument);
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
            .WithName("CreateDocument")
            .WithSummary("Issue and register a completely unique official document profile");

            group.MapPut("/{id:int}", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                [FromBody] DocumentUpdateDetailsRequest request,
                UpdateDocumentDetailsUseCase useCase) =>
            {
                if (id != request.Id)
                {
                    return TypedResults.BadRequest((object)new { error = "El ID del parámetro de ruta no coincide con el ID del cuerpo de la solicitud." });
                }

                try
                {
                    await useCase.ExecuteAsync(request);
                    return TypedResults.Ok((object)new { message = "Los detalles del documento han sido modificados correctamente." });
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
            .WithName("UpdateDocumentDetails")
            .WithSummary("Modify dynamic text parameters including proprietary details and reference metadata");

            group.MapPatch("/{id:int}/anular", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                AnularDocumentUseCase useCase) =>
            {
                try
                {
                    await useCase.ExecuteAsync(id);
                    return TypedResults.Ok((object)new { message = "El documento ha sido anulado de forma irreversible dentro de la base transaccional." });
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
            .WithName("AnularDocument")
            .WithSummary("Invalidate a specific active document breaking its enforcement state");
        }
    }
}