using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Server.Pages
{
    public class BasePageModel : PageModel
    {
        public PartialViewResult PartialView<T>(string viewName, T model)
            => new PartialViewResult
            {
                ViewName = viewName,
                ViewData = new ViewDataDictionary<T>(ViewData, model),
            };
    }
}
