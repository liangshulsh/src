using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;


namespace Remotable
{
    /// <summary>
    /// Represents the method that will handle the Remotable.RemoteClass.MessageReceived event.
    /// </summary>
    /// <param name="message">Received message</param>
    [Serializable]
    [ComVisible(true)]
    public delegate void MessageHandler(string message);

    /// <summary>
    /// Shared remoting class that orchestrate messaging tasks 
    /// </summary>
    public class RemoteClass : MarshalByRefObject
    {

        /// <summary>
        /// Occurs when a broadcast message received.
        /// </summary>
        public event MessageHandler MessageReceived;

        /// <summary>
        /// For call asynchronously
        /// </summary>
        private MessageHandler messageDelegate;

        /// <summary>
        /// Initializes a new instance of the Remotable.RemoteClass class.
        /// </summary>
        public RemoteClass()
        {
            messageDelegate = new MessageHandler(Send);
        }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this
        /// instance.
        /// </summary>
        /// <returns>
        ///An object of type System.Runtime.Remoting.Lifetime.ILease used to control
        ///the lifetime policy for this instance. This is the current lifetime service
        ///object for this instance if one exists; otherwise, a new lifetime service
        ///object initialized to the value of the System.Runtime.Remoting.Lifetime.LifetimeServices.LeaseManagerPollTime
        ///property.
        ///null value means this object has to live forever.
        /// </returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        private delegate void WrapperDelegate(string message, MessageHandler messageDelegate);
        /// <summary>
        /// Broadcast message to all clients
        /// </summary>
        /// <param name="message">message string</param>
        public void Send(string message)
        {
            if (MessageReceived != null)
            {
                MessageHandler messageDelegate = null;
                Delegate[] invocationList_ = null;
                try
                {
                    invocationList_ = MessageReceived.GetInvocationList();
                }
                catch (MemberAccessException ex)
                {
                    throw ex;
                }
                if (invocationList_ != null)
                {
                    lock (this)
                    {
                        foreach (Delegate del in invocationList_)
                        {
                            messageDelegate = (MessageHandler)del;
                            WrapperDelegate wrDel = new WrapperDelegate(BeginSend);
                            AsyncCallback callback = new AsyncCallback(EndSend);
                            wrDel.BeginInvoke(message, messageDelegate, callback, wrDel);
                        }
                    }
                }
            }
        }

        private void BeginSend(string message, MessageHandler messageDelegate)
        {
            try
            {
                System.Threading.Thread.Sleep(8000);
                messageDelegate(message);

            }
            catch (Exception e)
            {
                MessageReceived -= messageDelegate;
            }
        }

        private void EndSend(IAsyncResult result)
        {
            WrapperDelegate wrDel = (WrapperDelegate)result.AsyncState;
            wrDel.EndInvoke(result);
        }

    }


    

}
