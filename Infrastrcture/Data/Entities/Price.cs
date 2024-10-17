using System.ComponentModel.DataAnnotations;
public class Price
{
    // writed by mhd
    [Key]
    public int Id { get; set; }
    public int carId { get; set; }
    public int IdService { get; set; }

    public int PriceValue { get; set; }
    

}