using NUnit.Framework;

namespace GNU.Gettext.Test
{
    [TestFixture()]
    public class FormatValidatorTest
    {
        [Test()]
        public void DetectFormatsTest()
        {
            FormatValidator v1 = new FormatValidator("{{0}} {0} str1 {0:YYYY} str2 {1} str3 {2:####}");
            Assert.IsNotNull(v1.FormatItems);
            Assert.IsTrue(v1.ContainsFormat);
            Assert.That(4, Is.EqualTo(v1.FormatItems.Length));

            FormatValidator v2 = new FormatValidator("{0} mot similaire trouvé");
            Assert.IsTrue(v2.ContainsFormat);
            Assert.That(1, Is.EqualTo(v2.FormatItems.Length));

            FormatValidator v3 = new FormatValidator("mot similaire trouvé : {0}");
            Assert.IsTrue(v3.ContainsFormat);
            Assert.That(1, Is.EqualTo(v3.FormatItems.Length));
        }

        [Test()]
        public void CrushTest()
        {
            FormatValidator v1 = new FormatValidator(null);
            Assert.IsNotNull(v1.FormatItems);
            Assert.IsFalse(v1.ContainsFormat);
            Assert.IsTrue(v1.Validate().Result);

            Assert.IsFalse(FormatValidator.IsFormatString(null));
        }

        [Test()]
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
            Assert.That(valid, Is.EqualTo(v1.Validate().Result), "'{0}'\n{1}: {2}", format, v1.Validate().ErrorType, v1.Validate().ErrorMessage);
        }
    }
}

