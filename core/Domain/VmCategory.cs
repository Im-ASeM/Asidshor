using Microsoft.AspNetCore.Http;
public class VmCategory
{
    public int Id { get; set; }
    public string CatName { get; set; }

    public int ParentId { get; set; }

    public string Status { get; set; }



}
