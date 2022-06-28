using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.ExportRenderer(typeof(TextCell), typeof(ReportIt.iOS.TextCellCustomRenderer))]
[assembly: Xamarin.Forms.ExportRenderer(typeof(ViewCell), typeof(ReportIt.iOS.ViewCellCustomRenderer))]

// The following ListView.TextCell custom renderer is included because the iOS ListView
// implementation does not display correctly.
namespace ReportIt.iOS
{
    public class TextCellCustomRenderer : Xamarin.Forms.Platform.iOS.TextCellRenderer
    {
        public override UITableViewCell GetCell(Xamarin.Forms.Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            return cell;
        }
    }
}

// The following ListView.ViewCell custom renderer is included because the iOS ListView
// implementation does not display correctly.
namespace ReportIt.iOS
{
    public class ViewCellCustomRenderer : Xamarin.Forms.Platform.iOS.ViewCellRenderer
    {
        public override UITableViewCell GetCell(Xamarin.Forms.Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            return cell;
        }
    }
}