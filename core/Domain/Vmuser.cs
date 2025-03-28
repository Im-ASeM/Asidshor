using Microsoft.AspNetCore.Http;
public class VmUser
{
    public int Id { get; set; }
    public string FirstAndLastName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public IFormFile Urlfirst { get; set; }
    public string Url { get; set; }
    public int Code { get; set; }
    public string Cart { get; set; }
    public int CorrectAnswer { get; set; }
    public string Adress { get; set; }
    public string Latitude { get; set; }
    public string Longitude { get; set; }
    public int free { get; set; }
    public int use { get; set; }
    public string description { get; set; }
    public string Password { get; set; }
    public ICollection<Vm_WaletNew> walets { get; set; }
    public string CityName { get; set; }
    public List<int> Menus { get; set; }
}