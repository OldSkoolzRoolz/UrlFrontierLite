


namespace KC.Dropins.FrontierCore;

public class FrontierOptions
{
    public bool FollowExternalLinks { get; set; }
    public bool EnforceRobotFile { get; set; }




    /// <summary>
    /// Delay between each call to eachhost access
    /// Defaults to 5 seconds and forced to at least 3 seconds
    /// </summary>
    public TimeSpan HostAccessDelayInterval { get; set; } = TimeSpan.FromSeconds(5);


    /// <summary>
    /// Maximum threads to be dedicated to scraping
    /// </summary>
    public int MaxThreads { get; set; }


    /// <summary>
    /// <value>A hard limit applied to each host. The frontier will prevent any host from getting accessed more than this count.</value>
    /// </summary>
    public int MaxHostAccessCount { get; set; }


    /// <summary>
    /// The maximum capacity of the queue
    /// the queue will grow very quickly use a resonable value
    /// </summary>
    public int QueueMaxCapacity { get; set; }


    /// <summary>
    /// The maximum number of retries for puppeteer
    /// </summary>
    public int MaxPuppeteerRetries { get; set; }


    /// <summary>
    /// Create a backup queue of the application crashes or is ended before it can be processed
    /// </summary>
    public bool UseBackupQueue { get; set; }



    /// <summary>
    /// A search term to favor if found in the page title or url
    /// </summary>
    public string? SearchTerm { get; set; }





    public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36";







    /// <summary>
    /// The default options for the frontier.
    /// </summary>
    /// <value>
    /// The default options are the recommended values following best practices. Any value can be overridden.</value>
    public static FrontierOptions DefaultOptions
    {
        get
        {
            return new FrontierOptions
            {
                FollowExternalLinks = false,
                EnforceRobotFile = false,
                HostAccessDelayInterval = TimeSpan.FromSeconds(30),
                MaxThreads = 10,
                MaxHostAccessCount = 300,
                QueueMaxCapacity = 2000,
                MaxPuppeteerRetries = 3,
                UseBackupQueue = false,
                SearchTerm = null

            };
        }
    }









}
