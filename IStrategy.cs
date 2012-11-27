using System;
using System.Drawing;

namespace ImagePixelator
{
	public interface IStrategy
	{
		/// <summary>
		/// Generates the table.
		/// </summary>
		/// <param name='img'>
		/// Source image.
		/// </param>
		/// <param name='tbl'>
		/// Table to modify for output.
		/// </param>
		void generateTable(Image img, Table tbl);

		/// <summary>
		/// Gets the name of this strategy.
		/// </summary>
		/// <value>
		/// The name.
		/// </value>
		String Name {
			get;
		}
	}
}

