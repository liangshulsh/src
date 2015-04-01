using System;
using System.Collections;

namespace Ticker
{
	/// <summary>
	///  FakeTicker class.
	/// </summary>
	public class FakeTicker : EventArgs
	{
		private string strName;
		private int nValue;
		private Random rand;

		/// <summary>
		/// The name of this ticker
		/// </summary>
		public string Name
		{
			get
			{
				return strName;
			}
			set
			{
				strName = value;
			}
		}

		/// <summary>
		///  The current value for this ticker
		/// </summary>
		private int Value
		{
			get
			{
				return nValue;
			}
			set
			{
				nValue = value;
			}
		}

		/// <summary>
		/// block the standard constructor
		/// </summary>
		private FakeTicker()
		{
		}

		/// <summary>
		/// public constructor
		/// </summary>
		/// <param name="sName">The Name for this FakeTicker</param>
		/// <param name="nCount">The Starting parameter for this FakeTicker</param>
		public FakeTicker( string sName, int nSeed )
		{
			rand = new Random( nSeed );
			Value = rand.Next();
			Name = sName;
		}


		/// <summary>
		///  Calculate the value for this ticker
		/// </summary>
		/// <returns>The next random value for this ticker</returns>
		public int Calculate()
		{
			return Value = rand.Next();
		}

	}

}
