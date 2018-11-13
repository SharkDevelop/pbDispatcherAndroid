// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace pbDispatcher.iOS
{
    [Register ("CamController")]
    partial class CamController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ImageButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TimeLabel { get; set; }

        [Action ("ImageButtonTouch:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ImageButtonTouch (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (ImageButton != null) {
                ImageButton.Dispose ();
                ImageButton = null;
            }

            if (TimeLabel != null) {
                TimeLabel.Dispose ();
                TimeLabel = null;
            }
        }
    }
}