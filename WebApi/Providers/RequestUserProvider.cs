using System.Security.Claims;

using Microsoft.AspNetCore.Identity;

namespace WebApi.Providers {

    public class RequestUserProvider : IRequestUserProvider {
        private readonly HttpContext _context;

        public RequestUserProvider(IHttpContextAccessor contextAccessor) {
            _context = contextAccessor.HttpContext!;
        }

        public UserInfo? GetUserInfo() {
            if (!_context.User.Claims.Any()) {
                return null;
            }


            var userId = _context.User.Claims.First(e => e.Type == "userId").Value;
            var userName = _context.User.Identity!.Name!;

            return new UserInfo(userId, userName);
        }
    }
}
