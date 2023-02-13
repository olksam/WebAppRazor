using System.Collections;

using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebAppRazor.TagHelpers;

[HtmlTargetElement("table-view", Attributes = "items, details-link-template")]
public class TableViewTagHelper : TagHelper {
    [HtmlAttributeName("items")]
    public ICollection Items { get; set; } = new List<object>();

    [HtmlAttributeName("details-link-template")]
    public string? DetailsLinkTemplate { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output) {
        if (Items.Count == 0) {
            output.Content.SetContent("No data");
            return;
        }
        output.TagName = "table";
        output.TagMode = TagMode.StartTagAndEndTag;

        var properties = Items
            .Cast<object>()
            .First()
            .GetType()
            .GetProperties()
            .Where(e => e.CanRead)
            .ToList();

        output.Content.AppendHtml("<thead><tr>");

        foreach (var prop in properties) {
            output.Content.AppendHtml($"<th>{prop.Name}</th>");
        }

        output.Content.AppendHtml("</tr></thead>");

        output.Content.AppendHtml("<tbody>");

        foreach (var item in Items) {
            output.Content.AppendHtml("<tr>");
            if (DetailsLinkTemplate != null) {

            }
            foreach (var prop in properties) {
                output.Content.AppendHtml($"<td>{prop.GetValue(item)}</td>");
            }
            if (DetailsLinkTemplate != null) {

            }
            output.Content.AppendHtml("</tr>");
        }

        output.Content.AppendHtml("</tbody>");
        output.Attributes.Clear();
    }
}