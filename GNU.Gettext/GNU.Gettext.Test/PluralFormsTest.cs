using Xunit;

namespace GNU.Gettext.Test;

public class PluralFormsTest
{
    [Fact]
    public void TwoFormsEvalTest()
    {
        string expr = "nplurals=2; plural=(n != 1)";
        Assert.NotNull(PluralFormsCalculator.Make(expr + "\\n"));
        Assert.NotNull(PluralFormsCalculator.Make(expr + "\n"));
        Assert.NotNull(PluralFormsCalculator.Make(expr + ";"));

        PluralFormsCalculator pfc = PluralFormsCalculator.Make(expr);
        Assert.NotNull(pfc);
        Assert.Equal(2, pfc.NPlurals);

        Assert.Equal(1, pfc.Evaluate(0));
        Assert.Equal(0, pfc.Evaluate(1));
        Assert.Equal(1, pfc.Evaluate(5));
        Assert.Equal(1, pfc.Evaluate(23));
    }

    [Fact]
    public void ThreeFormsEvalTest()
    {
        string expr = "nplurals=3; plural=(n%10==1 && n%100!=11 ? 0 : n%10>=2 && n%10<=4 && (n%100<10 || n%100>=20) ? 1 : 2)";
        PluralFormsCalculator pfc = PluralFormsCalculator.Make(expr);
        Assert.NotNull(pfc);
        Assert.Equal(3, pfc.NPlurals);

        Assert.Equal(0, pfc.Evaluate(1));
        Assert.Equal(1, pfc.Evaluate(3));
        Assert.Equal(2, pfc.Evaluate(7));
        Assert.Equal(2, pfc.Evaluate(11));
    }

    [Fact]
    public void CatalogParseTest()
    {
        Catalog cat = new Catalog();
        cat.Load("./Data/Test01.po");

        PluralFormsCalculator pfc = PluralFormsCalculator.Make(cat.GetPluralFormsHeader());
        Assert.NotNull(pfc);
        Assert.Equal(3, pfc.NPlurals);

        Assert.Equal(0, pfc.Evaluate(1));
        Assert.Equal(1, pfc.Evaluate(3));
        Assert.Equal(2, pfc.Evaluate(7));
    }
}
