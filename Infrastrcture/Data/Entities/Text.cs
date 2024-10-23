using System.ComponentModel.DataAnnotations;

public class Text
{
    [Key]
    public int Id { get; set; }
    public string text { get; set; }
}