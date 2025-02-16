public class MenuCityViewModel
{
    public int MenuId { get; set; }
    public string MenuName { get; set; }
    public List<CitySelection> Cities { get; set; }
}
public class CitySelection
{
    public int CityId { get; set; }
    public string CityName { get; set; }
    public bool IsSelected { get; set; }
}