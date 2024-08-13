namespace KC.Dropins.FrontierCore;

public class UrlItem
{
    public string Address { get; set; }



    /// <summary>
    /// Value is the title of the page and is attempted to be retrieved
    /// during the calculation of the priority, if it cannot be retrieved value is null
    /// </summary>
    public string? PageTitle { get; set; }
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// Value is the base host of the Address given in .ctor
    /// </summary>
    public string BaseHost { get; }



    /// <summary>
    ///PriorityQueue's highest priority is the lowest number so we start with the lowest priority and 
    ///deduct when positive criteria is found based on the rules defined.
    /// </summary>
    public int Priority { get; set; } = 100;



    public UrlItem(string address, string? pageTitle)
    {
        this.CreatedDate = DateTime.Now;
        this.Address = address;
        this.PageTitle = pageTitle;
        this.BaseHost = new Uri(address).Host;
    }
}