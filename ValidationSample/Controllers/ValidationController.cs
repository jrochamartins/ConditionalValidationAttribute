using Microsoft.AspNetCore.Mvc;
using ValidationSample.Validations;

namespace ValidationSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValidationController : ControllerBase
    {
        public record WithDinamicCoreRequiredIfRequest([WithDinamicCoreRequiredIf("Validate ?? false", AllowEmptyStrings = false)] string? Name, bool? Validate, NameTypes? NameType);
        public record WithDynamicExpressoRequiredIfRequest([WithDynamicExpressoRequiredIf("Model.Validate ?? false", AllowEmptyStrings = false)] string? Name, bool? Validate, NameTypes? NameType);
        public record WithZExpresssionsRequiredIfRequest([WithZExpresssionsRequiredIf("Model.Validate ?? false", AllowEmptyStrings = false)] string? Name, bool? Validate, NameTypes? NameType);
        public record ValidateIfRequest([ValidateIf("(NameType ?? ValidationSample.Controllers.NameTypes.Name) == ValidationSample.Controllers.NameTypes.Address", typeof(RequiredStringAttribute), false)] string? Name, bool? Validate, NameTypes? NameType);

        [HttpPost(nameof(WithDinamicCoreRequiredIf))] public IResult WithDinamicCoreRequiredIf(WithDinamicCoreRequiredIfRequest request) => Results.Ok(request);
        [HttpPost(nameof(WithDynamicExpressoRequiredIf))] public IResult WithDynamicExpressoRequiredIf(WithDynamicExpressoRequiredIfRequest request) => Results.Ok(request);
        [HttpPost(nameof(WithZExpresssionsRequiredIf))] public IResult WithZExpresssionsRequiredIf(WithZExpresssionsRequiredIfRequest request) => Results.Ok(request);
        [HttpPost(nameof(ValidateIf))] public IResult ValidateIf(ValidateIfRequest request) => Results.Ok(request);
    }
}
