using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SFA.DAS.FAT.Web.TagHelpers;

[ExcludeFromCodeCoverage]
[HtmlTargetElement("a", Attributes = "asp-querystring")]
public class ExtendedAnchorTagHelper : AnchorTagHelper
{
    [HtmlAttributeName("asp-querystring")]
    public string QueryString { get; set; }


    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var hasHref = output.Attributes.TryGetAttribute("href", out var attribute);

        var hasRouteLike = context.AllAttributes.Any(a =>
            a.Name == "asp-action" ||
            a.Name == "asp-controller" ||
            a.Name == "asp-area" ||
            a.Name == "asp-route" ||
            a.Name.StartsWith("asp-route-") ||
            a.Name == "asp-protocol" ||
            a.Name == "asp-host" ||
            a.Name == "asp-fragment" ||
            a.Name == "asp-page" ||
            a.Name == "asp-page-handler");

        if (!(hasHref && hasRouteLike))
        {
            base.Process(context, output);
            output.Attributes.TryGetAttribute("href", out attribute);
        }

        var href = attribute?.Value?.ToString() ?? string.Empty;
        var qs = QueryString ?? string.Empty;

        if (string.IsNullOrEmpty(qs))
        {
            return;
        }

        if (!string.IsNullOrEmpty(href) && href.Contains('?'))
        {
            if (qs.StartsWith('?'))
            {
                qs = string.Concat("&", qs.AsSpan(1));
            }
        }
        else
        {
            if (qs.StartsWith('&'))
            {
                qs = string.Concat("?", qs.AsSpan(1));
            }
        }

        output.Attributes.SetAttribute("href", href + qs);
    }

    public ExtendedAnchorTagHelper(IHtmlGenerator generator) : base(generator)
    {
    }
}
