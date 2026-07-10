using Application.UseCase.Roles;
using Application.UseCases.DocumentTypes;
using Application.UseCases.Inventories;
using Application.UseCases.Municipalities;
using Application.UseCases.Notices;
using Application.UseCases.Positions;
using Application.UseCases.Problems;
using Application.UseCases.Projects;
using Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is missing. Please check your appsettings.json.");

builder.Services.AddData(connectionString);

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.Run();
