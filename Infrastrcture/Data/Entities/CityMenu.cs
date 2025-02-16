using System.ComponentModel.DataAnnotations;

public class CityMenu
{
    [Key]
    public int Id { get; set; }
    public int MenuId { get; set; }
    public Menu Menu { get; set; }
    public int CityId { get; set; }
    public City City { get; set; }
}