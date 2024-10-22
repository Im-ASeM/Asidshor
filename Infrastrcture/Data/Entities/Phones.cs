using System.ComponentModel.DataAnnotations;

public class Phones
{
    [Key]
    public int id { get; set; }
    public string Phone { get; set; }
}