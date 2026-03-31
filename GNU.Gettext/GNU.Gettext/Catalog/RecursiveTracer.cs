using System.Text;

namespace GNU.Gettext;

public class RecursiveTracer
{
    public int Level { get; set; }
    public StringBuilder Text { get; private set; }

    public RecursiveTracer()
    {
        Text = new StringBuilder();
        Level = 0;
    }

    public void SaveToFile(string fileName)
    {
        using (StreamWriter outfile = new(fileName))
        {
            outfile.Write(Text.ToString());
        }
    }

    public void Indent()
    {
        for (int i = 0; i < Level; i++)
            Text.Append("\t");
    }
}

