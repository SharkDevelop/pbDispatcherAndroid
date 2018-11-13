using Foundation;
using System;
using UIKit;
using DataUtils;

namespace pbDispatcher.iOS
{
	public partial class CamController : UIViewController
	{
		UIImage image = null;
		DateTime lastTime;

		public CamController(IntPtr handle) : base(handle)
		{
		}

		//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		public override void ViewDidLoad()
		{
            DataManager.currentView = ViewTypes.Cam;

            lastTime = DataManager.nodes[DataManager.selectedSensor].lastOnlineTime;

			for (int s = 0; s < 10; s++)
			{
				if (ReloadFile() == true)
					break;
				
				lastTime = lastTime.AddMinutes(-1);
			}
		}

		//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
		public void UpdateView()
		{
			if (image != null)
			{
				if (UIScreen.MainScreen.Bounds.Width < UIScreen.MainScreen.Bounds.Height)
				{
					TimeLabel.Frame = new CoreGraphics.CGRect(TimeLabel.Frame.X, 62, TimeLabel.Frame.Width, TimeLabel.Frame.Height);
					ImageButton.Frame = new CoreGraphics.CGRect(ImageButton.Frame.X, 77, ImageButton.Frame.Width, ImageButton.Frame.Height);
				}
				else
				{
					TimeLabel.Frame = new CoreGraphics.CGRect(TimeLabel.Frame.X, 62 - 33, TimeLabel.Frame.Width, TimeLabel.Frame.Height);
					ImageButton.Frame = new CoreGraphics.CGRect(ImageButton.Frame.X, 77 - 35, ImageButton.Frame.Width, ImageButton.Frame.Height);
				}

				nfloat scaleKoeff = (nfloat)Math.Min(UIScreen.MainScreen.Bounds.Width / image.Size.Width, (UIScreen.MainScreen.Bounds.Height - ImageButton.Frame.Y) / image.Size.Height);
				Console.WriteLine("W: " + image.Size.Width.ToString() + "H: " + image.Size.Height.ToString());

				Console.WriteLine("scaleKoeff: " + scaleKoeff.ToString() + "H: " + (image.Size.Height * scaleKoeff).ToString());

				ImageButton.SetBackgroundImage(image.Scale(new CoreGraphics.CGSize(image.Size.Width * scaleKoeff, image.Size.Height * scaleKoeff)), UIControlState.Normal);
				ImageButton.SizeToFit();

				TimeLabel.Text = lastTime.ToString();
			}
		}

		//------------------------------------------------------------------------------------------------------------------------------------------------------------------
		public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate(fromInterfaceOrientation);

			UpdateView();
		}

		//------------------------------------------------------------------------------------------------------------------------------------------------------------------
		partial void ImageButtonTouch(UIButton sender)
		{
			lastTime = lastTime.AddMinutes(-1);

			ReloadFile();
		}

		private bool ReloadFile()
		{


			return true;
		}
	}
}