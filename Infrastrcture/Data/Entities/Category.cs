using System.ComponentModel.DataAnnotations;

public class Category
{
    [Key]
    public int Id { get; set; }
    public string CatName { get; set; }

    public int ParentId { get; set; }

    public string Status { get; set; }
    
    
    
    
   
}