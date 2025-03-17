namespace Api.Web.Controllers.Auth;

internal record CreateUserRequest(
    string Email,
    string Name);
