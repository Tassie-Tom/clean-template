﻿using Api.Application.Abstractions.Authenication;
using Api.Domain.Users;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Api.Infrastructure.Authentication;

internal sealed class UserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;

    private User? _cachedUser;
    private bool _userLoaded;

    public UserContext(
        IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }

    public Guid UserId => GetCurrentUserAsync().GetAwaiter().GetResult()?.Id ?? Guid.Empty;

    public string FirebaseId =>
        _httpContextAccessor.HttpContext?.User?.FindFirstValue("user_id") ?? string.Empty;

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public async Task<User?> GetCurrentUserAsync()
    {
        if (_userLoaded)
        {
            return _cachedUser;
        }

        if (!IsAuthenticated || string.IsNullOrEmpty(FirebaseId))
        {
            _userLoaded = true;
            return null;
        }

        _cachedUser = await _userRepository.GetByFirebaseIdAsync(FirebaseId);
        _userLoaded = true;

        return _cachedUser;
    }
}