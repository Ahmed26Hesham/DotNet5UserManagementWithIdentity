using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using UserManagmentWithIdentity.Models;

namespace UserManagmentWithIdentity.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]

    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UsersController(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

          var result =  await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
                throw new Exception();

            return Ok();

        }
    }
}
