using System;
using System.Drawing;

namespace ImagePixelator
{
	/// <summary>
	/// Ensures all input images are converted to 24 bits per pixel format.
	/// 
	/// This allows the unsafe code in the strategies to work reliably.
	/// </summary>
	/// 
	public class BMPConvert : IFilter
	{
		public Image filter(Image img) {

			// Create a new 24 bit per pixel image of same dimensions as the input.
			Bitmap newImg = new Bitmap(
				img.Width,
				img.Height,
				System.Drawing.Imaging.PixelFormat.Format24bppRgb
			);

			// Draw the input straight to it without scaling.
			using (Graphics g = Graphics.FromImage (newImg)) {
				g.DrawImageUnscaled (img, 0, 0);
			}

			return newImg;
		}
	}
}

