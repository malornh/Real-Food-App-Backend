using RF1.Models.Enums;

namespace RF1.Dtos
{
    public class ShortProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Type { get; set; }
        public double PricePerUnit { get; set; }
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
        public string? PhotoUrl { get; set; }
        public double? Rating { get; set; }
        public DateOnly DateUpdated { get; set; }
    }
}
