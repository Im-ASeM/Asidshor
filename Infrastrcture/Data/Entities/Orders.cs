using System.ComponentModel.DataAnnotations;

public class Orders
{
    [Key]
    public int Id { get; set; }
    public int IdRequest { get; set; }

    public string ChildServiceName { get; set; }

    public int Price { get; set; }

    public virtual Request Request { get; set; }

    
    
    
    
   
}