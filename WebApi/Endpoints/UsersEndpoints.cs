using Application.Exceptions;
using Application.UseCases.Users;
using Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using static Application.DTOs.UserDtos;

namespace WebApi.Endpoints
{
    public static class UsersEndpoints
    {
        public static void MapUsersEndpoints(this IEndpointRouteBuilder app)
        {
            var authGroup = app.MapGroup("/api/auth")
                .WithTags("Authentication");
               
            var userGroup = app.MapGroup("/api/users")
                .WithTags("Users")
                 .RequireAuthorization(new AuthorizeAttribute { Roles = "Administrador" });

            authGroup.MapPost("/login", async Task<Results<Ok<LoginResponse>, BadRequest<object>, ProblemHttpResult>> (
                [FromBody] LoginRequest request,
                LoginUseCase useCase) =>
            {
                try
                {
                    var response = await useCase.ExecuteAsync(request);
                    return TypedResults.Ok(response);
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
            .WithName("AuthLogin")
            .WithSummary("Authenticate user credentials and generate an access token");

            userGroup.MapGet("/{id:int}", async Task<Results<Ok<UserResponse>, NotFound<object>, ProblemHttpResult>> (
                int id,
                GetUserQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var user = await queriesUseCase.GetByIdAsync(id);
                    return TypedResults.Ok(user);
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
            .WithName("GetUserById")
            .WithSummary("Get a specific user by its identifier");

            userGroup.MapGet("/email", async Task<Results<Ok<UserResponse>, NotFound<object>, ProblemHttpResult>> (
                [FromQuery] string email,
                GetUserQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var user = await queriesUseCase.GetByEmailAsync(email);
                    return TypedResults.Ok(user);
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
            .WithName("GetUserByEmail")
            .WithSummary("Search for a user utilizing their unique email address");

            userGroup.MapGet("/", async Task<Results<Ok<IEnumerable<UserResponse>>, ProblemHttpResult>> (
                GetUserQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var users = await queriesUseCase.GetAllAsync();
                    return TypedResults.Ok(users);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetAllUsers")
            .WithSummary("Get all registered users");

            userGroup.MapGet("/role/{roleId:int}", async Task<Results<Ok<IEnumerable<UserResponse>>, ProblemHttpResult>> (
                int roleId,
                GetUserQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var users = await queriesUseCase.GetByRoleAsync(roleId);
                    return TypedResults.Ok(users);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetUsersByRole")
            .WithSummary("Get all users filtering by their system security role");

            userGroup.MapGet("/status", async Task<Results<Ok<IEnumerable<UserResponse>>, ProblemHttpResult>> (
                [FromQuery] bool isActive,
                GetUserQueriesUseCase queriesUseCase) =>
            {
                try
                {
                    var users = await queriesUseCase.GetByStatusAsync(isActive);
                    return TypedResults.Ok(users);
                }
                catch (Exception ex)
                {
                    return TypedResults.Problem(
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            })
            .WithName("GetUsersByStatus")
            .WithSummary("Get accounts matching active or suspended status");

            userGroup.MapPost("/", async Task<Results<Created<UserResponse>, BadRequest<object>, ProblemHttpResult>> (
                [FromBody] UserCreateRequest request,
                CreateUserUseCase useCase) =>
            {
                try
                {
                    var createdUser = await useCase.ExecuteAsync(request);
                    return TypedResults.Created($"/api/users/{createdUser.Id}", createdUser);
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
            .WithName("CreateUser")
            .WithSummary("Register a new system user with credentials and a secure role mapping");

            userGroup.MapPut("/{id:int}", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                [FromBody] UserUpdateProfileRequest request,
                UpdateUserProfileUseCase useCase) =>
            {
                if (id != request.Id)
                {
                    return TypedResults.BadRequest((object)new { error = "El ID del parámetro de ruta no coincide con el ID del cuerpo de la solicitud." });
                }

                try
                {
                    await useCase.ExecuteAsync(request);
                    return TypedResults.Ok((object)new { message = "El perfil del usuario ha sido actualizado correctamente." });
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
            .WithName("UpdateUserProfile")
            .WithSummary("Modify name, email, and security role properties of an active account");

            userGroup.MapPatch("/{id:int}/password", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                [FromBody] UserChangePasswordRequest request,
                ChangeUserPasswordUseCase useCase) =>
            {
                if (id != request.Id)
                {
                    return TypedResults.BadRequest((object)new { error = "El ID del parámetro de ruta no coincide con el ID del cuerpo de la solicitud." });
                }

                try
                {
                    await useCase.ExecuteAsync(request);
                    return TypedResults.Ok((object)new { message = "La contraseña del usuario se ha actualizado de forma segura." });
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
            .WithName("ChangeUserPassword")
            .WithSummary("Apply a fresh cryptographic hash to a user credential sequence");

            userGroup.MapPatch("/{id:int}/suspend", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                UserStatusManagementUseCase useCase) =>
            {
                try
                {
                    await useCase.SuspendAsync(id);
                    return TypedResults.Ok((object)new { message = "La cuenta del usuario ha sido suspendida correctamente." });
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
            .WithName("SuspendUser")
            .WithSummary("Restrict access permissions for a specific user account");

            userGroup.MapPatch("/{id:int}/reactivate", async Task<Results<Ok<object>, BadRequest<object>, NotFound<object>, ProblemHttpResult>> (
                int id,
                UserStatusManagementUseCase useCase) =>
            {
                try
                {
                    await useCase.ReactivateAsync(id);
                    return TypedResults.Ok((object)new { message = "La cuenta del usuario ha sido reactivada con éxito." });
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
            .WithName("ReactivateUser")
            .WithSummary("Restore permission grants for a suspended user account");
        }
    }
}