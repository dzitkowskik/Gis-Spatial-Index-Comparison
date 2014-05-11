namespace SpatialIndexesComparison.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public static class HtmlExtensions
    {
        public static IEnumerable<SelectListItem> EnumDropdown(this HtmlHelper helper, Type enumType)
        {
            var result = new List<SelectListItem>();
            foreach (var value in Enum.GetValues(enumType))
            {
                result.Add(new SelectListItem { Text = value.ToString(), Value = ((int)value).ToString() });                
            }
            return result;
        }
    }
}