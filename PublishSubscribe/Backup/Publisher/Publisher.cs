using System;
using System.Collections;
using System.Data;
using Ticker;

namespace PublishSubscribe
{

	/// <summary>
	/// Class Publisher publishes the FakeTickerList and inherits from
	/// System.MarshalByRefObject so that subscribers can remotely access the class
	/// across process boundaries
	/// </summary>
	public class Publisher : System.MarshalByRefObject
	{

		/// <summary>
		/// An array list to hold the subscribed fake tickers
		/// </summary>
		public ArrayList FakeTickerList;

		/// <summary>
		/// Keep track of the number of subcriptions
		/// </summary>
		private int nNumSubscriptions;

		/// <summary>
		/// Get and set for the nmber of subscriptions
		/// </summary>
		public int NumSubscriptions
		{
			get
			{
				return nNumSubscriptions;
			}
			set
			{
				nNumSubscriptions = value;
			}
		}

		/// <summary>
		/// declare the definition for the function
		/// </summary>
		public delegate void PublishedTickerFunction( object SourceOfEvent, FakeTicker FakeTickerArg );

		/// <summary>
		/// declare the event to be triggered when a fake ticker is updated
		/// </summary>
		public event PublishedTickerFunction OnUpdatedFakeTicker;

		public void PublishNewEvents()
		{
			/// if the event is not null ( ie something has registered )
			/// publish the Faketicker list
			if( OnUpdatedFakeTicker != null )
			{
				foreach( FakeTicker fake in FakeTickerList )
				{
					/// give it a new value
					fake.Calculate();
					/// publish it
					OnUpdatedFakeTicker( this, fake ); 
				}
			}
		}

		public Publisher()
		{
			//
			// TODO: Add constructor logic here
			//

			FakeTickerList = new ArrayList();
		}

		/// <summary>
		/// Register a ticker with the Publish class
		/// </summary>
		/// <param name="ticker">The FakeTicker to be registered</param>
		public void RegisterFakeTicker( string strName, int nValue )
		{
			/// create a new faketicker
			FakeTicker Fake = new FakeTicker( strName, nValue );
			/// store it in the update list
			FakeTickerList.Add( Fake );

			nNumSubscriptions++;
		}
	}
}
