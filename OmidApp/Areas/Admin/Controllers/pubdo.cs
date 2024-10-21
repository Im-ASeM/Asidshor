public class pubdo
{
    private readonly Context db;
    public pubdo(Context context)
    {
        db = context;
    }

    public int unread(){
        return Convert.ToInt32(db.Requests.Where(x=>x.Status == "در انتظار تایید").Count());
    }
    
}