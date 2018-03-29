using System.ServiceModel;


namespace Skywolf.Utility
{
    [ServiceContract]
    public interface IEcho
    {
        [OperationContract]
        string Ping();
    }
}
