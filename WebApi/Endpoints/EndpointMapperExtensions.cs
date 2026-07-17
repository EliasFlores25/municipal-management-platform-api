
namespace WebApi.Endpoints
{
    public static class EndpointMapperExtensions
    {
        public static void MapApiEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapRolesEndpoints();
            app.MapMunicipalitiesEndpoints();
            app.MapPositionsEndpoints();
            app.MapDocumentTypesEndpoints();
        }
    }
}
