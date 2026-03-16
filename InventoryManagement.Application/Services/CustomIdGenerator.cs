using InventoryManagement.Application.Interfaces.Application;
using InventoryManagement.Application.Interfaces.Infrastructure.Repositories;
using InventoryManagement.Domain.Entities;
using InventoryManagement.Domain.Enums;

namespace InventoryManagement.Application.Services;

public class CustomIdGenerator : ICustomIdGenerator
{
    private readonly IInventoryItemRepository _itemRepository;

    public CustomIdGenerator(IInventoryItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public async Task<string> GenerateId(Inventory inventory)
    {
        if (inventory.CustomIdElements == null || !inventory.CustomIdElements.Any())
        {
            var defaultSeq = await _itemRepository.GetNextSequence(inventory.Id);
            return defaultSeq.ToString();
        }

        var parts = new List<string>();

        foreach (var element in inventory.CustomIdElements.OrderBy(e => e.Order))
        {
            switch (element.Type)
            {
                case CustomIdElementType.FixedText:
                    parts.Add(element.FixedText);
                    break;

                case CustomIdElementType.Random20Bit:
                    parts.Add(Random.Shared.Next(0, 1 << 20).ToString());
                    break;

                case CustomIdElementType.Random32Bit:
                    parts.Add(Random.Shared.NextInt64().ToString());
                    break;

                case CustomIdElementType.Random6Digit:
                    parts.Add(Random.Shared.Next(100000, 999999).ToString());
                    break;

                case CustomIdElementType.Random9Digit:
                    parts.Add(Random.Shared.Next(100000000, 999999999).ToString());
                    break;

                case CustomIdElementType.Guid:
                    parts.Add(Guid.NewGuid().ToString());
                    break;

                case CustomIdElementType.DateTime:
                    parts.Add(DateTime.UtcNow.ToString(
                        element.Format ?? "yyyyMMdd"));
                    break;

                case CustomIdElementType.Sequence:
                    var seq = await _itemRepository.GetNextSequence(inventory.Id);
                    if (!string.IsNullOrEmpty(element.Format))
                        parts.Add(seq.ToString(element.Format));
                    else
                        parts.Add(seq.ToString());
                    break;
            }
        }

        bool onlyFixedText = inventory.CustomIdElements.All(e => e.Type == CustomIdElementType.FixedText);

        if (onlyFixedText)
        {
            var fallbackSeq = await _itemRepository.GetNextSequence(inventory.Id);
            parts.Add(fallbackSeq.ToString());
        }

        return string.Join("", parts);
    }
}
