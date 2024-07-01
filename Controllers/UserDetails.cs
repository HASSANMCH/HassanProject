using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using HassanProject.Data; // Assuming ApplicationDbContext is in this namespace
using HassanProject.Models; // Assuming ApplicationUser is in this namespace
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace HassanProject.Controllers
{
    [Authorize]
    public class UserDetailsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public UserDetailsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _context.Users.Select(u => new
            {
                u.FirstName,
                u.LastName,
                u.Address,
                u.PhoneNumber,
                u.City,
                u.Email,
                u.EmailConfirmed,
                u.Id,
            }).ToList();

            return Json(users); // Return the list directly without wrapping in an anonymous type
        }

    
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                City = user.City
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Address = model.Address;
                user.PhoneNumber = model.PhoneNumber;
                user.City = model.City;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Index", _userManager.Users.ToList());
        }

    }
}


//[HttpGet]
//public async Task<IActionResult> Index(string id)
//{


//    var user = await _userManager.FindByIdAsync(id);
//    if (user == null)
//    {
//        return NotFound();
//    }

//    var model = new UserDetailsViewModel
//    {
//        FirstName = user.FirstName,
//        LastName = user.LastName,
//        Address = user.Address,
//        PhoneNumber = user.PhoneNumber,
//        City = user.City,
//        Email = user.Email,
//    };

//    return View(model);
//}

//public IActionResult Index()
//{
//    return View();
//}















































