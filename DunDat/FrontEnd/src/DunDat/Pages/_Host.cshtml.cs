using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class HostAuthenticationModel : PageModel
{
    public async Task<IActionResult> OnGet()
    {
        if (User.Identity.IsAuthenticated)
        {
            AccessToken = await HttpContext.GetTokenAsync("access_token");
        }

        return Page();
    }

    public string? AccessToken { get; set; }
}