namespace InventoryManagement.Application.DTOs.Inventory;

public sealed record GetInventoryStatisticsResponse(
    int TotalItems,
    List<NumericFieldStatistic> NumericFields,
    List<StringFieldStatistic> StringFields
);

public sealed record NumericFieldStatistic(
    Guid FieldId,
    string FieldName,
    decimal Min,
    decimal Max,
    decimal Average
);

public sealed record StringFieldStatistic(
    Guid FieldId,
    string FieldName,
    Dictionary<string, int> TopValues
);