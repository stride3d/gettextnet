using System.Resources;

namespace GNU.Gettext.Msgfmt;

public class ResourcesGen
{
    public Options Options { get; private set; }

    public ResourcesGen(Options options)
    {
        Options = options;
    }

    public void Run()
    {
        Catalog catalog = new();
        foreach (string fileName in Options.InputFiles)
        {
            Catalog temp = new();
            temp.Load(fileName);
            catalog.Append(temp);
        }

        using ResourceWriter writer = new(Options.OutFile);
        foreach (CatalogEntry entry in catalog)
        {
            try
            {
                writer.AddResource(entry.Key, entry.IsTranslated ? entry.GetTranslation(0) : entry.String);
            }
            catch (Exception e)
            {
                string message = string.IsNullOrEmpty(entry.Context)
                    ? $"Error adding item {entry.String}"
                    : $"Error adding item {entry.String} in context '{entry.Context}'";
                throw new Exception(message, e);
            }
        }
        writer.Generate();
    }
}
