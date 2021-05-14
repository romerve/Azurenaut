using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace WebPortal.Pages
{
    public class AccountModel : PageModel
    {
        private readonly ILogger<AccountModel> _logger;

        public AccountModel(ILogger<AccountModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {

        }
    }
}
