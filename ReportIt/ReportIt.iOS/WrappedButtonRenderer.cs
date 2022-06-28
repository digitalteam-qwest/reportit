using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: Xamarin.Forms.ExportRenderer(typeof(Button), typeof(ReportIt.iOS.WrappedButtonRenderer))]

// The following WrappedButtonRenderer custom renderer is included because the iOS Button
// implementation does not wrap or support multi-line text.
namespace ReportIt.iOS
{
    public class WrappedButtonRenderer : Xamarin.Forms.Platform.iOS.ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                if (e != null && e.NewElement != null && e.NewElement.Text != null)
                {
                    if (e.NewElement.Text.Contains(" "))
                    {
                        Control.TitleEdgeInsets = new UIEdgeInsets(4, 4, 4, 4);
                        Control.TitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
                        Control.TitleLabel.TextAlignment = UITextAlignment.Center;
                    }
                }
            }
        }
    }
}
