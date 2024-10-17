using System.ComponentModel.DataAnnotations;
public class Admin
{
    [Key]
    public int Id { get; set; }

    public string NameFamily { get; set; }

    public string PhoneNumber { get; set; }

    public string UserName { get; set; }

    public string Password { get; set; }
    public string Role { get; set; }
    public string Status { get; set; }
    

}