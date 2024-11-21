using Coinnect.Data;
using Coinnect.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using static Coinnect.Models.AllModels;
using static Coinnect.Models.ViewModels.UserViewModels;

namespace Coinnect.Controllers
{
    [Authorize(Roles = "Admin, Consultant, Advisor")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserRepository _userRepository;
        private readonly CoinnectDbContext _coinnectDbContext;
        private readonly IBankAccountRepository _bankRepository;

        public AdminController(IUserRepository userRepository, UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager, CoinnectDbContext coinnectDbContext, IBankAccountRepository bankAccountRepository)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userRepository = userRepository;
            _coinnectDbContext = coinnectDbContext;
            _bankRepository = bankAccountRepository;
        }

        public async Task<IActionResult> UserList()
        {
            var users = _userManager.GetUserId(User);
            var _users = _coinnectDbContext.Users.Where(u => u.Id != users).ToList();
            var userList = new ListViewModel
            {
                Users = _users
            };

            foreach (var user in _users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Admin"))
                {
                    var userViewModel = new User
                    {
                        UserName = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        Identifier = user.Identifier
                    };
                }

            }

            return View(userList);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignRole()
        {
            var users = await _userManager.Users.ToListAsync();
            var model = new List<AssignRole>();

            foreach (var user in users)
            {
                var role = await _userManager.IsInRoleAsync(user, "Admin");
                if (!role)
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    model.Add(new AssignRole
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        Roles = userRoles
                    });
                }
                
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            var result = await _userManager.AddToRoleAsync(user, role);

            if (result.Succeeded)
            {
                TempData["Message"] = $"{user.UserName} has been assigned to the role '{role}'.";
                return RedirectToAction("AssignRole");
            }

            ModelState.AddModelError("", "Failed to assign role.");
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> UpdateUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(User model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.Identifier = model.Identifier;
                user.PhoneNumber = model.PhoneNumber;
                user.DateOfBirth = model.DateOfBirth;

                var result = await _userManager.UpdateAsync(user);
                var updater = await _userManager.GetUserAsync(User);
                if (result.Succeeded)
                {
                    DateTime date = DateTime.Now;
                    string details = $"User, {user.FirstName} {user.LastName}, with ID '{user.Id}' was successfully updated by {updater.FirstName} {updater.LastName} on {date:dd MMMM yyyy HH:mm:ss}.";

                    await LogAction(updater.Id, "Update user", user.Id, details, date);
                    TempData["Message"] = $"{user.UserName} has been updated";
                    return RedirectToAction("UserList", "Admin");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }

            await _userRepository.DeleteA(id);

            var result = await _userManager.DeleteAsync(user);
            var remover = await _userManager.GetUserAsync(User);
            if (result.Succeeded)
            {
                DateTime date = DateTime.Now;
                string details = $"User, {user.FirstName} {user.LastName}, with ID '{user.Id}' was successfully deleted by {remover.FirstName} {remover.LastName} on {date:dd MMMM yyyy HH:mm:ss}.";

                await LogAction(remover.Id, "Delete user", id, details, date);
                TempData["Message"] = "User has been deleted.";
                return RedirectToAction("UserList");
            }
            return View(user);
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateUser(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                if (await _roleManager.FindByNameAsync("Client") == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole("Client"));
                }

                User user = new User
                {
                    UserName = registerModel.Username,
                    Email = registerModel.Email,
                    FirstName = registerModel.Firstname,
                    LastName = registerModel.Lastname,
                    UserType = registerModel.UserType,
                    Identifier = registerModel.Identifier,
                    PhoneNumber = registerModel.ContactNumber,
                    DateOfBirth = registerModel.DOB

                };

                var result = await _userManager.CreateAsync(user, registerModel.Password);
                var creator = await _userManager.GetUserAsync(User);
                if (result.Succeeded)
                {

                    await _userManager.AddToRoleAsync(user, "Client");
                    DateTime date = DateTime.Now;
                    string details = $"User, {user.FirstName} {user.LastName}, with ID '{user.Id}' was successfully created by {creator.FirstName} {creator.LastName} on {date:dd MMMM yyyy HH:mm:ss}.";

                    await LogAction(creator.Id, "Create user", user.Id, details, date);
                    TempData["Message"] = $"{user.UserName} has been created";
                    return RedirectToAction("UserList", "Admin");
                }
                else
                {
                    ModelState.AddModelError("", "Unable to register new user");
                }
            }
            return View(registerModel);
        }
        public IActionResult UserDetails(string id)
        {
            var user = _coinnectDbContext.Users.FirstOrDefault(a => a.Id == id);
            return View(user);
        }
        public async Task<IActionResult> UsersAccounts(string id)
        {
            var accounts = await _bankRepository.GetAccountsByUserId(id);
            var list = new ListViewModel
            {
                Accounts = accounts
            };

            return View(list);
        }
        public async Task<IActionResult> GenerateReport()
        {
            var consultantId = _userManager.GetUserId(User);
            var actions = await _coinnectDbContext.actionLogs
                .Where(a => a.UserId == consultantId)
                .ToListAsync();
            var actionlogs = new ListViewModel
            {
                Actions = actions
            };

            return new ViewAsPdf("Report", actionlogs)
            {
                FileName = $"Report_{DateTime.Now:dd MMMM yyyy HH:mm:ss}.pdf"
            };
        }
        private async Task LogAction(string consultantId, string action, string targetUserId, string details, DateTime date)
        {
            var actionLog = new ActionLog
            {
                UserId = consultantId,
                ActionPerformed = action,
                TargetUserId = targetUserId,
                ActionDate = date,
                Details = details
            };

            _coinnectDbContext.actionLogs.Add(actionLog);
            await _coinnectDbContext.SaveChangesAsync();
        }


        public async Task<IActionResult> ViewReport()
        {
            var user = await _userManager.GetUserAsync(User);
            var actions = await _coinnectDbContext.actionLogs
                .Where(a => a.UserId == user.Id)
                .ToListAsync();
            var actionlog = new ListViewModel
            {
                Actions = actions
            };


            return View(actionlog);
        }
        public async Task<IActionResult> ViewAllAdvice()
        {
            var advices = await _coinnectDbContext.Advices
                                    .Include(a => a.Advisor)
                                    .Include(a => a.Client)
                                    .ToListAsync();
            var advicelist = new ListViewModel
            {
                Advices = advices
            };
            return View(advicelist);
        }

        public IActionResult MessagesList()
        {
            var _messages = _coinnectDbContext.Messages.ToList();
            var messages = new ListViewModel
            {
                Messages = _messages
            };
            return View(messages);
        }
    }
}
