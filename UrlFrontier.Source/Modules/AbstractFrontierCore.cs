using System.Text.RegularExpressions;

using NLog;


namespace KC.Dropins.FrontierCore;

public class AbstractFrontierCore
{




    protected static PriorityQueue<UrlItem> _queue = new();

    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);
    private readonly HashSet<string> _processedUrls = new HashSet<string>();
    private readonly FrontierOptions _frontierOptions;
    private readonly HostAccessTracker _hostAccessTracker;
    private bool _disposedValue;
    protected bool _isFull = false;
    protected bool _isEmpty = false;
    protected ILogger _logger = LogManager.GetCurrentClassLogger();
    private readonly object _lock = new();




    internal AbstractFrontierCore(FrontierOptions frontierOptions)
    {
        _frontierOptions = frontierOptions;
        _hostAccessTracker = new(_frontierOptions.MaxHostAccessCount, _frontierOptions.HostAccessDelayInterval);
    }


    /// <summary>
    /// Enqueues a URL to the frontier.
    /// </summary>
    /// <param name="url">The URL to enqueue.</param>
    /// <returns>A task that completes when the URL is enqueued.</returns>
    protected async Task EnqueueUrlCoreAsync(string url)
    {

        // Check if the queue is full
        // will not enqueue if the queue is full
        if (_queue.Count >= _frontierOptions.QueueMaxCapacity)
        {
            _isFull = true;
            return;
        }

        // Check if we have processed a url before
        if (_processedUrls.Contains(url))
        {
            return; // already processed don't add
        }
        if (_hostAccessTracker.IsAccessCountExceeded(new Uri(url).Host))
        {
            _logger.Info($"The maximum access count for {new Uri(url).Host} has been reached. The URL will not be enqueued.");
            return;
        }

        var item = await CalculatePriorityAsync(url);



        _queue.Enqueue(item, item.Priority);
        _semaphore.Release();
    }





















    /// <summary>
    /// <typeparam name="UrlItem"/>
    /// Calculates the priority of a URL based on various factors.
    /// </summary>
    /// <param name="url">The URL to calculate the priority for.</param>
    /// <returns>
    /// A task that completes with the new queue item and it's calculated priority.
    /// </returns>
    private Task<UrlItem> CalculatePriorityAsync(string url)
    {
        var host = new Uri(url).Host;
        // We want to have enough granularity in the priority calculation
        // so the priority scale is from 0 to 100 and we start with 50
        UrlItem newItem = new(url, GetPageTitle(url));

        // Priority Queue works on a counter-intuitive scale where 0 is highest priority.
        // In this case the highest priority is 0, the lowest priority is 100. We start at 100 **
        var priority = newItem.Priority;


        // If a search term is given in options and the page title contains term boost priority accordingly.
        priority -= CalculatePageTitlePriority(newItem);


        // Check if the URL contains the search term
        priority -= CalculateSearchTermPriority(newItem);



        // Check Host Access
        priority -= CalculateHostPriority(host);




        // Check if the URL is a spider trap
        // A spider trap is a group of URLs that only differ in query parameters
        // Lowest priority is given to spider traps
        if (IsSpiderTrap(url))
        {
            priority -= 20;
        }







        // Set the priority
        newItem.Priority = priority;

        return Task.FromResult(newItem);
    }
















    /// <summary>
    /// A higher priority (20) is given to URLs that contain the search term
    /// </summary>
    /// <param name="newItem"></param>
    /// <returns>An integer representing the priority to subtract from the original priority. Returns 0 if no search term is given or does not exist in page title.</returns>
    private int CalculatePageTitlePriority(UrlItem newItem)
    {
        if (_frontierOptions.SearchTerm == null)
        {
            return 0;
        }
        else
        {
            if (newItem.PageTitle != null && newItem.PageTitle.Contains(_frontierOptions.SearchTerm, System.StringComparison.OrdinalIgnoreCase))
            {
                return 20;
            }
            return 0;
        }
    }


    private string? GetPageTitle(string url)
    {




        HttpClient client = new HttpClient();
        HttpResponseMessage response = client.GetAsync(url).Result;

        if (response.IsSuccessStatusCode)
        {
            string html = response.Content.ReadAsStringAsync().Result;

            // Extract the page title using a regular expression
            string pattern = @"<title>\s*(.+?)\s*</title>";
            Match match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            else
            {
                return null;
            }
        }
        else
        {
            _logger.Error($"Failed to fetch the webpage. Status code: {response.StatusCode}");
            return null;
        }



    }


    private int GetUrlCount(string key)
    {
        // Example implementation: Assume a dictionary to store the number of URLs for each key
        Dictionary<string, int> urlCounts = new Dictionary<string, int>();

        // Calculate the count based on the key
        int count = urlCounts.GetValueOrDefault(key, 0);

        return count;
    }






    private bool IsSpiderTrap(string url)
    {
        //TODO: ImplementRule
        // An example of a spider trap is if a group of URLs only difference may only be query parameters
        // Return true if the URL is a spider trap, otherwise false
        return false;
    }










    private int CalculateHostPriority(string host)
    {
        var temp = 0;
        // Example implementation: Sort the hosts so a host just contacted will have the lowest priority
        // and the host contacted with the greatest timespan will have the highest priority
        // Return the calculated priority
        if (!_hostAccessTracker.IsAccessCountExceeded(host))
        {
            temp -= 10;
        }
        else if (!_hostAccessTracker.IsTimeSinceLastAccessExceeded(host))
        {
            temp -= 10;
        }
        return temp;
    }





    /// <summary>
    /// A higher priority (10) is given to URLs that contain the search term
    /// </summary>
    /// <param name="newItem"></param>
    /// <returns></returns>
    protected int CalculateSearchTermPriority(UrlItem newItem)
    {

        if (_frontierOptions.SearchTerm != null)
        {
            if (newItem.Address.Contains(_frontierOptions.SearchTerm, System.StringComparison.OrdinalIgnoreCase))
            {
                return 10;
            }
            else
            { return 0; }
        }
        else
        {
            return 0;
        }


    }






    /// <summary>
    /// Dequeues a URL from the queue.
    /// </summary>
    /// <returns>A task that completes with the dequeued URL. Returns an empty string if no URL is dequeued.</returns>
    protected UrlItem? DequeueUrlCore()
    {
        if (_queue.Count == 0)
        {
            _logger.Info("An attempt was made to retrieve a URL from an empty queue. This should not happen.");
            _isEmpty = true;
            return null;
        }



        UrlItem? result;
        // Must use lock to ensure that only one thread can access the queue at a time or collision will occur
        lock (_lock)
        {

            result = _queue.Dequeue();

            if (result != null)
            {
                if (_hostAccessTracker.IsAccessCountExceeded(result.BaseHost))
                {
                    _logger.Info($"The maximum access count for {result.BaseHost} has been reached. The URL will be skipped.");
                    return null;
                }
                else if (_hostAccessTracker.IsTimeSinceLastAccessExceeded(result.BaseHost))
                {
                    _logger.Info($"The mandetory wait time since last access for {result.BaseHost} has not expired yet. The URL will be put back in the queue with lower priority.");
                    _queue.Enqueue(result, result.Priority += 15);
                    return null;
                }
                _processedUrls.Add(result.Address); // mark as processed
                _hostAccessTracker.RecordAccess(new Uri(result.Address).Host); // mark as accessed
            }
        }
        return result ?? null;
    }








    public async Task OnAppStartupAsync()
    {

        var urlBack = await LoadFrontierBackupAsync();
        foreach (var item in urlBack)
        {
            await EnqueueUrlCoreAsync(item);
        }

    }






    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }



    private async Task<string[]> LoadFrontierBackupAsync()
    {
        if (_frontierOptions.UseBackupQueue)
        {
            var temp = await File.ReadAllLinesAsync("UrlFrontier.bak");
            return temp;
        }
        return new string[] { };
    }






    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~FrontierToo()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }






    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }







}
