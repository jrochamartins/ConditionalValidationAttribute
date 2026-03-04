using Microsoft.AspNetCore.Mvc;
using ValidationSample.Models;

namespace ValidationSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValidationController : ControllerBase
    {
        [HttpPost]
        public IResult Post(ValidateDocumentRequest? request) => Results.Ok(request);
    }
}
