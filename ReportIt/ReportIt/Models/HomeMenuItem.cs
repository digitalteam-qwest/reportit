using System;
using System.Collections.Generic;
using System.Text;

namespace ReportIt.Models
{
    public enum MenuItemType
    {
        Map,
        Configuration,
        About
    }
    public class HomeMenuItem
    {
        public MenuItemType Id { get; set; }

        public string Title { get; set; }

        public override string ToString()
        {
            return Title;
        }
    }
}
