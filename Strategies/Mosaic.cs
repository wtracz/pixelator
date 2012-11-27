using System;
using System.Drawing;

using ImagePixelator;

namespace ImagePixelator.Strategies
{
	public class Mosaic : IStrategy
	{
		private int mSize;

		public int Size {
			get { return this.mSize; }
			set { 

				if (value < 0) {
					throw new ArgumentOutOfRangeException();
				}

				this.mSize = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImagePixelator.Mosaic"/> class.
		/// </summary>
		/// <param name='size'>
		/// The block size to use for compression.
		/// </param>
		public Mosaic (int size)
		{
			this.Size = size;
		}

		public Mosaic() {
			this.Size = 16;
		}

		public void generateTable(Image img, Table tbl) {
		
			// Downsample image.
			int newHeight = (img.Size.Height / this.mSize) + 1;
			int newWidth = (img.Size.Width / this.mSize) + 1;

			Bitmap newImg = new Bitmap(newWidth, newHeight);

			using (Graphics g = Graphics.FromImage (newImg))
			{
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
				g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

				g.DrawImage(img, new Rectangle(0, 0, newWidth, newHeight));

			}
			
			// Now iterate over down-sampled image to generate mosaic.
			for (int y = 0; y < newHeight; ++y) {

				TableRow tbr = new TableRow();
				tbl.Rows.Add (tbr);

				for(int x = 0; x < newWidth; ++x) {
					TableData tbd = new TableData();

					tbd.BackgroundColour = newImg.GetPixel (x, y);
					tbd.Height = this.Size;
					tbd.Width = this.Size;
					tbd.ChildElement = null;

					tbr.Cells.Add(tbd);
				}
			}
		}

		public String Name {
			get {
				return "Mosaic";
			}
		}
	}
}

