﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace idunno.CookieSharing.Core.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {                        
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                const string Issuer = "urn:net-core";
                var identity = new ClaimsIdentity(
                    new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, model.Email, ClaimValueTypes.String, Issuer)
                    },
                    ClaimTypes.Email,
                    ClaimTypes.Role,
                    "Cookie");
                var principal = new ClaimsPrincipal(identity);
                
                
                await HttpContext.SignInAsync("CustomCookie", principal,
                    new AuthenticationProperties
                    {
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(20),
                        IsPersistent = false,
                        AllowRefresh = false
                    });

                return RedirectToAction("Index");
            }
            else
            {
                //TODO remove
                return View("Index", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("CustomCookie");
            return RedirectToAction("Index");
        }
    }
}
