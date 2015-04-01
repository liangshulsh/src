using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using PublishSubscribe;
using Ticker;

/// usings for remoting
/// need to add the System.Runtime.Remoting reference
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace PublishSubscribe
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class SubscribeClient : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox SubscribeTo;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ListView listViewSubscriptions;
		private System.Windows.Forms.ColumnHeader Subscriptions;
		private System.Windows.Forms.ColumnHeader Value;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.NumericUpDown InitialValue;

		/// <summary>
		/// Maintain a publsher object so that the callbacks can be set up
		/// </summary>
		private Publisher NewPublisher;

		/// <summary>
		/// Create an instance of the subscriber dll
		/// </summary>
		private static Subscriber NewSubscriber;

		/// <summary>
		/// Set up the tcp channel to listen for callbacks
		/// </summary>
		private TcpChannel ClientChannel;

		/// <summary>
		/// Set a timer to check if any updates have occurred
		/// </summary>
		private Timer timer;

		private void TimerEvent( object PassedObject, EventArgs TimerArgs )
		{
            try
            {
                if (NewSubscriber.IsUpdated == true)
                {
                    /// redraw the list box
                    listViewSubscriptions.Clear();
                    ListViewItem lvItem;

                    ArrayList temp = NewSubscriber.GetDataList;
                    SubscriberData tempSubscriber;

                    ColumnHeader header = new ColumnHeader();
                    header.Width = 100;
                    header.Text = "Subscription";
                    listViewSubscriptions.Columns.Add(header);
                    ColumnHeader header2 = (ColumnHeader)header.Clone();
                    header2.Text = "Value";
                    listViewSubscriptions.Columns.Add(header2);

                    for (int i = 0; i < temp.Count; i++)
                    {
                        tempSubscriber = (SubscriberData)temp[i];

                        lvItem = listViewSubscriptions.Items.Add(tempSubscriber.Name);
                        lvItem.SubItems.Add(tempSubscriber.Value.ToString());

                    }

                    if (NewPublisher != null && NewPublisher.FakeTickerList != null)
                    {
                        for (int i = 0; i < NewPublisher.FakeTickerList.Count; i++)
                        {
                            tempSubscriber = (SubscriberData)temp[i];

                            lvItem = listViewSubscriptions.Items.Add("H" + tempSubscriber.Name);
                            lvItem.SubItems.Add(tempSubscriber.Value.ToString());
                        }
                    }

                    NewSubscriber.IsUpdated = false;
                }
			}
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
		}

		public SubscribeClient()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			NewSubscriber = new Subscriber();

            BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider();

            serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            BinaryClientFormatterSinkProvider clientProv = new BinaryClientFormatterSinkProvider();
            
            IDictionary props = new Hashtable();

            props["port"] = 0;

			/// create a channel to listen for the callbacks 
			/// 0 allows the system to decide the port and you can't listen directly to the
			/// broadcast port 8090 in this case
			ClientChannel = new TcpChannel(props, clientProv, serverProv );
            
			/// register the acceptance of the required channel
			ChannelServices.RegisterChannel( ClientChannel, false );

			/// get the serverobject from the required channel
			try
			{
				NewPublisher = ( Publisher )Activator.GetObject( typeof( Publisher ), "tcp://localhost:8090/PublishSubscribe" ); 
			}
			catch( NullReferenceException nullExp )
			{
				MessageBox.Show( "The url for the object is invalid " + nullExp.Message );
			}
			catch( RemotingException remExp )
			{
				MessageBox.Show( "The object type is not defined properly, it needs to be derived for a remoting class " + remExp.Message );
			}

			if( NewPublisher != null )
			{
				try
				{
					NewPublisher.OnUpdatedFakeTicker += new Publisher.PublishedTickerFunction( NewSubscriber.OnSubscriberUpdate );
				}
				catch( Exception exp )
				{
					MessageBox.Show( exp.Message );
				}
			}


			timer = new Timer();
			/// set up the timer event handler
			/// Note the EventHandler requires a void function that takes a object and
			/// an event handler as a parameter

			timer.Tick += new EventHandler( TimerEvent );
			/// set the interval
			timer.Interval = 3000;
			/// start the timer
			timer.Start();
			
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SubscribeTo = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.listViewSubscriptions = new System.Windows.Forms.ListView();
			this.Subscriptions = new System.Windows.Forms.ColumnHeader();
			this.Value = new System.Windows.Forms.ColumnHeader();
			this.button1 = new System.Windows.Forms.Button();
			this.InitialValue = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.InitialValue)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(144, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Subscribe ";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(152, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "Subscribe To";
			// 
			// SubscribeTo
			// 
			this.SubscribeTo.Location = new System.Drawing.Point(8, 56);
			this.SubscribeTo.Name = "SubscribeTo";
			this.SubscribeTo.Size = new System.Drawing.Size(152, 20);
			this.SubscribeTo.TabIndex = 2;
			this.SubscribeTo.Text = "Subcription name";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 80);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(152, 23);
			this.label3.TabIndex = 3;
			this.label3.Text = "Initial Value";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(232, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(168, 16);
			this.label4.TabIndex = 5;
			this.label4.Text = "Subscriptions";
			// 
			// listViewSubscriptions
			// 
			this.listViewSubscriptions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																									this.Subscriptions,
																									this.Value});
			this.listViewSubscriptions.FullRowSelect = true;
			this.listViewSubscriptions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewSubscriptions.Location = new System.Drawing.Point(232, 32);
			this.listViewSubscriptions.MultiSelect = false;
			this.listViewSubscriptions.Name = "listViewSubscriptions";
			this.listViewSubscriptions.Size = new System.Drawing.Size(192, 208);
			this.listViewSubscriptions.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewSubscriptions.TabIndex = 6;
			this.listViewSubscriptions.View = System.Windows.Forms.View.Details;
			// 
			// Subscriptions
			// 
			this.Subscriptions.Text = "Subscriptions";
			this.Subscriptions.Width = 128;
			// 
			// Value
			// 
			this.Value.Text = "Value";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(8, 136);
			this.button1.Name = "button1";
			this.button1.TabIndex = 7;
			this.button1.Text = "Subscribe";
			this.button1.Click += new System.EventHandler(this.OnSubscribe);
			// 
			// InitialValue
			// 
			this.InitialValue.Location = new System.Drawing.Point(8, 96);
			this.InitialValue.Maximum = new System.Decimal(new int[] {
																		 32768,
																		 0,
																		 0,
																		 0});
			this.InitialValue.Name = "InitialValue";
			this.InitialValue.Size = new System.Drawing.Size(152, 20);
			this.InitialValue.TabIndex = 8;
			// 
			// SubscribeClient
			// 
			this.AutoScale = false;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(432, 254);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.InitialValue,
																		  this.button1,
																		  this.listViewSubscriptions,
																		  this.label4,
																		  this.label3,
																		  this.SubscribeTo,
																		  this.label2,
																		  this.label1});
			this.MaximizeBox = false;
			this.Name = "SubscribeClient";
			this.Text = "Subscribe";
			((System.ComponentModel.ISupportInitialize)(this.InitialValue)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new SubscribeClient());
		}

		private void OnSubscribe(object sender, System.EventArgs e)
		{
			if( NewPublisher != null )
			{
				try
				{
					NewPublisher.RegisterFakeTicker( SubscribeTo.Text, ( int )InitialValue.Value );
                    NewPublisher.PublishSubName(SubscribeTo.Text);
				}
				catch( InvalidCastException invExp )
				{
					MessageBox.Show( invExp.Message );
				}
			}
			
		}

		private void OnPublish(object sender, System.EventArgs e)
		{
			NewPublisher.PublishNewEvents();
		}

	}
}
