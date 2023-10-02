using System.ComponentModel.DataAnnotations;

namespace TestShop.Entitys
{
    public class CustomerShoppingCart : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public List<ItemCounter>? ListItems { get; set; }
        public DateTime LastCheck { get; set; }
        public class ItemCounter : BaseEntity
        {
	        [Key]
	        public int Id { get; set; }
	        public Item Item { get; set; }
	        public int Count { get; set; }
        }
		public CustomerShoppingCart()
        {
            Id = Guid.NewGuid();
            LastCheck = DateTime.Now;
        }
    }
}

    
