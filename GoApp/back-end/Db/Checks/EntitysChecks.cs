namespace TestShop.Entitys;

public class EntitysChecks
{
	public List<Item> CheckErrorImagesInItemList(string rootFolder, List<Item> items)
	{
		var newlist = items;
		foreach (var item in newlist)
		{
			if (!File.Exists(rootFolder + "/wwwroot/" + item.ImageString)) item.ImageString = "images/Error.png";
		}

		return newlist;
	}

	public Item CheckErrorImage(string rootFolder, Item item)
	{
		var newitem = item;

		if (!File.Exists(rootFolder + "/wwwroot/" + item.ImageString)) item.ImageString = "images/Error.png";
		
		return item;
	}
}