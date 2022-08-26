using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using ContosoApp.Models;

namespace ContosoApp.Pages;

public class IndexModel : PageModel
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<IndexModel> _logger;
    public MyEnv opt {get; set;}

    public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public void OnGet()
    {
        opt = _configuration.GetSection("MyEnv").Get<MyEnv>();

    }
}
