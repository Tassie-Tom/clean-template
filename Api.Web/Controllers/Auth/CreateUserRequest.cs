namespace Api.Web.Controllers.Auth;

public record CreateUserRequest(
    string Email,
    string Name);
