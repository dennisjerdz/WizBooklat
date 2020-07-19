using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WizBooklat.Models
{
    public class ItemTypeConstants
    {
        public const sbyte BOOK = 0;
        public const sbyte OTHER_RESOURCE = 1;

        public List<SelectListItem> ItemTypeList = new List<SelectListItem>
        {
            new SelectListItem{ Text = "Book", Value = "0"},
            new SelectListItem{ Text = "Other Resource", Value = "1"}
        };
    }

    public class BookStatusConstant
    {
        public const sbyte AVAILABLE = 0;
        public const sbyte DAMAGED_AVAILABLE = 1;
        public const sbyte RESERVED = 2;
        public const sbyte PULLED_OUT = 3;
        public const sbyte REPORTED_LOST = 4;
        public const sbyte CONFIRMED_LOST = 5;

        public List<SelectListItem> BookStatusList = new List<SelectListItem>
        {
            new SelectListItem{ Text = "Available", Value = "0"},
            new SelectListItem{ Text = "Available - Damaged", Value = "1"},
            new SelectListItem{ Text = "Reserved", Value = "2"},
            new SelectListItem{ Text = "Pulled-out", Value = "3"},
            new SelectListItem{ Text = "Reported Lost", Value = "4"},
            new SelectListItem{ Text = "Confirmed Lost", Value = "5"},
        };
    }

    public class PointTypeConstant
    {
        public const sbyte MINUS = 0;
        public const sbyte ADD = 1;
    }
}