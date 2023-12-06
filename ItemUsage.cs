namespace InventoryFunction
{
    public class ItemUsage
    {
        string batchName { get; set; } = null!;
        string itemName { get; set; } = null!;
        float quantityUsed { get; set; } = float.NaN;
        string inventoryName { get; set; } = null!;
        string companyName { get; set; } = null!;

    }
}
