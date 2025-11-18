# Role-Based Authorization Guide

This guide explains how to implement role-based authorization in the Railway Ticket Booking application.

## Overview

The application uses JWT tokens for authentication and role-based authorization. Users are assigned roles (SuperAdmin, Admin, ZonalManager, Customer) that determine which pages they can access.

## How It Works

1. **Authentication**: When a user logs in, a JWT token is generated and stored in localStorage
2. **Authorization**: The `CustomAuthenticationStateProvider` reads the token and validates it
3. **Role Claims**: The JWT token contains role information that is used for authorization
4. **Page Protection**: Pages can be protected using the `[Authorize]` attribute

## Protecting Pages

### Method 1: Using [Authorize] Attribute with Roles

Add the `[Authorize]` attribute at the top of your Razor page:

```razor
@page "/admin-page"
@using Microsoft.AspNetCore.Authorization
@attribute [Authorize(Roles = "Admin,SuperAdmin")]

<h3>Admin Page</h3>
```

**Available Role Options:**

- `[Authorize(Roles = "SuperAdmin")]` - Only SuperAdmin
- `[Authorize(Roles = "Admin,SuperAdmin")]` - Admin or SuperAdmin
- `[Authorize(Roles = "ZonalManager,Admin,SuperAdmin")]` - ZonalManager, Admin, or SuperAdmin
- `[Authorize(Roles = "Customer")]` - Only Customer
- `[Authorize]` - Any authenticated user

### Method 2: Using Authorization Policies

You can also use predefined policies from `Program.cs`:

```razor
@page "/admin-page"
@using Microsoft.AspNetCore.Authorization
@attribute [Authorize(Policy = "AdminOnly")]

<h3>Admin Page</h3>
```

**Available Policies:**
- `SuperAdminOnly` - Only SuperAdmin
- `AdminOnly` - Admin or SuperAdmin
- `ZonalManagerOnly` - ZonalManager, Admin, or SuperAdmin
- `CustomerOnly` - Only Customer
- `AuthenticatedUsers` - Any authenticated user

### Method 3: Conditional Rendering Based on Roles

You can conditionally show/hide content based on user roles:

```razor
@page "/dashboard"
@using Microsoft.AspNetCore.Authorization
@inject AuthenticationStateProvider AuthenticationStateProvider

@if (context.User.IsInRole("Admin") || context.User.IsInRole("SuperAdmin"))
{
    <div>Admin Content</div>
}

@code {
    private AuthenticationState? context;

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
        context = authState;
    }
}
```

## Example Pages

Example pages demonstrating different authorization levels:

1. **SuperAdmin Only**: `/superadmin-only`
   - Only accessible to SuperAdmin users

2. **Admin Only**: `/admin-only`
   - Accessible to Admin and SuperAdmin users

3. **Customer Only**: `/customer-only`
   - Only accessible to Customer users

4. **Authenticated Users**: `/authenticated`
   - Accessible to any logged-in user

## Accessing User Information

To access user information in your pages:

```razor
@using Microsoft.AspNetCore.Components.Authorization

@code {
    [CascadingParameter]
    private Task<AuthenticationState>? authenticationStateTask { get; set; }

    private AuthenticationState? context;

    protected override async Task OnInitializedAsync()
    {
        if (authenticationStateTask != null)
        {
            context = await authenticationStateTask;
        }
    }

    // Access user claims
    private string? GetUserEmail() => 
        context?.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
    
    private string? GetUserRole() => 
        context?.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
    
    private string? GetUserId() => 
        context?.User?.FindFirst("UserId")?.Value;
}
```

## Logout

Users can logout by navigating to `/logout` or by calling the logout functionality:

```razor
@inject NavigationManager NavigationManager
@inject IJSRuntime JSRuntime
@inject AuthenticationStateProvider AuthenticationStateProvider

<button @onclick="Logout">Logout</button>

@code {
    private async Task Logout()
    {
        await JSRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        // ... remove other items
        
        if (AuthenticationStateProvider is CustomAuthenticationStateProvider customProvider)
        {
            customProvider.NotifyUserLogout();
        }
        
        NavigationManager.NavigateTo("/login");
    }
}
```

## User Roles

The application supports the following roles:

1. **SuperAdmin** (1) - Full system access
2. **Admin** (2) - Administrative access
3. **ZonalManager** (3) - Regional management access
4. **Customer** (4) - Standard user access

## Best Practices

1. **Always protect admin pages**: Use `[Authorize(Roles = "Admin,SuperAdmin")]` for admin functionality
2. **Use least privilege**: Only grant the minimum required role access
3. **Check roles in code**: When performing sensitive operations, verify the user's role
4. **Handle unauthorized access**: The system automatically redirects unauthorized users to login

## Troubleshooting

**Issue**: User is redirected to login even after logging in
- **Solution**: Check that the JWT token is being stored correctly in localStorage
- **Solution**: Verify the token hasn't expired (default: 60 minutes)

**Issue**: User can't access a page even with correct role
- **Solution**: Check that the role name matches exactly (case-sensitive)
- **Solution**: Verify the JWT token contains the correct role claim

**Issue**: Authorization not working
- **Solution**: Ensure `CascadingAuthenticationState` is configured in `App.razor`
- **Solution**: Verify `CustomAuthenticationStateProvider` is registered in `Program.cs`

