using System.Security.Claims;

namespace BE.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(
            this ClaimsPrincipal user)
        {
            var claim = user.FindFirst(
                ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                throw new Exception(
                    "UserId claim not found");
            }

            return Guid.Parse(claim.Value);
        }


        public static string GetEmail(
            this ClaimsPrincipal user)
        {
            return user.FindFirstValue(
                       ClaimTypes.Email)
                   ?? string.Empty;
        }


        public static string GetUsername(
            this ClaimsPrincipal user)
        {
            return user.FindFirstValue(
                       ClaimTypes.Name)
                   ?? string.Empty;
        }


        public static string GetRole(
            this ClaimsPrincipal user)
        {
            return user.FindFirstValue(
                       ClaimTypes.Role)
                   ?? string.Empty;
        }
    }
}
