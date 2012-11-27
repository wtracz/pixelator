using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImagePixelator
{
	/// <summary>
	/// A filter for reducing an images dynamic range (on a per-channel basis).
	/// 
	/// This is not actually used.
	/// 
	/// </summary>
	public class DynamicRangeReduce : IFilter
	{

		public Image filter(Image img) {

			// Ensure format matches.
			if (img.PixelFormat != PixelFormat.Format24bppRgb) {
				throw new ArgumentException();
			}

			// Read the original data from sourceBitmap and output to destBitmap.
			Bitmap sourceBitmap = new Bitmap(img);
			Bitmap destBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, PixelFormat.Format24bppRgb);

			BitmapData sourceData = sourceBitmap.LockBits(
				new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
				ImageLockMode.ReadOnly,
				PixelFormat.Format24bppRgb
			);

			BitmapData destData = destBitmap.LockBits (
				new Rectangle(0, 0, destBitmap.Width, destBitmap.Height),
				ImageLockMode.WriteOnly,
				PixelFormat.Format24bppRgb
			);

			unsafe {

				// Get maximum value per R, G, B channel.
				byte[] m = new byte[] { 0, 0, 0 };

				byte* sourcePtr;
				sourcePtr = (byte*) sourceData.Scan0;

				for (int y = 0; y < sourceBitmap.Height; ++y) {
					for (int x = 0; x < sourceBitmap.Width; ++x) {

						for (byte c = 0; c < 3; ++c) {
							m[c] = sourcePtr[c] > m[c] ? sourcePtr[c] : m[c];

							// 3 byte per pixel.
							sourcePtr += 3;
						}
					}

					// Align to next row boundary.
					sourcePtr += sourceData.Stride - (sourceBitmap.Width * 3);
				}

				// Now scale all values by the relevant maximum.
				sourcePtr = (byte*) sourceData.Scan0;
				byte* destPtr = (byte*) destData.Scan0;

				for (int y = 0; y < sourceBitmap.Height; ++y) {
				
					for (int x = 0; x < sourceBitmap.Width; ++x) {

						for (byte c = 0; c < 3; ++c) {
							destPtr[c] = (byte) (Math.Log (1 + sourcePtr[c]) * 255 / Math.Log (1 + m[c]));
						}

						// 3 byte per pixel.
						sourcePtr += 3;
						destPtr += 3;
					}

					// Align to next row boundary.
					sourcePtr += sourceData.Stride - (sourceBitmap.Width * 3);
					destPtr += destData.Stride - (destBitmap.Width * 3);
				}
			}

			sourceBitmap.UnlockBits (sourceData);
			destBitmap.UnlockBits (destData);

			return destBitmap;
		}
	}
}

