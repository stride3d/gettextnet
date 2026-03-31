using Xunit;

using GNU.Gettext.Xgettext;

namespace GNU.Gettext.Test
{
    public class XgettextTest
    {
        [Fact]
        public void ExtractorCSharpTest()
        {
            string ressourceId = string.Format("{0}.{1}", this.GetType().Assembly.GetName().Name, "Data.XgettextTest.txt");
            string text = "";
            using (Stream stream = this.GetType().Assembly.GetManifestResourceStream(ressourceId))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    text = reader.ReadToEnd();
                }
            }

            Options options = new Options();
            options.InputFiles.Add(@"./Test/File/Name.cs");
            options.OutFile = @"./Test.pot";
            options.Overwrite = true;
            ExtractorCsharp extractor = new ExtractorCsharp(options);
            extractor.GetMessages(text, options.InputFiles[0]);
            extractor.Save();

            int ctx = 0;
            int multiline = 0;
            foreach (CatalogEntry entry in extractor.Catalog)
            {
                if (entry.HasContext)
                    ctx++;
                if (entry.String == "multiline-string-1-string-2" ||
                    entry.String == "Multiline Hint for label1")
                    multiline++;
            }

            Assert.Equal(2, ctx);
            Assert.Equal(2, extractor.Catalog.PluralFormsCount);
            Assert.Equal(17, extractor.Catalog.Count);
            Assert.Equal(2, multiline);
        }

        [Fact]
        public void RemoveCommentsTest()
        {
            string input = @"
/*
 *
 * This
 * is
 * // Comment
 */
string s = ""/*This is not comment*/"";
string s2 = ""This is //not comment too"";
button1.Text = ""Save""; // Save data.Text = ""10""
//button1.Text = ""Save""; // Save data.Text = ""10""
// button1.Text = ""Save""; // Save data.Text = ""10""
/*button1.Text = ""Save""; // Save data.Text = ""10""*/
";
            string output = ExtractorCsharp.RemoveComments(input);
            Assert.True(output.IndexOf("/*This is not comment*/") >= 0);
            Assert.True(output.IndexOf("This is //not comment too") >= 0);
            Assert.Equal(-1, output.IndexOf("// Save"));
            Assert.Equal(-1, output.IndexOf("//button1"));
            Assert.Equal(-1, output.IndexOf("/*\n"));
            Assert.Equal(-1, output.IndexOf("/*button1"));
        }
    }
}
