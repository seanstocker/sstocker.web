using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;

namespace sstocker.core.Helpers
{
    public static class EnumHelper
    {
        public static SelectList ToSelectList<TEnum>(this TEnum obj, Enum selected)
            where TEnum : struct, IComparable, IFormattable, IConvertible
        {
            var result = new SelectList(Enum.GetValues(typeof(TEnum)).OfType<Enum>()
                .Select(x =>
                    new SelectListItem
                    {
                        Text = Enum.GetName(typeof(TEnum), x),
                        Value = (Convert.ToInt32(x)).ToString(),
                        Selected = Enum.GetName(typeof(TEnum), x) == Enum.GetName(typeof(TEnum), selected)
                    }), "Value", "Text", new SelectListItem(Convert.ToInt32(selected).ToString(), Enum.GetName(typeof(TEnum), selected), true));

            return result;
        }
    }
}
