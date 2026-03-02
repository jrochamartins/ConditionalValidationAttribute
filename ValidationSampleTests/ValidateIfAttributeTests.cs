using System.ComponentModel.DataAnnotations;
using ValidationSample.Validations;

namespace ValidationSample.Tests
{
    public class ValidateIfAttributeTests
    {
        public class InvestmentPortfolio
        {
            public int AssetType { get; set; } // 1 para FIIs, 2 para Ações B3

            [ValidateIf("AssetType == 1", typeof(RequiredAttribute))]
            public string? Ticker { get; set; }

            [ValidateIf("AssetType == 2", typeof(StringLengthAttribute), 5)]
            public string? StockCode { get; set; }
        }

        [Fact]
        public void Constructor_WhenConditionIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ValidateIfAttribute(null!, typeof(RequiredAttribute)));
        }

        [Fact]
        public void Constructor_WhenInnerValidatorTypeIsNull_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new ValidateIfAttribute("AssetType == 1", null!));
        }

        [Fact]
        public void Constructor_WhenInnerValidatorTypeIsInvalid_ThrowsArgumentException()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                new ValidateIfAttribute("AssetType == 1", typeof(string))); // String não é ValidationAttribute

            Assert.Contains("must be a ValidationAttribute", ex.Message);
        }

        [Fact]
        public void IsValid_WhenConditionIsFalse_IgnoresInnerValidator_AndReturnsSuccess()
        {
            var model = new InvestmentPortfolio { AssetType = 99, Ticker = null };
            var context = new ValidationContext(model);
            var attribute = new ValidateIfAttribute("AssetType == 1", typeof(RequiredAttribute));

            var result = attribute.GetValidationResult(model.Ticker, context);

            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void IsValid_WhenConditionIsTrue_AndInnerValidatorPasses_ReturnsSuccess()
        {
            var model = new InvestmentPortfolio { AssetType = 1, Ticker = "MXRF11" };
            var context = new ValidationContext(model);
            var attribute = new ValidateIfAttribute("AssetType == 1", typeof(RequiredAttribute));

            var result = attribute.GetValidationResult(model.Ticker, context);

            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void IsValid_WhenConditionIsTrue_AndInnerValidatorFails_ReturnsError()
        {
            var model = new InvestmentPortfolio { AssetType = 1, Ticker = null };
            var context = new ValidationContext(model);
            var attribute = new ValidateIfAttribute("AssetType == 1", typeof(RequiredAttribute));

            var result = attribute.GetValidationResult(model.Ticker, context);

            Assert.NotNull(result);
            Assert.NotEqual(ValidationResult.Success, result);
        }

        [Fact]
        public void IsValid_WithConstructorArguments_ConfiguresInnerValidatorCorrectly()
        {
            var model = new InvestmentPortfolio { AssetType = 2, StockCode = "PETR44" };
            var context = new ValidationContext(model);
            var attribute = new ValidateIfAttribute("AssetType == 2", typeof(StringLengthAttribute), 5);

            var result = attribute.GetValidationResult(model.StockCode, context);

            Assert.NotNull(result);
            Assert.NotEqual(ValidationResult.Success, result);
        }
    }
}