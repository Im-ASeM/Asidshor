using System.ComponentModel.DataAnnotations;

public class UserMenu
{
    [Key]
    public int Id { get; set; }
    public int MenuId { get; set; }
    public Menu Menu { get; set; }
    public int UserId { get; set; }
    public User MyProperty { get; set; }
}