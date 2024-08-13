
using KC.Dropins.FrontierCore;

namespace KC.DropIns.FrontierCore;


public class Frontier : AbstractFrontierCore, IFrontier
{
    public int QueCount => _queue.Count;

    public bool IsFull => _isFull;
    public bool IsEmpty => _isEmpty;



    private readonly FrontierOptions _frontierOptions;

    public Frontier(FrontierOptions options) : base(options)
    {
        _frontierOptions = options;
    }





    public Task EnqueueUrlAsync(string url)
    {
        if (IsFull) return Task.CompletedTask;
        return base.EnqueueUrlCoreAsync(url);
    }






    public async Task<UrlItem?> DequeueUrlAsync()
    {
        if (IsEmpty)
        {
            _logger.Info("An attempt was made to retrieve a URL from an empty queue. This should not happen.");
            return null;
        }

        // Frontier has a hard coded 3 second delay between host calls. 
        // User setting will be honored if it is set.
        if (_frontierOptions.HostAccessDelayInterval.TotalMilliseconds <= 2999)
        {
            _logger.Warn("HostAccessDelayInterval is not valid. Setting to 3000 milliseconds.");
            _frontierOptions.HostAccessDelayInterval = TimeSpan.FromMilliseconds(3000);
        }


        // If the Url being dequeued has not exceeded the maximum access count for the host
        // we will force a delay between host calls here. 
        // We still want to adhere to the priority rules and not skip the host or url. This will break the rules.
        // Wait for host access delay  HOST POLITENESS RULES
        await Task.Delay((int)_frontierOptions.HostAccessDelayInterval.TotalMilliseconds);

        return base.DequeueUrlCore();


    }






    /// <summary>
    /// Gets the current size of the frontier.
    /// </summary>
    /// <returns>The number of URLs currently in the frontier.</returns>
    public static int QueueSize => _queue.Count;

public new int CalculateSearchTermPriority(UrlItem urlItem)
{
  return  base.CalculateSearchTermPriority(urlItem);
}



}