using System;
using System.Drawing;

namespace ImagePixelator
{
	/// <summary>
	/// Quick interface for image filters.
	/// </summary>
	public interface IFilter
	{
		/// <summary>
		/// Take an image and apply some operation to it, with the result returned.
		/// </summary>
		/// <param name='img'>
		/// Source image.
		/// </param>
		Image filter(Image img);
	}
}

