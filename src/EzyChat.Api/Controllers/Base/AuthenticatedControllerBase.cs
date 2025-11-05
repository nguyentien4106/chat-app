using EzyChat.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EzyChat.Api.Controllers.Base;

[Authorize]
public abstract class AuthenticatedControllerBase : ControllerBase
{
    protected Guid CurrentUserId => GetCurrentUserId();

    private Guid GetCurrentUserId()
    {
        // Try ClaimTypes.NameIdentifier first
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        // If not found, try alternative claim names that JWT might use
        if (string.IsNullOrEmpty(userIdClaim))
        {
            userIdClaim = User.FindFirst("sub")?.Value; // Standard JWT subject claim
        }
        
        if (string.IsNullOrEmpty(userIdClaim))
        {
            userIdClaim = User.FindFirst("nameid")?.Value; // Short form
        }
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedException("User ID not found in token.");
        }
        return userId;
    }
} 