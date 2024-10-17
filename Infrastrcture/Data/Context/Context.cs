using Microsoft.EntityFrameworkCore;
public class Context:DbContext
{
       public Context(DbContextOptions<Context> options) : base(options)
       {
        
       }
    public DbSet<User> Users { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<Walet> walets { get; set; }
    public DbSet<Price> Prices { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Orders> Orders { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Card> Cards { get; set; }
   public DbSet<City> Cities { get; set; }
       
        
        //  protected override void OnConfiguring(DbContextOptionsBuilder db)
        //  {
        //      db.UseSqlServer("Data Source=.;initial Catalog=cardb;integrated Security=SSPI;MultipleActiveResultSets=true");
        //  }

    // mhddotnet ef database update

    //  protected override void OnConfiguring(DbContextOptionsBuilder db)
    // {
    //     db.UseSqlServer("Data Source=.;initial Catalog=acid;USER ID=SA;PASSWORD=SM1384@@mh;MultipleActiveResultSets=true");
    // }


}