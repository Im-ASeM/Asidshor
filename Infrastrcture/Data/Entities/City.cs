using System.ComponentModel.DataAnnotations;

public class City
{
    [Key]
    public int Id { get; set; }
    public string CityName { get; set; }
    public int UserId  { get; set; }

    
   
}