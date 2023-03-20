using AcademiaWeb.Identity.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AcademiaWeb.Identity.Services
{
    public class AuthenticateService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthenticateService(SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<bool> Authenticate(string email, string password)
        {
            //primeiro false define se vai armazenar de forma persistente
            //lockoutOnFailure define vai bloquear ou não caso haja falha
            var result = await _signInManager.PasswordSignInAsync(email,
                password, false, lockoutOnFailure: false);

            return result.Succeeded;
        }

        public async Task<bool> RegisterUser(string email, string password)
        {
            var applicationUser = new ApplicationUser
            {
                UserName = email,
                Email = email,
            };

            var result = await _userManager.CreateAsync(applicationUser, password);

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(applicationUser, isPersistent: false);
            }
            return result.Succeeded;
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
