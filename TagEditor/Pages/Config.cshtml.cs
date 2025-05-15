using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TagEditor.Pages;

public class ConfigModel(Storage storage) : PageModel
{
    [BindProperty]
    public PromptConfig PromptConfig { get; set; }

    public IActionResult OnGet()
    {
        PromptConfig = storage.Get<PromptConfig>() ?? new PromptConfig();

        return Page();
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid) // バリデーションチェック
        {
            return Page();
        }
        storage.Set(PromptConfig);

        return Page();
    }
}
