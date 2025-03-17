namespace Api.Web.Controllers.Auth;

internal record CreateUserResponse(
    Guid UserId,
    string FirebaseUid,
    string Email);