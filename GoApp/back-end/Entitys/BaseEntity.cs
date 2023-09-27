using System.ComponentModel.DataAnnotations;

namespace GoApp.Entitys;

public class BaseEntity
{
	[Key]
	public int Id;
	public string Name;
}