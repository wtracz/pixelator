using System;

namespace ImagePixelator
{
	public class StrategyAttribute : Attribute
	{

		private String mName;

		public String Name {
			get { return this.mName; }
		}

		private int mMinValue;

		public int MinimumValue {
			get { return this.mMinValue; }
		}

		public StrategyAttribute (String name, int minValue)
		{
			this.mName = name;
			this.mMinValue = minValue;
		}

	}
}

