using System.Text.Json;

using Microsoft.AspNetCore.Authorization;

internal class CanTestRequirement : IAuthorizationHandler, IAuthorizationRequirement {
    public Task HandleAsync(AuthorizationHandlerContext context) {

        var claim = context.User.Claims.FirstOrDefault(c => c.Type == "permissions");

        if (claim != null) {
            var permissions = JsonSerializer.Deserialize<string[]>(claim.Value);

            if (permissions.Contains("CanTest")) {
                context.Succeed(this);

                return Task.CompletedTask;
            }
        }

        context.Fail();

        return Task.CompletedTask;
    }
}