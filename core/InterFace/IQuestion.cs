public interface IQuestion
{

   VmUser check(string phone, int code);
    bool Savecode(string phone, int code);
    bool AddToAnswer(int id, string UserAnswer, string userid);
    Task<int> Rate(string userId);
    Task<int> GetFinalQuestion(string userId);
    Task<int> MaxRate();
    Task<int>  ShowResult(int myrate);
    Task<List<Vmresult>>  ShowResult2();

    
    Task<List<Vmresult>>  ShowResultAll(DateTime today);
    //GetFinalQuestion
    int MyRank(string userId);
    
     Task<List<Vmresult>>  ShowResultUser(int id);
   
    
    Task<bool> EndTime();
    //GetCorrectAnswer
    int GetCorrectAnswer(int id);


}