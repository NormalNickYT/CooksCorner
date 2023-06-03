using CooksCornerAPP.Models.DTO;
using Microsoft.AspNetCore.Mvc;

namespace CooksCornerAPP.Repositories.Abstract
{
    public interface IUserAuthenticationService
    {

        Task<Status> LoginAsync(Login model);
        Task LogoutAsync();
        Task<Status> RegisterAsync(Registration model);


    }
}
