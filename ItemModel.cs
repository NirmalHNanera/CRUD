using System.ComponentModel.DataAnnotations;

namespace Crud_Practice.Models
{
    public class ItemModel
    {
        public int? ItemId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Range(0.01, 999999999)]
        public decimal Price { get; set; }

        public string Category { get; set; }

        public bool IsAvailable { get; set; }

        public List<string> Features { get; set; } // Multi-select checkbox

        public string Tags { get; set; } // Multi-select

        public string SelectedRadioOption { get; set; } // Radio button
    }

}
