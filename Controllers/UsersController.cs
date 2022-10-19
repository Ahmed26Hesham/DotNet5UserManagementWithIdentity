﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UserManagmentWithIdentity.Models;
using UserManagmentWithIdentity.ViewModels;

namespace UserManagmentWithIdentity.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this._roleManager = roleManager;
            this._userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.Select(user => new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                Roles = _userManager.GetRolesAsync(user).GetAwaiter().GetResult()

            }).ToListAsync();

            return View(users);
        }

        public async Task<IActionResult> Add()
        {
            var roles = await _roleManager.Roles.Select(role => new RoleViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name
            }).ToListAsync();

            var ViewModel = new AddUserViewModel() {
                Roles = roles
            };

            return View(ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddUserViewModel model)
        {
            if(!ModelState.IsValid)
                return View(model);

            if (!model.Roles.Any(r => r.IsSelected))
            {
                ModelState.AddModelError("Roles", "Please Select at least one role");
                return View(model);
            }

            if(await _userManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError("Email", "Email is already Exists");
                return View(model);
            }
            if (await _userManager.FindByNameAsync(model.UserName) != null)
            {
                ModelState.AddModelError("UserName", "UserName is already Exists");
                return View(model);
            }
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Roles", error.Description);
                }
                return View(model);
            }
            await _userManager.AddToRolesAsync(user, model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName));

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();


            var viewModel = new ProfileFormViewModel()
            {
                 Id = userId,  
                 FirstName = user.FirstName,
                 LastName = user.LastName,
                 UserName = user.UserName,
                 Email = user.Email
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
                return NotFound();

            var userWithSameEmail = await _userManager.FindByEmailAsync(model.Email);

            if(userWithSameEmail != null && userWithSameEmail.Id != model.Id)
            {
                ModelState.AddModelError("Email", "This Email is already assigned to another user");
                return View(model);
            }

            var userWithUserName = await _userManager.FindByNameAsync(model.UserName);

            if (userWithUserName != null && userWithUserName.Id != model.Id)
            {
                ModelState.AddModelError("UserName", "This UserName is already assigned to another user");
                return View(model);
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.UserName;
            user.Email = model.Email;

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound();

            var roles = await _roleManager.Roles.ToListAsync();

            var viewModel = new UserRolesViewModel()
            {
                UserId = userId,
                UserName = user.UserName,
                Roles = roles.Select(role => new RoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    IsSelected = _userManager.IsInRoleAsync(user,role.Name).Result

                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(UserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
                return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in model.Roles)
            {
                if (userRoles.Any(r => r == role.RoleName) && !role.IsSelected)
                    await _userManager.RemoveFromRoleAsync(user , role.RoleName);

                if (!userRoles.Any(r => r == role.RoleName) && role.IsSelected)
                    await _userManager.AddToRoleAsync(user, role.RoleName);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
