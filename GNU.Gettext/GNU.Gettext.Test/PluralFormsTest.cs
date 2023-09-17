using NUnit.Framework;

namespace GNU.Gettext.Test
{
    [TestFixture()]
    public class PluralFormsTest
    {
        [Test()]
        public void TwoFormsEvalTest()
        {
            string expr = "nplurals=2; plural=(n != 1)";
            Assert.IsNotNull(PluralFormsCalculator.Make(expr + "\\n"));
            Assert.IsNotNull(PluralFormsCalculator.Make(expr + "\n"));
            Assert.IsNotNull(PluralFormsCalculator.Make(expr + ";"));

            PluralFormsCalculator pfc = PluralFormsCalculator.Make(expr);
            Assert.IsNotNull(pfc, "Parse failed");
            Assert.That(2, Is.EqualTo(pfc.NPlurals), "Plurals count");

            Assert.That(1, Is.EqualTo(pfc.Evaluate(0)), "Case 1");
            Assert.That(0, Is.EqualTo(pfc.Evaluate(1)), "Case 2");
            Assert.That(1, Is.EqualTo(pfc.Evaluate(5)), "Case 3");
            Assert.That(1, Is.EqualTo(pfc.Evaluate(23)), "Case 4");
        }

        [Test()]
        public void ThreeFormsEvalTest()
        {
            string expr = "nplurals=3; plural=(n%10==1 && n%100!=11 ? 0 : n%10>=2 && n%10<=4 && (n%100<10 || n%100>=20) ? 1 : 2)";
            PluralFormsCalculator pfc = PluralFormsCalculator.Make(expr);
            Assert.IsNotNull(pfc, "Parse failed");
            Assert.That(3, Is.EqualTo(pfc.NPlurals), "Plurals count");


            Assert.That(0, Is.EqualTo(pfc.Evaluate(1)), "Case 1");
            Assert.That(1, Is.EqualTo(pfc.Evaluate(3)), "Case 2");
            Assert.That(2, Is.EqualTo(pfc.Evaluate(7)), "Case 3");
            Assert.That(2, Is.EqualTo(pfc.Evaluate(11)), "Case 4");
        }

        [Test()]
        public void CatalogParseTest()
        {
            Catalog cat = new Catalog();
            cat.Load("./Data/Test01.po");

            PluralFormsCalculator pfc = PluralFormsCalculator.Make(cat.GetPluralFormsHeader());
            Assert.IsNotNull(pfc, "Parse failed");
            Assert.That(3, Is.EqualTo(pfc.NPlurals), "Plurals count");

            Assert.That(0, Is.EqualTo(pfc.Evaluate(1)), "Case 1");
            Assert.That(1, Is.EqualTo(pfc.Evaluate(3)), "Case 2");
            Assert.That(2, Is.EqualTo(pfc.Evaluate(7)), "Case 3");
        }
    }
}

