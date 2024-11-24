using InspectorJournal.Models;
using InspectorJournal.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InspectorJournal.Controllers;

[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.OrderBy(user => user.Id).ToList();

        List<UserViewModel> userViewModel = new();

        foreach (var user in users)
        {
            var userRoles = await _userManager.GetRolesAsync(user);
            var urole = userRoles.FirstOrDefault() ?? "";

            userViewModel.Add(new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RegistrationDate = user.RegistrationDate,
                RoleName = urole
            });
        }

        return View(userViewModel);
    }


    public IActionResult Create()
    {
        var allRoles = _roleManager.Roles.ToList();
        CreateUserViewModel user = new();

        ViewData["UserRole"] = new SelectList(allRoles, "Name", "Name");

        return View(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Проверка, существует ли пользователь с таким email
            var existingUserByEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingUserByEmail != null)
                ModelState.AddModelError(string.Empty, "Пользователь с таким email уже существует.");

            // Проверка, существует ли пользователь с таким username
            var existingUserByName = await _userManager.FindByNameAsync(model.UserName);
            if (existingUserByName != null)
                ModelState.AddModelError(string.Empty, "Пользователь с таким именем уже существует.");

            // Если есть ошибки, перезагружаем список ролей и возвращаем представление с ошибками
            if (!ModelState.IsValid)
            {
                var allRoles = _roleManager.Roles.ToList(); // Изменено на roleList
                ViewData["UserRole"] = new SelectList(allRoles, "Name", "Name");
                return View(model);
            }

            ApplicationUser user = new()
            {
                Email = model.Email,
                UserName = model.UserName,
                RegistrationDate = model.RegistrationDate
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Убедитесь, что роль существует
                var role = model.UserRole;
                if (!string.IsNullOrEmpty(role))
                {
                    var roleExist = await _roleManager.RoleExistsAsync(role);
                    if (roleExist)
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Роль не существует.");
                        var allRoles = _roleManager.Roles.ToList(); // Изменено на roleList
                        ViewData["UserRole"] = new SelectList(allRoles, "Name", "Name");
                        return View(model);
                    }
                }

                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
        }

        // Если произошла ошибка, обновляем список ролей и перезагружаем форму с ошибками
        var roleList = _roleManager.Roles.ToList(); // Изменено на roleList
        ViewData["UserRole"] = new SelectList(roleList, "Name", "Name");
        return View(model);
    }


    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);
        var allRoles = _roleManager.Roles.ToList();
        var userRole = "";
        if (userRoles.Count > 0) userRole = userRoles[0] ?? "";

        EditUserViewModel model = new()
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            RegistrationDate = user.RegistrationDate,
            UserRole = userRole
        };
        ViewData["UserRole"] = new SelectList(allRoles, "Name", "Name", model.UserRole);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user != null)
            {
                var oldRoles = await _userManager.GetRolesAsync(user);

                if (oldRoles.Count > 0) await _userManager.RemoveFromRolesAsync(user, oldRoles);

                var newRole = model.UserRole;
                if (newRole.Length > 0) await _userManager.AddToRoleAsync(user, newRole);

                user.Email = model.Email;
                user.UserName = model.UserName;
                user.RegistrationDate = model.RegistrationDate;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                // Добавляем ошибки в ModelState
                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // При возникновении ошибки, обновляем список ролей для отображения в форме
        var allRoles = await _roleManager.Roles.ToListAsync();
        ViewBag.UserRole = new SelectList(allRoles, "Name", "Name", model.UserRole);

        return View(model);
    }


    [HttpPost]
    public async Task<ActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            var result = await _userManager.DeleteAsync(user);
        }

        return RedirectToAction("Index");
    }
}