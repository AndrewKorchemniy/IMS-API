using System;

namespace InventoryFunction
{
    public class Item
    {
        public string id { get; set; } = null!;
        public string name { get; set; } = null!;
        public string inventoryName { get; set; } = null!;
        public string companyName { get; set; } = null!;
        public string manufactureName { get; set; } = null!;
        public float? quantity { get; set; } = null;
        public string quantityRemaining { get; set; } = null!;
        public string quantityUnit { get; set; } = null!;
        public DateTime? dateIntroduced { get; set; } = null;
        public DateTime? dateExpired { get; set; } = null;
        public string location { get; set; } = null!;
        public bool isInspected { get; set; } = false;
        public string lotNumber { get; set; } = null!;
    }
}
