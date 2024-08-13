




namespace KC.Dropins.FrontierCore;

public class HostAccessTracker
{



    public  Dictionary<string, AccessDataObj> AccessData { get; set; }
    public readonly int _maxAccessCount;
    public readonly TimeSpan _minTimeSinceLastAccess;

    public HostAccessTracker(int maxAccessCount, TimeSpan minTimeSinceLastAccess)
    {
        AccessData = new Dictionary<string, AccessDataObj>();
        _maxAccessCount = maxAccessCount;
        _minTimeSinceLastAccess = minTimeSinceLastAccess;
    }

    public void RecordAccess(string host)
    {
        if (!AccessData.TryGetValue(host, out var accessData))
        {
            accessData = new AccessDataObj();
            AccessData[host] = accessData;
        }
        accessData.AccessCount++;
        accessData.LastAccessTime = DateTime.Now;
        TrimAccessData();
    }

    private void TrimAccessData()
    {
        var now = DateTime.Now;
        var keysToRemove = new List<string>();
        foreach (var kvp in AccessData)
        {
            var timeSinceLastAccess = now - kvp.Value.LastAccessTime;
            if (kvp.Value.AccessCount >= _maxAccessCount || timeSinceLastAccess < _minTimeSinceLastAccess)
            {
                keysToRemove.Add(kvp.Key);
            }
        }
        foreach (var key in keysToRemove)
        {
            AccessData.Remove(key);
        }
    }

    public int GetAccessCount(string host)
    {
        AccessData.TryGetValue(host, out var accessData);
        return accessData?.AccessCount ?? 0;
    }

    public TimeSpan GetTimeSinceLastAccess(string host)
    {
        AccessData.TryGetValue(host, out var accessData);
        return accessData?.LastAccessTime == null ? TimeSpan.Zero : DateTime.Now - accessData.LastAccessTime;
    }

    public bool IsAccessCountExceeded(string host)
    {
        return AccessData.TryGetValue(host, out var accessData) && accessData.AccessCount >= _maxAccessCount;
    }

    public bool IsTimeSinceLastAccessExceeded(string host)
    {
        if (AccessData.TryGetValue(host, out var accessData))
        {
            var timeSinceLastAccess = DateTime.Now - accessData.LastAccessTime;
            return timeSinceLastAccess >= _minTimeSinceLastAccess;
        }
        return false;
    }

 public class AccessDataObj
    {
        public int AccessCount { get; set; }
        public DateTime LastAccessTime { get; set; }
    }

}