using Application.Exceptions;
using Application.UseCases.Inventories;
using Domain.Enums;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Application.DTOs.InventoryDtos;

namespace WebApi.Endpoints
{
    public static class InventoryEndpoints
    {
        public static void MapInventoryEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/api/inventories")
                .WithTags("Inventory")
                .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrador,Director de Operaciones,Asistente Operativo" });

            group.MapGet("/{id:int}", async Task<Results<Ok<InventoryResponse>, NotFound<object>, ProblemHttpResult>> (
                int id,
                GetInventoryQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var item = await queriesUseCase.GetByIdAsync(id);
                    return TypedResults.Ok(item);
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
            .WithName("GetInventoryItemById")
            .WithSummary("Get a specific inventory item by its identifier");

            group.MapGet("/", async Task<Results<Ok<IEnumerable<InventoryResponse>>, ProblemHttpResult>> (
                GetInventoryQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var items = await queriesUseCase.GetAllOrderedAsync();
                    return TypedResults.Ok(items);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetAllInventoryItems")
            .WithSummary("Get all inventory items ordered alphabetically by item name");

            group.MapGet("/municipality/{municipalityId:int}", async Task<Results<Ok<IEnumerable<InventoryResponse>>, ProblemHttpResult>> (
                int municipalityId,
                GetInventoryQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var items = await queriesUseCase.GetByMunicipalityAsync(municipalityId);
                    return TypedResults.Ok(items);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetInventoryByMunicipality")
            .WithSummary("Get all inventory items belonging to a specific municipality");

            group.MapGet("/status/{status}", async Task<Results<Ok<IEnumerable<InventoryResponse>>, BadRequest<object>, ProblemHttpResult>> (
                string status,
                GetInventoryQueriesUseCase queriesUseCase) =>
            {
                if (!Enum.TryParse<InventoryStatus>(status, true, out var parsedStatus))
                {
                    return TypedResults.BadRequest((object)new { error = $"El estado proporcionado no es válido. Los valores permitidos son: {string.Join(", ", Enum.GetNames<InventoryStatus>())}" });
                }

                try
                {
                    var items = await queriesUseCase.GetByStatusAsync(parsedStatus);
                    return TypedResults.Ok(items);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetInventoryByStatus")
            .WithSummary("Get all inventory items filtered by their active status");

            group.MapGet("/low-stock", async Task<Results<Ok<IEnumerable<InventoryResponse>>, ProblemHttpResult>> (
                [FromQuery] int threshold,
                GetInventoryQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var items = await queriesUseCase.GetLowStockAsync(threshold);
                    return TypedResults.Ok(items);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetLowStockInventory")
            .WithSummary("Get all items whose quantity falls below the specified threshold");

            group.MapPost("/", async Task<Results<Created<InventoryResponse>, BadRequest<object>, ProblemHttpResult>> (
                [FromBody] InventoryCreateRequest request,
                CreateInventoryUseCase useCase) =>
            {
                try
                {
                    var createdItem = await useCase.ExecuteAsync(request);
                    return TypedResults.Created($"/api/inventories/{createdItem.Id}", createdItem);
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
            .WithName("CreateInventoryItem")
            .WithSummary("Create a new inventory item mapping it to a specific municipality");

            group.MapPut("/{id:int}", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                [FromBody] InventoryUpdateInfoRequest request,
                UpdateInventoryInfoUseCase useCase) =>
            {
                if (id != request.Id)
                {
                    return TypedResults.BadRequest((object)new { error = "El ID del parámetro de ruta no coincide con el ID del cuerpo de la solicitud." });
                }

                try
                {
                    await useCase.ExecuteAsync(request);
                    return TypedResults.Ok((object)new { message = "Información básica del ítem de inventario actualizada correctamente." });
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
            .WithName("UpdateInventoryInfo")
            .WithSummary("Update description and image of an existing inventory item");

            group.MapPatch("/{id:int}/stock/increase", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                [FromBody] InventoryStockAdjustmentRequest request,
                IncreaseInventoryStockUseCase useCase) =>
            {
                if (id != request.Id)
                {
                    return TypedResults.BadRequest((object)new { error = "El ID del parámetro de ruta no coincide con el ID del cuerpo de la solicitud." });
                }

                try
                {
                    await useCase.ExecuteAsync(request);
                    return TypedResults.Ok((object)new { message = "El stock del inventario se incrementó correctamente." });
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
            .WithName("IncreaseInventoryStock")
            .WithSummary("Increase the available stock for an inventory item");

            group.MapPatch("/{id:int}/stock/decrease", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                [FromBody] InventoryStockAdjustmentRequest request,
                DecreaseInventoryStockUseCase useCase) =>
            {
                if (id != request.Id)
                {
                    return TypedResults.BadRequest((object)new { error = "El ID del parámetro de ruta no coincide con el ID del cuerpo de la solicitud." });
                }

                try
                {
                    await useCase.ExecuteAsync(request);
                    return TypedResults.Ok((object)new { message = "El stock del inventario se redujo correctamente." });
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
            .WithName("DecreaseInventoryStock")
            .WithSummary("Decrease the available stock for an inventory item");

            group.MapPatch("/{id:int}/baja", async Task<Results<Ok<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                FlagInventoryAsBajaUseCase useCase) =>
            {
                try
                {
                    await useCase.ExecuteAsync(id);
                    return TypedResults.Ok((object)new { message = "El ítem de inventario ha sido marcado como dado de baja correctamente." });
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
            .WithName("FlagInventoryAsBaja")
            .WithSummary("Flag an item state as 'Baja' within the inventory system");
        }
    }
}