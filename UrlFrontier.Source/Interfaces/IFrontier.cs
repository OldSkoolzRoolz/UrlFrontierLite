namespace KC.Dropins.FrontierCore;




public interface IFrontier
{


    Task EnqueueUrlAsync(string url);

    Task<UrlItem?> DequeueUrlAsync();



    int QueCount { get; }


    bool IsFull { get; }


    bool IsEmpty { get; }



}




