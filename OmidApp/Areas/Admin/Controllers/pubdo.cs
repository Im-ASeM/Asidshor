using Microsoft.EntityFrameworkCore;

public class pubdo
{
    private readonly Context db;
    public pubdo(Context context)
    {
        db = context;
    }

    public int unread(string roll, int id)
    {
        var admin = db.Admins.Include(x => x.City).ThenInclude(x => x.Menu).FirstOrDefault(x => x.Id == id);
        if (roll == "admin")
        {
            return db.Requests.Where(x => x.Status == "در انتظار تایید").Count();
        }
        else if (admin.City == null) return 0;
        else
        {
            return db.Requests.Where(x => x.Status == "در انتظار تایید").AsEnumerable().Where(x=> admin.City.Menu.Any(m => m.MenuId == x.MenuId)).ToList().Count();
        }
    }

}