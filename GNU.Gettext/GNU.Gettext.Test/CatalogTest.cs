using NUnit.Framework;

namespace GNU.Gettext.Test
{
    [TestFixture()]
    public class CatalogTest
    {
        [Test()]
        public void ParsingTest()
        {
            Catalog cat = new Catalog();
            cat.Load("./Data/Test01.po");

            Assert.That(6, Is.EqualTo(cat.Count), "Entries count");
            Assert.That(3, Is.EqualTo(cat.PluralFormsCount), "Plurals entries count");

            int nonTranslatedCount = 0;
            int ctx = 0;
            foreach (CatalogEntry entry in cat)
            {
                if (!entry.IsTranslated)
                    nonTranslatedCount++;
                if (entry.HasPlural)
                {
                    Assert.That("{0} ошибка найдена", Is.EqualTo(entry.GetTranslation(0)));
                    Assert.That("{0} ошибки найдены", Is.EqualTo(entry.GetTranslation(1)));
                    Assert.That("{0} ошибок найдено", Is.EqualTo(entry.GetTranslation(2)));
                }
                if (entry.HasContext)
                    ctx++;
            }

            Assert.That(1, Is.EqualTo(nonTranslatedCount), "Non translated strings count");
            Assert.That(2, Is.EqualTo(ctx), "Contextes count");
        }


        [Test]
        public void ToGettextFormatTest()
        {
            Assert.That("123456", Is.EqualTo(StringEscaping.UnEscape(StringEscaping.EscapeMode.CSharp, "123456")), "Case 1");
            Assert.That(@"12""3""456", Is.EqualTo(StringEscaping.UnEscape(StringEscaping.EscapeMode.CSharpVerbatim, "12\"\"3\"\"456")), "Case 2");
            Assert.That("12\r\n\"3\"456", Is.EqualTo(StringEscaping.UnEscape(StringEscaping.EscapeMode.CSharp, "12\\r\\n\\\"3\\\"456")), "Case 3");
            Assert.That("12\r\n\"3\"\r\n456", Is.EqualTo(StringEscaping.UnEscape(StringEscaping.EscapeMode.CSharpVerbatim,
                @"12
""""3""""
456")), "Case 4");
        }
    }
}

