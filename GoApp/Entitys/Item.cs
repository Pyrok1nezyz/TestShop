using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoApp.Entitys;

public class Item : BaseEntity
{
	[Key]
	public int? Id { get; set; }
	public string Name { get; set; }
	public string Customer { get; set; }
	public string Description { get; set; }
	public int CategoryId { get; set; }
	[ForeignKey("CategoryId")]
	public Category? Category { get; set; }
	public float? Price { get; set; }
	public string ImageString { get; set; }
}