using Application.UseCases.Municipalities;
using Application.UseCases.DocumentTypes;
using Application.UseCases.Inventories;
using Application.UseCases.Employees;
using Application.UseCases.Positions;
using Application.UseCases.Problems;
using Application.UseCases.Projects;
using Application.UseCases.Notices;
using Application.UseCases.Users;
using Application.UseCase.Roles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Infrastructure;
using System.Text;
using Data;
using Application.UseCases.Documents;
using WebApi.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing. Please check your appsettings.json.");

builder.Services.AddData(connectionString);

builder.Services.AddInfrastructureServices();

builder.Services.AddScoped<CreateRoleUseCase>();
builder.Services.AddScoped<GetAllRolesOrderedByNameUseCase>();
builder.Services.AddScoped<GetRoleByIdUseCase>();

builder.Services.AddScoped<CreateMunicipalityUseCase>();
builder.Services.AddScoped<DeleteMunicipalityUseCase>();
builder.Services.AddScoped<GetAllMunicipalitiesOrderedByNameUseCase>();
builder.Services.AddScoped<GetMunicipalityByIdUseCase>();
builder.Services.AddScoped<UpdateMunicipalityUseCase>();

builder.Services.AddScoped<CreatePositionUseCase>();
builder.Services.AddScoped<DeletePositionUseCase>();
builder.Services.AddScoped<GetAllPositionsOrderedByNameUseCase>();
builder.Services.AddScoped<GetPositionByIdUseCase>();
builder.Services.AddScoped<SearchPositionsByNameUseCase>();
builder.Services.AddScoped<UpdatePositionUseCase>();

builder.Services.AddScoped<CreateDocumentTypeUseCase>();
builder.Services.AddScoped<DeleteDocumentTypeUseCase>();
builder.Services.AddScoped<GetAllDocumentTypesOrderedByNameUseCase>();
builder.Services.AddScoped<GetDocumentTypeByIdUseCase>();
builder.Services.AddScoped<UpdateDocumentTypeUseCase>();

builder.Services.AddScoped<CreateInventoryUseCase>();
builder.Services.AddScoped<DecreaseInventoryStockUseCase>();
builder.Services.AddScoped<FlagInventoryAsBajaUseCase>();
builder.Services.AddScoped<GetInventoryQueriesUseCase>();
builder.Services.AddScoped<IncreaseInventoryStockUseCase>();
builder.Services.AddScoped<UpdateInventoryInfoUseCase>();

builder.Services.AddScoped<ChangeProjectStatusUseCase>();
builder.Services.AddScoped<CreateProjectUseCase>();
builder.Services.AddScoped<GetProjectQueriesUseCase>();
builder.Services.AddScoped<UpdateProjectPlanningUseCase>();

builder.Services.AddScoped<ArchiveNoticeUseCase>();
builder.Services.AddScoped<CreateNoticeUseCase>();
builder.Services.AddScoped<GetNoticeQueriesUseCase>();
builder.Services.AddScoped<UpdateNoticeContentUseCase>();

builder.Services.AddScoped<ChangeProblemStatusUseCase>();
builder.Services.AddScoped<CreateProblemUseCase>();
builder.Services.AddScoped<GetProblemQueriesUseCase>();
builder.Services.AddScoped<UpdateProblemReportUseCase>();

builder.Services.AddScoped<ChangeUserPasswordUseCase>();
builder.Services.AddScoped<CreateUserUseCase>();
builder.Services.AddScoped<GetUserQueriesUseCase>();
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<UpdateUserProfileUseCase>();
builder.Services.AddScoped<UserStatusManagementUseCase>();

builder.Services.AddScoped<CreateEmployeeUseCase>();
builder.Services.AddScoped<EmployeeLifecycleUseCase>();
builder.Services.AddScoped<GetEmployeeQueriesUseCase>();
builder.Services.AddScoped<UpdateEmployeeProfileUseCase>();

builder.Services.AddScoped<AnularDocumentUseCase>();
builder.Services.AddScoped<CreateDocumentUseCase>();
builder.Services.AddScoped<GetDocumentQueriesUseCase>();
builder.Services.AddScoped<UpdateDocumentDetailsUseCase>();

var jwtSection = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSection["SecretKey"]
    ?? throw new InvalidOperationException("La clave secreta de JWT no está configurada.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Alcaldia Management API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Ingrese 'Bearer' seguido de un espacio y su token JWT. Ejemplo: 'Bearer eyJhbGciOi...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapApiEndpoints();
app.Run();
