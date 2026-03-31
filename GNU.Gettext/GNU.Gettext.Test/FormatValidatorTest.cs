using Xunit;

namespace GNU.Gettext.Test;

public class FormatValidatorTest
{
    [Fact]
    public void DetectFormatsTest()
    {
        FormatValidator v1 = new FormatValidator("{{0}} {0} str1 {0:YYYY} str2 {1} str3 {2:####}");
        Assert.NotNull(v1.FormatItems);
        Assert.True(v1.ContainsFormat);
        Assert.Equal(4, v1.FormatItems.Length);

        FormatValidator v2 = new FormatValidator("{0} mot similaire trouvé");
        Assert.True(v2.ContainsFormat);
        Assert.Single(v2.FormatItems);

        FormatValidator v3 = new FormatValidator("mot similaire trouvé : {0}");
        Assert.True(v3.ContainsFormat);
        Assert.Single(v3.FormatItems);
    }

    [Fact]
    public void CrushTest()
    {
        FormatValidator v1 = new FormatValidator(null);
        Assert.NotNull(v1.FormatItems);
        Assert.False(v1.ContainsFormat);
        Assert.True(v1.Validate().Result);

        Assert.False(FormatValidator.IsFormatString(null));
    }

    [Fact]
    public void ValidationTest()
    {
        ValidateFormat(@"
{{0} {0} str1
{0:YYYY} str2 {1} {{
str3 {2:####}");
        ValidateFormat("{0} mot similaire trouvé", true);
        ValidateFormat("{0} mot {{ similaire trouvé", true);
        ValidateFormat("{0} mot { { similaire trouvé");
        ValidateFormat("{0} mot { { similaire trouvé } }");
        ValidateFormat("{0} mot { { similaire trouvé } }{1");
        ValidateFormat("{0} mot similaire trouvé }{1}");
        ValidateFormat("{0} mot similaire trouvé {}{1}");
        ValidateFormat("{0} mot {{ similaire }} trouvé {1}", true);
        ValidateFormat("{{ {{ {{ ", true);
        ValidateFormat("}} }} }} ", true);
    }

    private void ValidateFormat(string format, bool valid = false)
    {
        FormatValidator v1 = new FormatValidator(format);
        Assert.Equal(valid, v1.Validate().Result);
    }
}
