using System;
using System.ComponentModel.DataAnnotations;

public class WaletNew
{

    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public string babat { get; set; }
    public int variz { get; set; }
    public int bardasht { get; set; }
    public DateTime date { get; set; }
    
}