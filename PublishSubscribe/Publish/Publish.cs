using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
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
	/// Main publish class that displays connection and subscription information
	/// It also controls the publishing of new information to subscribers which
	/// is controlled by a timer and has responsibility for the TCP channels that
	/// allow remote access to the publisher
	/// </summary>
	public class PublishService : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TextBox m_NumSubscriptions;
		private System.Windows.Forms.TextBox m_TimerCalls;

		/// <summary>
		/// delcare a member of the publisher class
		/// </summary>
		private static Publisher NewPublisher;


		/// <summary>
		/// Set a timer to control the flow of the publications
		/// </summary>
		private Timer timer;

		/// <summary>
		/// The number of timer calls. Needs to be static as it is incremented in the timer function
		/// </summary>
		private int nTimerCalls = 0;

		/// <summary>
		/// The TCP Server Channel
		/// </summary>
		private TcpChannel Channel;
        private TextBox txtClientNames;

		/// <summary>
		/// The number of subscriptions
		/// </summary>
		private int nSubscriptions = 0;


		/// <summary>
		/// The timer event to generate publications
		/// </summary>
		/// <param name="PassedObject">The Object that created the Timer</param>
		/// <param name="TimerArgs">The Event Arguments that are passed to the Timer function</param>
		private void TimerEvent( object PassedObject, EventArgs TimerArgs )
		{
			nTimerCalls++;
			
			m_TimerCalls.Text = nTimerCalls.ToString();
			
			if( NewPublisher != null )
				NewPublisher.PublishNewEvents();

			m_NumSubscriptions.Text = ( ( object )NewPublisher.NumSubscriptions ).ToString();

		}

		public PublishService()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//

			timer = new Timer();
			/// set up the timer event handler
			/// Note the EventHandler requires a void function that takes a object and
			/// an event handler as a parameter

			timer.Tick += new EventHandler( TimerEvent );
			/// set the interval
			timer.Interval = 5000;
			/// start the timer
			timer.Start();


            BinaryServerFormatterSinkProvider serverProv = new BinaryServerFormatterSinkProvider();

            serverProv.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

            BinaryClientFormatterSinkProvider clientProv = new BinaryClientFormatterSinkProvider();

            IDictionary props = new Hashtable();

            props["port"] = 8090;


            /// Set up the remoting channel
            /// create the channel on port 8090
            Channel = new TcpChannel(props, clientProv, serverProv);

			/// register the channel with the runtime
			ChannelServices.RegisterChannel( Channel );

			/// register the remote type and pass in a uri identifying string
			RemotingConfiguration.RegisterWellKnownServiceType( typeof( Publisher ), "PublishSubscribe", WellKnownObjectMode.Singleton );

		    
			/// get a remote instance to the singleton publisher object
			try
			{
				NewPublisher = ( Publisher )Activator.GetObject( typeof( Publisher ), "tcp://localhost:8090/PublishSubscribe" );
                NewPublisher.OnUpdateSubscrabberName += new Publisher.SubscrabberName(UpdateSubscrabberName);
			}
			catch( NullReferenceException nullExp )
			{
				MessageBox.Show( "The url for the object is invalid " + nullExp.Message );
			}
			catch( RemotingException remExp )
			{
				MessageBox.Show( "The object type is not defined properly, it needs to be derived for a remoting class " + remExp.Message );
			}
		}

        public void UpdateSubscrabberName(string name)
        {
            this.Invoke(new System.Action(delegate()
                        {
                            try
                            {
                                txtClientNames.Text += name + ";";
                            }
                            catch (Exception)
                            {
                            }
                        }));
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
            this.m_NumSubscriptions = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.m_TimerCalls = new System.Windows.Forms.TextBox();
            this.txtClientNames = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Number of Subscriptions";
            // 
            // m_NumSubscriptions
            // 
            this.m_NumSubscriptions.Location = new System.Drawing.Point(152, 8);
            this.m_NumSubscriptions.Name = "m_NumSubscriptions";
            this.m_NumSubscriptions.ReadOnly = true;
            this.m_NumSubscriptions.Size = new System.Drawing.Size(100, 20);
            this.m_NumSubscriptions.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "Timer Calls";
            // 
            // m_TimerCalls
            // 
            this.m_TimerCalls.Location = new System.Drawing.Point(152, 32);
            this.m_TimerCalls.Name = "m_TimerCalls";
            this.m_TimerCalls.ReadOnly = true;
            this.m_TimerCalls.Size = new System.Drawing.Size(100, 20);
            this.m_TimerCalls.TabIndex = 3;
            // 
            // txtClientNames
            // 
            this.txtClientNames.Location = new System.Drawing.Point(152, 58);
            this.txtClientNames.Name = "txtClientNames";
            this.txtClientNames.Size = new System.Drawing.Size(293, 20);
            this.txtClientNames.TabIndex = 4;
            // 
            // PublishService
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(480, 142);
            this.Controls.Add(this.txtClientNames);
            this.Controls.Add(this.m_TimerCalls);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.m_NumSubscriptions);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "PublishService";
            this.Text = "Publish";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new PublishService());
		}

		private void OnPublishData(object sender, System.EventArgs e)
		{
			NewPublisher.PublishNewEvents();
		}
	}
}
