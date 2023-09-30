using System.ComponentModel.DataAnnotations;
using GoApp.back_end.Entitys;
using GoApp.Entitys;

namespace GoApp.back_end.Data
{
    public class CustomerShoppingCart : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public Guid CustomerId { get; set; } = Guid.NewGuid();
        public List<ItemCounter> ListItems { get; set; }

        public DateTime LastCheck { get; set; } = DateTime.Now;
    }

    public class ItemCounter
    {
        private Item Item { get; set; }
        private int Count { get; set; }
    }
}
