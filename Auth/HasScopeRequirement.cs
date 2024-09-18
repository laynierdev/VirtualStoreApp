﻿using Microsoft.AspNetCore.Authorization;

namespace VirtualStoreApp.Auth;
public class HasScopeRequirement : IAuthorizationRequirement
{
    public string Scope { get; }
    public string Issuer { get; }

    public HasScopeRequirement(string scope, string issuer)
    {
        Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
    }
}