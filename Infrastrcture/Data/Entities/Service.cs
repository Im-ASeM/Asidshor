using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class Service
{
    // writed by mhd
    [Key]
    public int Id { get; set; }
    public string Srvicename { get; set; }
    public int Parentid { get; set; }
    public string Status { get; set; }
  
    [NotMapped]
    public int Price { get; set; }
    [NotMapped]
    public int Quantity { get; set; }
    public int? MenuId { get; set; }
    public Menu Menu { get; set; }
    public int? CatId { get; set; }
    public Category Cat { get; set; }
}