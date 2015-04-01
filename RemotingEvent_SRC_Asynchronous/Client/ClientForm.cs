using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Remoting;
using Remotable;
using System.Runtime.Remoting.Messaging;

namespace Client
{
    public partial class ClientForm : Form
    {
        public ClientForm()
        {
            InitializeComponent();
        }

        RemoteClass remoteClass;
        WrapperClass wrapperClass;
        

        private void Form1_Load(object sender, EventArgs e)
        {
            
            //Configure remoting.
            RemotingConfiguration.Configure(Application.StartupPath + "\\Client.exe.config",false);

            // Create a proxy from remote object.
            remoteClass = (RemoteClass)Activator.GetObject(typeof(RemoteClass), "http://localhost:5000/Chat");
            //Create an instance of wrapper class.
            wrapperClass = new WrapperClass();
          
            //Associate remote object event with wrapper method.
            remoteClass.MessageReceived += new MessageHandler(wrapperClass.WrapperMessageReceivedHandler);
            //Associate wrapper event with current form event handler.
            wrapperClass.WrapperMessageReceived += new MessageHandler(MessageReceivedHandler);
            
        }

       
        /// <summary>
        /// Sends message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSend_Click(object sender, EventArgs e)
        {
            remoteClass.Send(textBoxMessage.Text);
        }
       
       
       
        
        /// <summary>
        /// Receive message.
        /// </summary>
        /// <param name="message"></param>
        public void MessageReceivedHandler(string message)
        {
            if (listBoxReceived.InvokeRequired == false)
            {
                listBoxReceived.Items.Add(message);
            }
            else
            {
                // Show the text asynchronously
                MessageHandler statusDelegate =
                   new MessageHandler(crossThreadEventHandler);
                this.BeginInvoke(statusDelegate,
                  new object[] { message });
            }
            
        }

        /// <summary>
        /// Asynchronously called method to show message in the UI.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void crossThreadEventHandler(string message)
        {
            listBoxReceived.Items.Add(message);
        }

        /// <summary>
        /// Unassociate event handlers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                remoteClass.MessageReceived -= new MessageHandler(wrapperClass.WrapperMessageReceivedHandler);
                wrapperClass.WrapperMessageReceived -= new MessageHandler(MessageReceivedHandler);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        
    }

   

}