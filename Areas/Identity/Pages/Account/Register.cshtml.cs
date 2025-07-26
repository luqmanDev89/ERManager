// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using ERManager.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

namespace ERManager.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Admin")]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager; // Manages user sign-in
        private readonly UserManager<User> _userManager; // Manages user information
        private readonly IUserStore<User> _userStore; // User store interface
        private readonly IUserEmailStore<User> _emailStore; // Email store interface
        private readonly ILogger<RegisterModel> _logger; // Logger for the model
        private readonly IEmailSender _emailSender; // Service to send emails

        public RegisterModel(
            UserManager<User> userManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        // Properties bound to the view
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; } // URL to return to after registration

        public IList<AuthenticationScheme> ExternalLogins { get; set; } // External login schemes

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; } // User email

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; } // User password

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } // Confirm password field

            // Add the Role property
            [Required(ErrorMessage = "Role is required.")]
            [Display(Name = "Role")]
            public string Role { get; set; } // User role
        }

        // Handles GET requests to the register page
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl; // Set return URL
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(); // Get external logins
        }

        // Handles POST requests for user registration
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/"); // Default return URL if none provided
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(); // Get external logins

            if (ModelState.IsValid)
            {
                var user = CreateUser(); // Create user instance

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None); // Set username
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None); // Set email
                var result = await _userManager.CreateAsync(user, Input.Password); // Create user with password

                if (result.Succeeded)
                {
                    // Assign role to user
                    await _userManager.AddToRoleAsync(user, Input.Role); // Assign the selected role

                    _logger.LogInformation("User created a new account with password."); // Log account creation

                    // Generate email confirmation token and callback URL
                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    // Send confirmation email
                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    // Redirect based on confirmation requirement
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false); // Sign in the user
                        return LocalRedirect(returnUrl); // Redirect to the return URL
                    }
                }

                foreach (var error in result.Errors) // Add errors to model state if creation fails
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Redisplay form if something fails
            return Page();
        }


        // Creates a new user instance
        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>(); // Create an instance of User
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        // Gets the email store
        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support."); // Ensure email support is available
            }
            return (IUserEmailStore<User>)_userStore; // Return the email store
        }
    }
}
