using Api.Application.Users.Create;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Web.Controllers.Auth;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(request.Email, request.Name);

        var result = await sender.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    //[HttpGet("check-email/{email}")]
    //public async Task<IActionResult> CheckEmailExists(string email)
    //{
    //    var query = new CheckEmailExistsQuery(email);
    //    var result = await sender.Send(query);

    //    return Ok(result.Value);
    //}
    //
}
