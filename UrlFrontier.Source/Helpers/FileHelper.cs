


namespace KC.Dropins.FrontierCore.Helpers;


internal static class FileHelper
{



    internal static void AppendLineToFile(string line, string path)
    {

        using (StreamWriter sw = File.AppendText(path))
        {
            sw.WriteLine(line);
        }
    }






}