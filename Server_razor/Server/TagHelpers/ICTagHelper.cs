using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Server.TagHelpers
{
    [HtmlTargetElement("tbody")]
    public class TBodyHelper : ICTagHelper { }
    public class FormTagHelper : ICTagHelper { }
    public class ButtonTagHelper : ICTagHelper { }
    public class ATagHelper : ICTagHelper { }
    public class DivTagHelper : ICTagHelper { }
    public class TrTagHelper : ICTagHelper { }

    public class ICTagHelper : TagHelper
    {
        public string ICTarget { get; set; }
        public string ICPostTo { get; set; }
        public string ICGetFrom { get; set; }
        public string ICAppendFrom { get; set; }
        public string ICAction { get; set; }
        public string ICSrc { get; set; }
        public string ICActionTarget { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if(!(ICActionTarget is null))
                output.Attributes.SetAttribute("ic-action-target", ICAction);
            if(!(ICAction is null))
                output.Attributes.SetAttribute("ic-action", ICAction);
            if(!(ICTarget is null))
                output.Attributes.SetAttribute("ic-target", ICTarget);
            if(!(ICPostTo is null))
                output.Attributes.SetAttribute("ic-post-to", ICPostTo);
            if (!(ICGetFrom is null))
                output.Attributes.SetAttribute("ic-get-from", ICGetFrom);
            if (!(ICAppendFrom is null))
                output.Attributes.SetAttribute("ic-append-from", ICAppendFrom);
            if (!(ICSrc is null))
                output.Attributes.SetAttribute("ic-src", ICSrc);
            base.Process(context, output);
        }
    }
}
