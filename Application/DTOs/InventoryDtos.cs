
namespace Application.DTOs
{
    public static class InventoryDtos
    {
        public record InventoryCreateRequest(
                     string ItemName,
                     string Description,
                     int Quantity,
                     int MunicipalityId,
                     string ImageUrl);

        public record InventoryUpdateInfoRequest(
                    int Id,
                    string Description,
                    string ImageUrl);

        public record InventoryStockAdjustmentRequest(
                    int Id,
                    int Amount);

        public record InventoryResponse(
                    int Id,
                    string ItemName,
                    string Description,
                    int Quantity,
                    DateTime EntryDate,
                    string State,
                    string ImageUrl,
                    int MunicipalityId,
                    string MunicipalityName);
    }
}
