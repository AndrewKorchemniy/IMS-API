using System;

namespace InventoryFunction
{
    public class Record
    {
        string userName { get; set; } = null!;
        string batchName { get; set; } = null!;
        string inventoryName { get; set; } = null!;
        string companyName { get; set; } = null!;
        DateTime dateManufactured { get; set; } = DateTime.MinValue;
    }
}
