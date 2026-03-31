using Xunit;

namespace GNU.Gettext.Test
{
    public class CatalogTest
    {
        [Fact]
        public void ParsingTest()
        {
            Catalog cat = new Catalog();
            cat.Load("./Data/Test01.po");

            Assert.Equal(6, cat.Count);
            Assert.Equal(3, cat.PluralFormsCount);

            int nonTranslatedCount = 0;
            int ctx = 0;
            foreach (CatalogEntry entry in cat)
            {
                if (!entry.IsTranslated)
                    nonTranslatedCount++;
                if (entry.HasPlural)
                {
                    Assert.Equal("{0} ошибка найдена", entry.GetTranslation(0));
                    Assert.Equal("{0} ошибки найдены", entry.GetTranslation(1));
                    Assert.Equal("{0} ошибок найдено", entry.GetTranslation(2));
                }
                if (entry.HasContext)
                    ctx++;
            }

            Assert.Equal(1, nonTranslatedCount);
            Assert.Equal(2, ctx);
        }

        [Fact]
        public void ToGettextFormatTest()
        {
            Assert.Equal("123456", StringEscaping.UnEscape(StringEscaping.EscapeMode.CSharp, "123456"));
            Assert.Equal(@"12""3""456", StringEscaping.UnEscape(StringEscaping.EscapeMode.CSharpVerbatim, "12\"\"3\"\"456"));
            Assert.Equal("12\r\n\"3\"456", StringEscaping.UnEscape(StringEscaping.EscapeMode.CSharp, "12\\r\\n\\\"3\\\"456"));
            Assert.Equal("12\r\n\"3\"\r\n456", StringEscaping.UnEscape(StringEscaping.EscapeMode.CSharpVerbatim,
                @"12
""""3""""
456"));
        }
    }
}
