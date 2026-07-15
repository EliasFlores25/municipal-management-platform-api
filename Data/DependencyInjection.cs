using Data.Persistence;
using Data.Repositories;
using Domain.Abstractions;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Data
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddData(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

            var config = TypeAdapterConfig.GlobalSettings;
            config.Scan(Assembly.GetExecutingAssembly());
            services.AddSingleton(config);
            services.AddScoped<IMapper, Mapper>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IMunicipalityRepository, MunicipalityRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();

            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<INoticeRepository, NoticeRepository>();
            services.AddScoped<IProblemRepository, ProblemRepository>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            services.AddScoped<IDocumentRepository, DocumentRepository>();
            return services;
        }
    }
}
