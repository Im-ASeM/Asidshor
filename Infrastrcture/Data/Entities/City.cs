using System.ComponentModel.DataAnnotations;

public class City
{
    [Key]
    public int Id { get; set; }
    public string CityName { get; set; }
    public List<User> Users { get; set; }
    public List<Admin> Admins { get; set; }
    public List<CityMenu> Menu { get; set; }
}