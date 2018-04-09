using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Skywolf.Client
{
    public class Utility
    {
        #region WCF Helper

        public static NetTcpBinding BuildNetTcpBinding()
        {
            NetTcpBinding binding = new NetTcpBinding();
            binding.CloseTimeout = new TimeSpan(0, 50, 0);
            binding.OpenTimeout = new TimeSpan(0, 50, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 50, 0);
            binding.SendTimeout = new TimeSpan(0, 50, 0);
            binding.TransactionFlow = false;
            binding.TransferMode = TransferMode.Buffered;
            binding.TransactionProtocol = TransactionProtocol.OleTransactions;
            binding.HostNameComparisonMode = HostNameComparisonMode.StrongWildcard;
            binding.ListenBacklog = 10;
            binding.MaxBufferPoolSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxConnections = 10;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.ReliableSession.Ordered = true;
            binding.ReliableSession.InactivityTimeout = new TimeSpan(0, 50, 0);
            binding.ReliableSession.Enabled = false;
            binding.Security.Mode = SecurityMode.None;
            binding.ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas();
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            binding.ReaderQuotas.MaxDepth = int.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;

            return binding;
        }

        public static BasicHttpBinding BuildBasicHttpBinding()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.CloseTimeout = new TimeSpan(0, 10, 0);
            binding.OpenTimeout = new TimeSpan(0, 10, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
            binding.SendTimeout = new TimeSpan(0, 10, 0);
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MessageEncoding = WSMessageEncoding.Mtom;
            binding.UseDefaultWebProxy = false;
            binding.Security.Mode = BasicHttpSecurityMode.None;
            binding.ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas();
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            binding.ReaderQuotas.MaxDepth = int.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;

            return binding;
        }

        public static BasicHttpBinding BuildBasicHttpBindingLongTime()
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.CloseTimeout = new TimeSpan(1, 50, 0);
            binding.OpenTimeout = new TimeSpan(1, 50, 0);
            binding.ReceiveTimeout = new TimeSpan(1, 50, 0);
            binding.SendTimeout = new TimeSpan(1, 50, 0);
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MessageEncoding = WSMessageEncoding.Mtom;
            binding.UseDefaultWebProxy = false;
            binding.Security.Mode = BasicHttpSecurityMode.None;
            binding.ReaderQuotas = new System.Xml.XmlDictionaryReaderQuotas();
            binding.ReaderQuotas.MaxArrayLength = int.MaxValue;
            binding.ReaderQuotas.MaxBytesPerRead = int.MaxValue;
            binding.ReaderQuotas.MaxDepth = int.MaxValue;
            binding.ReaderQuotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas.MaxNameTableCharCount = int.MaxValue;

            return binding;
        }

        public static T CreateMaxItemsChannel<T>(Binding binding, EndpointAddress endpoint)
        {
            ChannelFactory<T> factory = new ChannelFactory<T>(binding, endpoint);

            foreach (OperationDescription op in factory.Endpoint.Contract.Operations)
            {
                var dataContractBehavior = op.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (dataContractBehavior != null)
                {
                    dataContractBehavior.MaxItemsInObjectGraph = int.MaxValue;
                }
            }

            return factory.CreateChannel();
        }

        public static T CreateMaxItemsChannel<T>(InstanceContext callbackInstance, Binding binding, EndpointAddress endpoint)
        {
            DuplexChannelFactory<T> factory = new DuplexChannelFactory<T>(callbackInstance, binding, endpoint);

            foreach (OperationDescription op in factory.Endpoint.Contract.Operations)
            {
                var dataContractBehavior = op.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (dataContractBehavior != null)
                {
                    dataContractBehavior.MaxItemsInObjectGraph = int.MaxValue;
                }
            }

            return factory.CreateChannel();
        }

        public static EndpointAddress BuildEndpointAddress(string key)
        {
            string endpointAddr = ConfigurationManager.AppSettings[key];
            return new EndpointAddress(endpointAddr);
        }

        #endregion

        public static Stream Download(string strURLFileandPath)
        {
            MemoryStream fstr = null;

            try
            {
                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(strURLFileandPath);
                HttpWebResponse ws = (HttpWebResponse)wr.GetResponse();
                Stream str = ws.GetResponseStream();
                byte[] inBuf = new byte[500000];
                int bytesToRead = (int)inBuf.Length;
                int bytesRead = 0;
                while (bytesToRead > 0)
                {
                    int n = str.Read(inBuf, bytesRead, bytesToRead);
                    if (n == 0)
                        break;
                    bytesRead += n;
                    bytesToRead -= n;
                }

                if (bytesRead > 100)
                {
                    fstr = new MemoryStream();
                    fstr.Write(inBuf, 0, bytesRead);
                    str.Close();
                }
            }
            catch (Exception)
            {
            }

            return fstr;
        }

        public static void SendReportMail(
                string[] reportFiles,
                string emailAddress,
                string fromAddress,
                string emailSubject,
                string emailBody)
        {
            MailMessage smtpMsg = null;

            try
            {
                SmtpClient client = new SmtpClient("smtp.126.com");

                smtpMsg = new MailMessage();
                smtpMsg.From = new MailAddress(fromAddress);
                if (emailAddress.Contains(";"))
                {
                    string[] lstToAddressTo = emailAddress.Split(';');
                    foreach (string toAddress in lstToAddressTo)
                    {
                        if (toAddress.Length > 0)
                            smtpMsg.To.Add(new MailAddress(toAddress));
                    }
                }
                else
                    smtpMsg.To.Add(new MailAddress(emailAddress));

                if (string.IsNullOrEmpty(emailSubject))
                    smtpMsg.Subject = "empty";
                else
                    smtpMsg.Subject = emailSubject;

                if (!string.IsNullOrEmpty(emailBody))
                    smtpMsg.Body = emailBody;

                smtpMsg.IsBodyHtml = true;

                if (reportFiles != null)
                {
                    //Attachments
                    foreach (string reportFile in reportFiles)
                    {
                        if (reportFile.Length == 0 || !File.Exists(reportFile))
                        {
                            return;
                        }
                        smtpMsg.Attachments.Add(new Attachment(reportFile));
                    }
                }
                
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential("skywolfsystem@126.com", "optimusprime");
                client.EnableSsl = true;
                client.Send(smtpMsg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (smtpMsg != null) smtpMsg.Dispose();
            }
        }
    }
}
