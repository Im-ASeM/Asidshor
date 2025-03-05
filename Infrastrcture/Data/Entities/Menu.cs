using System.ComponentModel.DataAnnotations;

public class Menu
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public List<CityMenu> City { get; set; }
    public List<UserMenu> UserMenus { get; set; }
    public int? AdminId { get; set; }
    public Admin Admin { get; set; }
}