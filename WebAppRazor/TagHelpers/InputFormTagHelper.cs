using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebAppRazor.TagHelpers;

public enum InputFormMethod {
    Get,
    Post
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class FormIgnoreAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class FormInputAttribute : Attribute {
    public string? Name { get; set; }
    public string? Placeholder { get; set; }
    public string? Tooltip { get; set; }
}


[HtmlTargetElement("input-form", Attributes = "model, action-url, method")]
public class InputFormTagHelper : TagHelper {
    [HtmlAttributeName("model")]
    public Type? ModelType { get; set; }

    [HtmlAttributeName("action-url")]
    public string Action { get; set; } = "/";

    [HtmlAttributeName("method")]
    public InputFormMethod Method { get; set; } = InputFormMethod.Post;

    public override void Process(TagHelperContext context, TagHelperOutput output) {
        output.TagName = "div";

        if (ModelType is null) {
            output.Content.SetContent("No data");
            return;
        }

        output.TagMode = TagMode.StartTagAndEndTag;

        var properties = GetWriteableProperties(ModelType);

        BeginForm(output.Content);

        foreach (var prop in properties) {
            if (IsString(prop.PropertyType)) {
                AppendTextInput(output.Content, prop);
            } else if (IsChar(prop.PropertyType)) {
                AppendCharInput(output.Content, prop);
            } else if (IsInteger(prop.PropertyType)) {
                AppendIntegerInput(output.Content, prop);
            } else if (IsFloat(prop.PropertyType)) {
                AppendFloatInput(output.Content, prop);
            } else if (IsBool(prop.PropertyType)) {
                AppendBoolInput(output.Content, prop);
            }
        }

        EndForm(output.Content);

        output.Attributes.Clear();
    }

    private static ICollection<PropertyInfo> GetWriteableProperties(Type type) =>
        type.GetProperties()
            .Where(e => e.CanWrite && e.GetCustomAttribute<FormIgnoreAttribute>() is null)
            .ToList();


    private void BeginForm(TagHelperContent content) {
        content.AppendHtml(
            $"""
                <form method="{Method}" action="{Action}">
                """);
    }

    private void EndForm(TagHelperContent content) {
        content.AppendHtml(
            """
                    <div>
                        <input type="submit" value="save"></input>
                    </div>
                </form>
                """);
    }

    private void AppendTextInput(TagHelperContent content, PropertyInfo prop) {
        var name = GetName(prop);
        var required = GetRequired(prop);

        content.AppendHtml(
            $"""
                    <div>   
                        <label for="{name}">{name}:</label><br>
                        <input type="text" id="{name}" name="{name}" {(required ? "required" : "")}>
                    </div>
                """);
    }

    private void AppendIntegerInput(TagHelperContent content, PropertyInfo prop) {
        var name = GetName(prop);
        var (min, max) = GetIntRange(prop);
        var required = GetRequired(prop);

        content.AppendHtml(
            $"""
                    <div>
                        <label for="{name}">{name}:</label><br>
                        <input type="number" id="{name}" name="{name}" step="1" min="{min}" max="{max}" {(required ? "required" : "")}>
                    </div>
                """);
    }

    private void AppendFloatInput(TagHelperContent content, PropertyInfo prop) {
        var name = GetName(prop);
        var (min, max) = GetFloatRange(prop);
        var required = GetRequired(prop);

        content.AppendHtml(
            $"""
                    <div>
                        <label for="{name}">{name}:</label><br>
                        <input type="number" id="{name}" name="{name}" min="{min}" max="{max}" {(required ? "required" : "")}>
                    </div>
                """);
    }

    private void AppendCharInput(TagHelperContent content, PropertyInfo prop) {
        var name = GetName(prop);
        var required = GetRequired(prop);

        content.AppendHtml(
            $"""
                    <div>
                        <label for="{name}">{name}:</label><br>
                        <input type="text" id="{name}" name="{name}" {(required ? "minlength=\"1\" required" : "")} maxlength="1">
                    </div>
                """);
    }

    private void AppendBoolInput(TagHelperContent content, PropertyInfo prop) {
        var name = GetName(prop);

        content.AppendHtml(
            $"""
                <div>
                    <input type="checkbox" id="{name}" name="{name}">
                    <label for="{name}">{name}</label>
                </div>
                """);
    }

    private static bool IsString(Type type) => type == typeof(string);

    private static bool IsChar(Type type) => type == typeof(char) || type == typeof(char?);

    private static bool IsInteger(Type type) =>
        type == typeof(byte) || type == typeof(sbyte) || type == typeof(byte?) || type == typeof(sbyte?)
        || type == typeof(ushort) || type == typeof(short) || type == typeof(ushort?) || type == typeof(short?)
        || type == typeof(uint) || type == typeof(int) || type == typeof(uint?) || type == typeof(int?)
        || type == typeof(ulong) || type == typeof(long) || type == typeof(ulong?) || type == typeof(long?);

    private static bool IsFloat(Type type) =>
        type == typeof(float) || type == typeof(float?)
        || type == typeof(double) || type == typeof(double?)
        || type == typeof(decimal) || type == typeof(decimal?);

    private static bool IsBool(Type type) => type == typeof(bool) || type == typeof(bool?);

    private static string GetName(PropertyInfo prop) =>
        prop.GetCustomAttribute<FormInputAttribute>()?.Name ?? prop.Name;

    private static string? GetToolTip(PropertyInfo prop) =>
        prop.GetCustomAttribute<FormInputAttribute>()?.Name ?? prop.Name;

    private static string? GetPlaceholder(PropertyInfo prop) =>
        prop.GetCustomAttribute<FormInputAttribute>()?.Name ?? prop.Name;

    private static bool GetRequired(PropertyInfo prop) =>
        prop.GetCustomAttribute<RequiredAttribute>() is not null;

    private static (long Min, long Max) GetIntRange(PropertyInfo prop) {
        var rangeAttribute = prop.GetCustomAttribute<RangeAttribute>();

        long min;
        long max;

        if (rangeAttribute is not null) {
            min = Convert.ToInt64(rangeAttribute.Minimum);
            max = Convert.ToInt64(rangeAttribute.Maximum);
        } else {
            min = Convert.ToInt64(prop.PropertyType.GetField("MinValue").GetValue(null));
            max = Convert.ToInt64(prop.PropertyType.GetField("MaxValue").GetValue(null));
        }

        return (min, max);
    }

    private static (double Min, double Max) GetFloatRange(PropertyInfo prop) {
        var rangeAttribute = prop.GetCustomAttribute<RangeAttribute>();

        double min;
        double max;

        if (rangeAttribute is not null) {
            min = Convert.ToDouble(rangeAttribute.Minimum);
            max = Convert.ToDouble(rangeAttribute.Maximum);
        } else {
            min = Convert.ToDouble(prop.PropertyType.GetField("MinValue").GetValue(null));
            max = Convert.ToDouble(prop.PropertyType.GetField("MaxValue").GetValue(null));
        }

        return (min, max);
    }
}





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