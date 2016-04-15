using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Epam_MVC4.Extensions
{
    public static class Extensions
    {


        public static HtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> html, Expression<Func<TModel, TEnum>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);

            var enumType = Nullable.GetUnderlyingType(metadata.ModelType) ?? metadata.ModelType;

            var enumValues = Enum.GetValues(enumType).Cast<object>();

            var items = from enumValue in enumValues
                        select new SelectListItem
                        {
                            Text = enumValue.ToString(),
                            Value = ((int)enumValue).ToString(),
                            Selected = enumValue.Equals(metadata.Model)
                        };

            return html.DropDownListFor(expression, items, string.Empty, null);
        }

        ////<button type = "button" class="btn btn-default @active" onclick="location.href='@Url.Action("UpdateProvider")'">@provider.Name</button>
        ///*<label class="btn btn-primary ">
        //                   <input id="@provider.Id" name="provider" value="yahoo" type="radio"> yahoo
        //               </label>*/

        ///// <summary>
        ///// Return a radio button <label class=""><input type="radio">label</label> construction
        ///// </summary>
        ///// <param name="html"></param>
        ///// <param name="name">Common name for radio button</param>
        ///// <param name="value">Value of radio button</param>
        ///// <param name="pageUrl">Page url of required action</param>
        ///// <param name="label">Label inside radio button</param>
        ///// <param name="active">Is radio checked?</param>
        ///// <param name="id">Id of the input</param>
        ///// <param name="parameters">You can pass any add parameters for input and class for lable</param>
        ///// <returns></returns>
        //public static MvcHtmlString e_RadioButton( this HtmlHelper html, 
        //                                        string name, int value, Func<int, string> pageUrl, string label = "", bool active = false, string id = "", object parameters = null)
        //{
        //    StringBuilder builder = new StringBuilder();

        //    var tLabel = new TagBuilder("label");
        //    var tAnchor = new TagBuilder("a");
        //    var tInput = new TagBuilder("input");

        //    string class_descr = "";
        //    var _class = parameters.GetType().GetProperty("class");
        //    if ( _class != null)
        //    {
        //        class_descr = _class.GetValue(parameters).ToString();
        //    }
        //    if (active)
        //    {
        //        class_descr += " active";
        //        tInput.MergeAttribute("checked", "checked");
        //    }
        //    if (class_descr != "")
        //    {
        //        tLabel.MergeAttribute("class", class_descr);
        //        //tAnchor.MergeAttribute("class", class_descr);
        //    };

        //    if (id != "") tInput.MergeAttribute("id", id);
        //    tInput.MergeAttribute("type", "radio");
        //    tInput.MergeAttribute("name", name);
        //    tInput.MergeAttribute("value", value.ToString());
        //    tInput.MergeAttribute("checked", (active ? "checked" : ""));
        //    tAnchor.MergeAttribute("href", pageUrl.Invoke(value));

        //    tAnchor.InnerHtml = tInput.ToString(TagRenderMode.StartTag) + label;
        //    tLabel.InnerHtml = tAnchor.ToString();

        //    var properties = parameters.GetType().GetProperties();
        //    foreach (var property in properties)
        //    {
        //        if (property.Name == "class" || property.Name == "id") continue;
        //        tAnchor.MergeAttribute(property.Name, property.GetValue(parameters).ToString());
        //    }

        //    builder.Append(tAnchor.ToString());

        //    return new MvcHtmlString(builder.ToString());
        //}
    }
}