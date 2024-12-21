using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using IdentityProvider.Application.Interfaces.Infrastructure;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
{
    private readonly string _role;

    // Make the 'role' parameter optional with a default value of null
    public CustomAuthorizeAttribute(string role = null)
    {
        _role = role;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Allow anonymous requests
        if (context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any())
            return;

        if (!context.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeaderValue))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var token = authorizationHeaderValue.FirstOrDefault()?.Split(" ").Last();
        if (string.IsNullOrEmpty(token))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        //var customAuthorizeAttribute = context.ActionDescriptor.EndpointMetadata
        //    .OfType<CustomAuthorizeAttribute>()
        //    .FirstOrDefault();

        //if (customAuthorizeAttribute != null && !string.IsNullOrEmpty(_role) && _role == "ADMIN")
        //{
        //    
        //}

        try
        {
            var decodedToken = DecodeJwt(token);


            var now = DateTime.UtcNow;
            var epirationDate = decodedToken.ValidTo;
            if (now > epirationDate)
            {
                context.Result = new UnauthorizedResult();
                return;
            }


            if (!HasRequiredRoles(decodedToken))
            {
                context.Result = new ForbidResult();
                return;
            }

            context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(decodedToken.Claims));
        }
        catch (Exception)
        {
            context.Result = new UnauthorizedResult();
            return;
        }
    }

    private JwtSecurityToken DecodeJwt(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

        if (jsonToken == null)
        {
            throw new InvalidOperationException("Invalid JWT token.");
        }

        return jsonToken;
    }

    private bool HasRequiredRoles(JwtSecurityToken jwtToken)
    {
        return string.IsNullOrEmpty(_role) || jwtToken.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value.ToLower() == _role.ToLower());
    }
}
