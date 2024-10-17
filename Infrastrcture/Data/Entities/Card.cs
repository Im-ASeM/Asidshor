using System.ComponentModel.DataAnnotations;

public class Card
{
    [Key]
    public int Id { get; set; }
    public string CardNumber { get; set; }
    public string OwnerName { get; set; }

    
   
}