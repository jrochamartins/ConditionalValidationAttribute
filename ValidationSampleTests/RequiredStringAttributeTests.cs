using ValidationSample.Validations;

namespace ValidationSample.Tests
{
    public class RequiredStringAttributeTests
    {   
        [Fact]
        public void Constructor_Default_SetsAllowEmptyStringsToFalse()
        {
            var attribute = new RequiredStringAttribute();

            Assert.False(attribute.AllowEmptyStrings);
        }

        [Fact]
        public void Constructor_WithTrue_SetsAllowEmptyStringsToTrue()
        {
            var attribute = new RequiredStringAttribute(allowEmptyStrings: true);

            Assert.True(attribute.AllowEmptyStrings);
        }

        
        [Fact]
        public void IsValid_WhenValueIsNull_ReturnsFalse()
        {
            var attribute = new RequiredStringAttribute(allowEmptyStrings: true);

            var isValid = attribute.IsValid(null);

            Assert.False(isValid);
        }

        [Fact]
        public void IsValid_WhenValueIsEmptyString_AndAllowEmptyIsFalse_ReturnsFalse()
        {
            var attribute = new RequiredStringAttribute(allowEmptyStrings: false);

            var isValid = attribute.IsValid(""); // String vazia

            Assert.False(isValid);
        }

        [Fact]
        public void IsValid_WhenValueIsEmptyString_AndAllowEmptyIsTrue_ReturnsTrue()
        {
            var attribute = new RequiredStringAttribute(allowEmptyStrings: true);

            var isValid = attribute.IsValid("");

            Assert.True(isValid);
        }

        [Fact]
        public void IsValid_WhenValueIsAValidString_ReturnsTrue()
        {
            var attribute = new RequiredStringAttribute();

            var isValid = attribute.IsValid("Texto válido preenchido");

            Assert.True(isValid);
        }
    }
}