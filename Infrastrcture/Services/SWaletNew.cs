using System.ComponentModel.DataAnnotations;
public class SWaletNew
{
    public static ICollection<Vm_WaletNew> convert(ICollection<WaletNew> walets)
    {
        ICollection<Vm_WaletNew> results = new List<Vm_WaletNew>();
        foreach (var walet in walets)
        {
            var result = new Vm_WaletNew
            {
                babat = walet.babat,
                bardasht = walet.bardasht,
                date = walet.date,
                Id = walet.Id,
                UserFullName = walet.User.FirstAndLastName,
                UserId = walet.UserId,
                UserName = walet.User.Url,
                UserPhone = walet.User.Phone,
                variz = walet.variz
            };
            results.Add(result);
        }
        return results;
    }
}
