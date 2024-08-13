

# Url Frontier Core Drop-In

<p> The Url Frontier Project was a large distributed URL management and prioritizing system used in web scraping for search engine data. [^1] It is comprehensive and rule based to assign a priority value to each Url captured. That value determines when that Url is to be scraped.</p>


<P>This convenient Drop-In is designed to be quickly added to any project needing queue type management of urls. As urls are added to the queue a value is attached to the address based on rules or options enabled/disabled in the FrontierConfiguration type. It ensures the urls are valid and adhere to the desired patterns you are looking to capture and use.</p>

<p>During a scrape of a typical full featured site, it is not uncommon to quickly gather 20-30,000 urls in just a few minutes. It is for this reason that a tool such as The Frontier, distributed over 100's of servers and networks is a must have.</p>

This drop-in, "Frontier Core" is a more personal sized url manager and queue system. It is fully asynchronous, thread safe and scalable.  Urls are added to the system using the ```EnqueueAsync()``` method and then prioritized using the enabled options.  When you are ready to act on the next available url with the highest priority, a call to ```DequeueAsync()``` will deliver the next url. These calls are thread safe and can be called from any where.

<h2>Frontier Core Key Features</h2>

+ Easy Configuration   

+ Thread safe / Scalable   

+ robots.txt compatible
+ host access throttling - count or time based

---   

## Configuration:


##### ** See FrontierOptions type for all options available

```   

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

```

---

[^1]: [The Url Frontier - Stanford University](https://nlp.stanford.edu/IR-book/html/htmledition/the-url-frontier-1.html)