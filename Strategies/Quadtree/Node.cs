using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImagePixelator
{
	public class Tree
	{
		private Node mRootNode;

		private int mMinSize;

		private int mThreshold;

		private BitmapData mImage;

		public int MinimumBlockSize {
			get { return this.mMinSize; }
		}

		public int Threshold {
			get { return this.mThreshold; }
		}

		public BitmapData BitmapData {
			get { return this.mImage; }
		}

		public int NestedCount {
			get { return this.mRootNode.NestedCount; }
		}

		public Node RootNode {
			get { return this.mRootNode; }
		}

		public Tree(BitmapData img, int minSize, int threshold) {

			if (minSize < 1) {
				throw new ArgumentOutOfRangeException();
			}

			this.mMinSize = minSize;
			this.mImage = img;
			this.mThreshold = threshold;

			this.mRootNode = new Node(this, 0, 0, img.Width, img.Height);
		}

		/// <summary>
		/// Gets the average colour for this noe from the image.
		/// </summary>
		/// <returns>
		/// The colour for node.
		/// </returns>
		/// <param name='node'>
		/// Node.
		/// </param>
		public Color GetColourForNode(Node node) {

			int tR = 0, tG = 0, tB = 0;

			unsafe {
				byte* imagePtr = (byte*) (mImage.Scan0 + (mImage.Stride * node.Y) + (node.X * 3));

				for (int i = 0; i < node.Height; ++i) {
				
					for (int j = 0; j < node.Width; ++j) {

						// Bitmap data held in BGR.
						tR += imagePtr[2]; tG += imagePtr[1]; tB += imagePtr[0];

						imagePtr += 3;
					}

					imagePtr += mImage.Stride - (node.Width * 3);
				}
			}

			tR /= node.Area; tG /= node.Area; tB /= node.Area;

			return Color.FromArgb(tR, tG, tB);
		}

		public bool IsNodeSplitRequired(Node node) {

			// Minimum width check.
			if (node.Width / 2 < mMinSize) {
				return false;
			}

			// Minimum height check.
			if (node.Height / 2 < mMinSize) {
				return false;
			}

			// Variance check.
			float variance = 0;

			unsafe {
				byte* imagePtr;

				// Calculate node average per channel.
				imagePtr = (byte*) (mImage.Scan0 + (mImage.Stride * node.Y) + (node.X * 3));

				float tR = 0, tG = 0, tB = 0;
				for (int i = 0; i < node.Height; ++i) {
				
					for (int j = 0; j < node.Width; ++j) {

						// Bitmap data held in BGR.
						tR += imagePtr[2]; tG += imagePtr[1]; tB += imagePtr[0];

						imagePtr += 3;
					}

					imagePtr += mImage.Stride - (node.Width * 3);
				}

				tR /= node.Area; tG /= node.Area; tB /= node.Area;

				// Now calculate variance over all channels.
				imagePtr = (byte*) (mImage.Scan0 + (mImage.Stride * node.Y) + (node.X * 3));

				for (int i = 0; i < node.Height; ++i) {
				
					for (int j = 0; j < node.Width; ++j) {

						// Bitmap data held in BGR.
						float cR = imagePtr[2] - tR;
						float cG = imagePtr[1] - tG;
						float cB = imagePtr[0] - tB;

						variance += (cR * cR) + (cG * cG) + (cB * cB);

						imagePtr += 3;
					}

					imagePtr += mImage.Stride - (node.Width * 3);
				}
			}

			// If avg. variance is over threshold (multiplied by 3 due to 3 channels) split.
			if (variance >= mThreshold * mThreshold * 3 * node.Area) {
				return true;
			}

			return false;
		}
	}

	public class Node
	{
		static byte NorthWest = 0, NorthEast = 1, SouthWest = 2, SouthEast = 3;

		private int mX, mY, mWidth, mHeight;

		private Node[] mChildNodes;

		private Color mColour;

		public Node[] ChildNodes {
			get { return this.mChildNodes; }
		}

		public Color Colour {
			get {
				if (IsLeaf == false) {
					throw new Exception();
				}

				return mColour;
			}
		}

		public bool IsLeaf {
			get {
				return mChildNodes == null;
			}
		}

		public int Area {
			get{
				return mWidth * mHeight;
			}
		}

		public int NestedCount {
			get {
				int count = 1;

				if (this.IsLeaf) {
					return count;
				}
			
				for (int i = 0; i < 4; ++i) {
					count += mChildNodes[i].NestedCount;
				}

				return count;
			}
		}

		public int X {
			get {
				return mX;
			}
		}

		public int Y {
			get {
				return mY;
			}
		}

		public int Width {
			get {
				return mWidth;
			}
		}

		public int Height {
			get {
				return mHeight;
			}
		}
		public Color GetPixel(int i, int j) {
		
			if (this.IsLeaf == true) {
				return mColour;
			}
			else {
				if (i < mX + mWidth / 2) {
				
					if (j < mY + mHeight / 2) {
						return mChildNodes[Node.NorthWest].GetPixel(i, j);
					}
					else {
						return mChildNodes[Node.SouthWest].GetPixel (i, j);
					}
				
				}
				else {
				
					if (j < mY + mHeight / 2) {
						return mChildNodes[Node.NorthEast].GetPixel(i, j);
					}
					else {
						return mChildNodes[Node.SouthEast].GetPixel (i, j);
					}
				
				}
			}
		}

		public Node (Tree tree, int x, int y, int width, int height)
		{
			if (width < 1 || height < 1) {
				throw new ArgumentOutOfRangeException();
			}

			this.mX = x; this.mY = y;
			this.mWidth = width; this.mHeight = height;

			// Test if this node requires splitting.
			if (tree.IsNodeSplitRequired(this) == false) {

				// Assign colour - this is a leaf.
				mColour = tree.GetColourForNode(this);
			}
			else {
			
				// Split into four (half width, half height) sections.
				int newWidth = width / 2;
				int newHeight = height / 2;

				mChildNodes = new Node[4] { null, null, null, null };

				Node childNW = new Node(tree, x, y, newWidth, newHeight);
				mChildNodes[Node.NorthWest] = childNW;

				Node childSW = new Node(tree, x, y + newHeight, newWidth, newHeight);
				mChildNodes[Node.SouthWest] = childSW;

				Node childSE = new Node(tree, x + newWidth, y + newHeight, newWidth, newHeight);
				mChildNodes[Node.SouthEast] = childSE;

				Node childNE = new Node(tree, x + newWidth, y, newWidth, newHeight);
				mChildNodes[Node.NorthEast] = childNE;
			}

		}
	}
}

