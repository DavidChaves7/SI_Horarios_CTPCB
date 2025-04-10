using System.Security.Claims;

namespace SI_Horarios_CTPCB.Infrastructure.Services
{
    public class UserService
    {
        private ClaimsPrincipal currentUser = new(new ClaimsIdentity());

        public ClaimsPrincipal GetUser()
        {
            return currentUser;
        }

        internal void SetUser(ClaimsPrincipal user)
        {
            if (currentUser != user)
            {
                currentUser = user;
            }
        }
    }
}
