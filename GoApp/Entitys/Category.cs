﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace GoApp.Entitys;
[Owned]
public class Category : BaseEntity
{
	[Key]
	public int? Id { get; set; }
	public string Name { get; set; }
	public ICollection<Item> Items { get; set; }
}