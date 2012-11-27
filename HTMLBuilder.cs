using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;

namespace ImagePixelator
{

	public interface IHTMLElementVisitor {
		void visit(Table element);
		void visit(TableRow element);
		void visit(TableData element);
	}

	/// <summary>
	/// Visitor for generating HTML for visited elements.
	/// </summary>
	public class HTMLBuilder : IHTMLElementVisitor {

		private StringBuilder mStringBuilder;

		private Color mCurrentBackground;

		public String HTML {
			get { return mStringBuilder.ToString (); }
		}

		public void visit(TableData element) {
		
			// Opening tag.
			mStringBuilder.Append("<td");

			// Add colour if required.
			Color original = this.mCurrentBackground;
			if (element.BackgroundColour.Equals (this.mCurrentBackground) == false) {
				mStringBuilder.AppendFormat(" bgcolor={0}", ColorTranslator.ToHtml(element.BackgroundColour));

				this.mCurrentBackground = element.BackgroundColour;
			}

			// If no nested tag add width and height.
			if (element.ChildElement == null) {
				mStringBuilder.AppendFormat(" width={0} height={1}", element.Width, element.Height);
			}

			if (element.ColumnSpan > 1) {
				mStringBuilder.AppendFormat (" colspan={0}", element.ColumnSpan);
			}

			mStringBuilder.Append (">");

			// If this is a nested tag, recurse.
			if (element.ChildElement != null) {
				element.ChildElement.accept(this);
			}
			 
			// Closing tag.
			mStringBuilder.Append ( "</td>");

			// Reset background colour.
			this.mCurrentBackground = original;
		}

		public void visit(TableRow element) {
		
			// Now have a straight set of column runs.
			mStringBuilder.Append ("<tr>");

			foreach(TableData cell in element.Cells) {
				cell.accept (this);
			}

			mStringBuilder.Append ("</tr>");
		}

		public void visit(Table element) {

			// Opening tag.
			mStringBuilder.Append ( "<table cellpadding=0 cellspacing=0");

			// Add colour if required.
			Color original = this.mCurrentBackground;
			if (element.BackgroundColour.Equals (this.mCurrentBackground) == false) {
				mStringBuilder.AppendFormat ( " bgcolor={0}",ColorTranslator.ToHtml(element.BackgroundColour));

				this.mCurrentBackground = element.BackgroundColour;
			}

			mStringBuilder .Append ( ">");

			// Generate row HTML.
			foreach(TableRow row in element.Rows) {
				row.accept (this);
			}

			// Closing tag.
			mStringBuilder.Append ("</table>");

			// Reset background colour.
			this.mCurrentBackground = original;
		}

		public HTMLBuilder() {
			this.mStringBuilder = new StringBuilder();
		}

	}

	/// <summary>
	/// Base HTML table element.
	/// </summary>
	public interface IHTMLElement {
		void accept(IHTMLElementVisitor visitor);
	}

	/// <summary>
	/// Table cell class.
	/// </summary>
	public class TableData : IHTMLElement {

		private Color mBackgroundColour;

		private IHTMLElement mChildElement;

		private int mHeight;

		private int mWidth;

		private int mColumnSpan;

		public IHTMLElement ChildElement {
			get { return this.mChildElement; }
			set { this.mChildElement = value; }
		}

		public Color BackgroundColour {
			get { return this.mBackgroundColour; }
			set { this.mBackgroundColour = value; }
		}

		public int Height {
			get { return this.mHeight; }
			set {
			
				if (value < 1) {
					throw new Exception();
				}

				this.mHeight = value;
			}
		}
		
		public int Width {
			get { return this.mWidth; }
			set {
			
				if (value < 1) {
					throw new Exception();
				}

				this.mWidth = value;
			}
		}
		
		public int ColumnSpan {
			get { return this.mColumnSpan; }
			set {
			
				if (value < 1) {
					throw new Exception();
				}

				this.mColumnSpan = value;
			}
		}


		public void accept(IHTMLElementVisitor visitor) {
			visitor.visit (this);
		}

	}

	/// <summary>
	/// Table row class.
	/// </summary>
	public class TableRow : IHTMLElement {

		private List<TableData> mCells;

		public List<TableData> Cells {
			get { return this.mCells; }
			set { this.mCells = value; } 
		}

		public TableRow() {
			mCells = new List<TableData>();
		}

		public void accept(IHTMLElementVisitor visitor) {
			visitor.visit (this);
		}
	}

	/// <summary>
	/// Table class.
	/// </summary>
	public class Table : IHTMLElement {

		private List<TableRow> mRows;

		public List<TableRow> Rows {
			get { return this.mRows; }
			set { this.mRows = value; } 
		}

		public Table() {
			this.mRows = new List<TableRow>();
		}

		private Color mBackgroundColour;

		public Color BackgroundColour {
			get { return this.mBackgroundColour; }
			set { this.mBackgroundColour = value; }
		}
		
		public void accept(IHTMLElementVisitor visitor) {
			visitor.visit (this);
		}
	}

}

