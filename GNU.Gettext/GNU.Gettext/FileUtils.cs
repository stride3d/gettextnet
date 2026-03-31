namespace GNU.Gettext.Utils;

public static class FileUtils
{
    public static string GetRelativeUri(string uriString, string relativeUriString)
    {
        if ((!uriString.EndsWith("\\") || !uriString.EndsWith("/")) &&
            (relativeUriString.EndsWith("\\") || relativeUriString.EndsWith("/")))
            relativeUriString += "dummy";
        Uri fileUri = new Uri(uriString);
        Uri dirUri = new Uri(relativeUriString);
        Uri relativeUri = dirUri.MakeRelativeUri(fileUri);
        return relativeUri.ToString();
    }

    public static List<string> ReadStrings(string fileName)
    {
        return ReadStrings(fileName, null);
    }

    public static List<string> ReadStrings(string fileName, List<string> mergeWith)
    {
        List<string> strings = [];
        if (mergeWith is not null)
            strings = mergeWith;
        using StreamReader r = new StreamReader(fileName);
        string line;
        while ((line = r.ReadLine()) is not null && line.Trim().Length > 0)
        {
            if (!strings.Contains(line))
                strings.Add(line);
        }
        return strings;
    }
}

