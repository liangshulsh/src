using System;
using System.Collections;
using Ticker;

namespace PublishSubscribe
{
	/// <summary>
	/// Subscriber data needs to be a public class so that it can be accessed
	/// by the SubscribeClient class
	/// </summary>
	public class SubscriberData
	{
		private string strName;

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

		private int nValue;

		public int Value
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

		public SubscriberData()
		{
			Name = "";
			Value = 0;
		}
	}


	/// <summary>
	/// The subscriber callback function is placed in a dll so that the publisher can
	/// reference the subscriber and accept the class for callbacks.
	/// </summary>
	
	public class Subscriber : MarshalByRefObject
	{
		/// <summary>
		/// Declare an arraylist to hold the updated information
		/// </summary>
		private ArrayList DataList;

		/// <summary>
		/// Get the data list and put its data in the list box
		/// </summary>
		public ArrayList GetDataList
		{
			get
			{
				return DataList;
			}
		}

		/// <summary>
		/// Set a boolean to true if there has been an update
		/// </summary>
		private bool bUpdated;

		/// <summary>
		/// get and set for bUpdated
		/// </summary>
		public bool IsUpdated
		{
			get
			{
				return bUpdated;
			}
			set
			{
				bUpdated = value;
			}
		}
 
		public Subscriber()
		{
			//
			// TODO: Add constructor logic here
			//

			DataList = new ArrayList();
		}

		/// <summary>
		/// Callback function called when the publisher update is triggered
		/// </summary>
		public void OnSubscriberUpdate( object SourceOfEvent, FakeTicker fakeTickerArg )
		{
			bool bFound = false;
			SubscriberData temp;
			for( int i=0; i<DataList.Count; i++ )
			{
				temp = ( SubscriberData )DataList[ i ];

				if( temp.Name == fakeTickerArg.Name )
				{
					temp.Value = fakeTickerArg.Value;
					bFound = true;
				}
			}

			if( bFound == false )
			{
				SubscriberData data = new SubscriberData();
				data.Name = fakeTickerArg.Name;
				data.Value = fakeTickerArg.Value;

				DataList.Add( data );
			}

			IsUpdated = true;

		}
	}
}
