using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Pages
{
    public class CustomPageRouteModelConvention : IPageRouteModelConvention
    {
        public void Apply(PageRouteModel model)
        {
            var isIndexPage = model.ViewEnginePath.EndsWith("/Index", StringComparison.OrdinalIgnoreCase);

            foreach (var selector in model.Selectors.ToList())
            {
                var template = selector.AttributeRouteModel.Template;
                if (isIndexPage)
                {
                    var isIndexRoute = template.EndsWith("Index", StringComparison.OrdinalIgnoreCase);
                    if (isIndexRoute)
                    {
                        model.Selectors.Remove(selector);
                        continue;
                    }
                }
                else
                {
                    selector.AttributeRouteModel.Template =
                        AttributeRouteModel.CombineTemplates(template,
                            "{handler?}/{id?}");
                }
            }
        }
    }
}
