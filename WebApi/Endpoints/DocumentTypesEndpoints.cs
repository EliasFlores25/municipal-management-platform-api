using Application.Exceptions;
using Domain.Exceptions;
using Application.UseCases.DocumentTypes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Application.DTOs.DocumentTypeDtos;

namespace WebApi.Endpoints
{
    public static class DocumentTypesEndpoints
    {
        public static void MapDocumentTypesEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/document-types")
                .WithTags("DocumentTypes");

            group.MapGet("/{id:int}", async Task<Results<Ok<DocumentTypeResponse>, NotFound<object>, ProblemHttpResult>> (
                int id,
                GetDocumentTypeByIdUseCase useCase) =>
            {
                try
                {
                    var documentType = await useCase.ExecuteAsync(id);
                    return TypedResults.Ok(documentType);
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
            .WithName("GetDocumentTypeById")
            .WithSummary("Get a specific document type by its unique identifier");

            group.MapPost("/", async Task<Results<Created<DocumentTypeResponse>, BadRequest<object>, ProblemHttpResult>> (
                [FromBody] DocumentTypeCreateRequest request,
                CreateDocumentTypeUseCase useCase) =>
            {
                try
                {
                    var createdType = await useCase.ExecuteAsync(request);
                    return TypedResults.Created($"/api/document-types/{createdType.Id}", createdType);
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
            .WithName("CreateDocumentType")
            .WithSummary("Create a new document type record");

            group.MapGet("/", async Task<Results<Ok<IEnumerable<DocumentTypeResponse>>, ProblemHttpResult>> (
                GetAllDocumentTypesOrderedByNameUseCase useCase) =>
            {
                try
                {
                    var documentTypes = await useCase.ExecuteAsync();
                    return TypedResults.Ok(documentTypes);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetAllDocumentTypes")
            .WithSummary("Get all registered document types ordered alphabetically by name");

            group.MapPut("/{id:int}", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                [FromBody] DocumentTypeUpdateRequest request,
                UpdateDocumentTypeUseCase useCase) =>
            {
                if (id != request.Id)
                {
                    return TypedResults.BadRequest((object)new { error = "Route ID parameter does not match the request body ID." });
                }

                try
                {
                    await useCase.ExecuteAsync(request);
                    return TypedResults.Ok((object)new { message = "Document type successfully updated." });
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
            .WithName("UpdateDocumentType")
            .WithSummary("Update an existing document type details");

            group.MapDelete("/{id:int}", async Task<Results<NoContent, NotFound<object>, ProblemHttpResult>> (
                int id,
                DeleteDocumentTypeUseCase useCase) =>
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
            .WithName("DeleteDocumentType")
            .WithSummary("Remove an existing document type from the system");
        }
    }
}