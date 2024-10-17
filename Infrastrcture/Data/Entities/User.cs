using System.ComponentModel.DataAnnotations;
public class User
{
    [Key]
    public int Id { get; set; }
    public string FirstAndLastName { get; set; }
    [Required]
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Url  { get; set; }
    public int Code { get; set; }
    public string Cart { get; set; }
    public string Adress { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public int free { get; set; }
    public int use { get; set; }

}