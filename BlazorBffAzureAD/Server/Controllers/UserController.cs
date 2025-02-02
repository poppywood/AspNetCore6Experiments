﻿using BlazorHosted.Shared.Authorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

namespace BlazorBffOpenIDConnect.Server.Controllers;

// orig src https://github.com/berhir/BlazorWebAssemblyCookieAuth
[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult GetCurrentUser() => Ok(CreateUserInfo(User));

    private UserInfo CreateUserInfo(ClaimsPrincipal claimsPrincipal)
    {
        if (!claimsPrincipal?.Identity?.IsAuthenticated ?? true)
        {
            return UserInfo.Anonymous;
        }

        var userInfo = new UserInfo
        {
            IsAuthenticated = true
        };

        if (claimsPrincipal?.Identity is ClaimsIdentity claimsIdentity)
        {
            userInfo.NameClaimType = claimsIdentity.NameClaimType;
            userInfo.RoleClaimType = claimsIdentity.RoleClaimType;
        }
        else
        {
            userInfo.NameClaimType = ClaimTypes.Name;
            userInfo.RoleClaimType = ClaimTypes.Role;
        }

        if (claimsPrincipal?.Claims?.Any() ?? false)
        {
            var claims = claimsPrincipal.FindAll(userInfo.NameClaimType)
                                        .Select(u => new ClaimValue(userInfo.NameClaimType, u.Value))
                                        .ToList();

            // Uncomment this code if you want to send additional claims to the client.
            //var allClaims = claimsPrincipal.Claims.Select(u => new ClaimValue(userInfo.NameClaimType, u.Value))
            //                                      .ToList();
            //claims.AddRange(allclaims).Distinct();

            userInfo.Claims = claims;
        }

        return userInfo;
    }
}
