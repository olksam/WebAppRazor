using System.Text;

using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebAppMvc.HtmlHelpers {
    public static class HtmlListHelpers {
        public static HtmlString ListFor(this IHtmlHelper helper, IEnumerable<object> items, object? htmlAttributes = null) {
            var sb = new StringBuilder();
            sb.Append("<ul>");
            foreach (var item in items) {
                sb.Append($"<li>{item}</li>");
            }
            sb.Append("</ul>");


            return new HtmlString(sb.ToString());
        }
    }
}
