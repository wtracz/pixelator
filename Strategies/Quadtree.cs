using System;
using System.Drawing;
using System.Drawing.Imaging;

using ImagePixelator;

namespace ImagePixelator.Strategies
{
	public class Quadtree : IStrategy
	{
		private int mMinSize;

		public int MinimumBlockSize {
			get { return this.mMinSize; }
			set {
			
				if (value < 1) {
					throw new ArgumentOutOfRangeException();
				}

				this.mMinSize = value;
			}
		}

		private int mThreshold;

		public int Threshold {
			get { return this.mThreshold; }
			set {
				this.mThreshold = value;
			}
		}

		public Quadtree (int minSize, int threshold)
		{
			this.MinimumBlockSize = minSize;
			this.Threshold = threshold;
		}

		public Quadtree() {
			this.MinimumBlockSize = 8;
			this.Threshold = 30;
		}

		private Tree BuildQuadtree(Image img) {
			Bitmap sourceBitmap = new Bitmap(img);

			BitmapData sourceData = sourceBitmap.LockBits(
				new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
				ImageLockMode.ReadOnly,
				PixelFormat.Format24bppRgb
			);

			Tree tree =  new Tree(sourceData, MinimumBlockSize, Threshold);

			sourceBitmap.UnlockBits(sourceData);

			return tree;
		}

		private void recursivelyGenerateTable(Node current, TableData tbd) {
		
			if (current.IsLeaf == false) {

				// If not a leaf, introduce a nested table.
				Table tbl = new Table();
				tbl.BackgroundColour = tbd.BackgroundColour;
				tbd.ChildElement = tbl;

				// Add children to this nested table in NW, NE, SW, SE order.
				TableRow tbr = null;
				for (int i = 0; i < 4; ++i) {

					if (i % 2 == 0) {
						tbr = new TableRow();
						tbl.Rows.Add(tbr);
					}
				
					TableData tbdd = new TableData();
					tbr.Cells.Add(tbdd);

					recursivelyGenerateTable(current.ChildNodes[i], tbdd);

				}

			}
			else {

				// If a leaf, simply set the colour on the current element.
				tbd.BackgroundColour = current.Colour;
				tbd.Height = current.Height;
				tbd.Width = current.Width;
				tbd.ColumnSpan = 1;
			}

		}

		public void generateTable(Image img, Table tbl) {

			Tree tree = BuildQuadtree (img);

			// Create initial row and cell.
			TableRow tbr = new TableRow();
			TableData tbd = new TableData();
			tbr.Cells.Add(tbd);
			tbl.Rows.Add(tbr);

			// Generate into cell.
			recursivelyGenerateTable(tree.RootNode, tbd);

		}

		public String Name {
			get {
				return "Quadtree";
			}
		}
	}
}

