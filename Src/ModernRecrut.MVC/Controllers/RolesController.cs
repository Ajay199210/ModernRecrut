using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ModernRecrut.MVC.Models.User;

namespace ModernRecrut.MVC.Controllers
{
    [Authorize(Roles = "Employe, Admin, RH")]
    public class RolesController : Controller
    {
        private readonly UserManager<Utilisateur>? _userManager;
        private readonly RoleManager<IdentityRole>? _roleManager;
        private readonly ILogger<RolesController> _logger;

        public RolesController(UserManager<Utilisateur>? userManager, RoleManager<IdentityRole>? roleManager,
            ILogger<RolesController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation($"[+] Obtention de {_roleManager.Roles.Count()} roles");
            return View(await _roleManager.Roles.ToListAsync());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityRole identityRole)
        {
            if (await _roleManager.Roles.AnyAsync(r => r.Name.ToLower() == identityRole.Name.ToLower()))
            {
                _logger.LogWarning($"[!] Essaie d'une attribution à un rôle déjà existant !");
                ModelState.AddModelError("Name", "Le role existe déjà");
            }

            if (ModelState.IsValid)
            {
                await _roleManager.CreateAsync(identityRole);
                
                _logger.LogInformation($"[+] `{identityRole.Name}` est créée");

                return RedirectToAction("Index");
            }

            return View(identityRole);
        }

        [HttpGet]
        public async Task<IActionResult> Assigner()
        {
            await RemplirSelectList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assigner(UserRoleViewModel userRoleViewModel)
        {
            var user = await _userManager.FindByIdAsync(userRoleViewModel.UserId.ToString());
            var userRoles = await _userManager.GetRolesAsync(user);

            // Vérifier si le rôle est déjà assigné à l'utilisateur
            if(userRoles.Contains(userRoleViewModel.RoleName))
            {
                _logger.LogWarning($"[!] L'utilisateur #{user.Id}  ne peut avoir le même rôle !");
                
                ModelState.AddModelError("RoleName", "L'utilisateur ne peut pas avoir le même rôle");
            }

            // Les conditions qui suivent vérifient si un rôle employé essaie de venir un candidat
            // et vice-versa
            if(userRoles.Contains("Candidat"))
            {
                // Liste temporaire pour qu'on valide qu'un candidat ne peut être 
                // un employé, admin ou un RH en même temps
                var tempRolesList = new List<string> { "Employe", "Admin", "RH" };
                if(tempRolesList.Any(r => r == userRoleViewModel.RoleName))
                {
                    _logger.LogWarning($"[!] Essaie d'attribuer un rôle candidat avec un rôle (employe, RH, ou admin) " +
                        $"pour l'utilisateur #{user.Id}");
                    
                    ModelState.AddModelError("RoleName", $"L'utilisateur ne peut pas avoir un rôle Candidat " +
                        $"et {userRoleViewModel.RoleName} et en même temps");
                }
            }
            else
            {
                if(userRoleViewModel.RoleName == "Candidat")
                {
                    _logger.LogWarning($"[!] Essaie d'attribuer un rôle employé, RH, ou admin pour l'utilisateur " +
                        $"#{user.Id} avec un rôle candidat !");

                    ModelState.AddModelError("RoleName", $"L'utilisateur ne peut pas avoir les rôles disponibles d'un employé " +
                        $"et candidat en même temps");
                }
            }
            
            if (ModelState.IsValid)
            {
                await _userManager.AddToRoleAsync(user, userRoleViewModel.RoleName);
                
                _logger.LogInformation($"[+] Le rôle {userRoleViewModel.RoleName} est assigné pour l'utilisateur " +
                    $"{user.Nom} {user.Prenom}");
                
                return RedirectToAction("Index");
            }

            await RemplirSelectList();
            return View(userRoleViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string name)
        {
            var roleASupprimer = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == name);
            return View(roleASupprimer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(IdentityRole identityRole)
        {
            var roleASupprimer = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Name == identityRole.Name);

            if (roleASupprimer != null)
            {
                foreach (var user in _userManager.Users)
                {
                    if(await _userManager.IsInRoleAsync(user, roleASupprimer.Name))
                    {
                        ModelState.AddModelError("NormalizedName", $"Vous ne pouvez pas supprimer le rôle {roleASupprimer.Name}" +
                            $" puisqu'il est associé avec au moins un utilisateur");
                        return View();
                    }
                }

                await _roleManager.DeleteAsync(roleASupprimer);

                _logger.LogInformation($"[+] Le rôle `{roleASupprimer.Name}` a été supprimé de la liste des rôles");
                return RedirectToAction("Index");
            }

            _logger.LogWarning($"[-] Le rôle `{roleASupprimer.Name}` n'est pas trouvé");

            return NotFound();
        }

        private async Task RemplirSelectList()
        {
            ViewBag.Users = new SelectList(await _userManager.Users.ToListAsync(), "Id", "UserName");
            ViewBag.Roles = new SelectList(await _roleManager.Roles.ToListAsync(), "Name", "Name");
        }
    }
}
