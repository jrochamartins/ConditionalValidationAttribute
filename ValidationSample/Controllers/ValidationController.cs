using Microsoft.AspNetCore.Mvc;
using ValidationSample.Validations;

namespace ValidationSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValidationController : ControllerBase
    {
        public record Post1Request([RequiredIf1("Validate ?? false", AllowEmptyStrings = false)] string? Name, bool? Validate, NameTypes? NameType);
        public record Post2Request([RequiredIf2("Model.Validate ?? false", AllowEmptyStrings = false)] string? Name, bool? Validate, NameTypes? NameType);
        public record Post3Request([RequiredIf3("Model.Validate ?? false", AllowEmptyStrings = false)] string? Name, bool? Validate, NameTypes? NameType);
        public record Post4Request([ValidateIf("(NameType ?? ValidationSample.Controllers.NameTypes.Name) == ValidationSample.Controllers.NameTypes.Address", typeof(RequiredStringAttribute), false)] string? Name, bool? Validate, NameTypes? NameType);

        [HttpPost(nameof(Post1))] public IResult Post1(Post1Request request) => Results.Ok(request);
        [HttpPost(nameof(Post2))] public IResult Post2(Post2Request request) => Results.Ok(request);
        [HttpPost(nameof(Post3))] public IResult Post3(Post3Request request) => Results.Ok(request);
        [HttpPost(nameof(Post4))] public IResult Post4(Post4Request request) => Results.Ok(request);
    }
}
