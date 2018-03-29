using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Skywolf.Utility
{
    public class LogOperationBehavior : IOperationBehavior
    {
        void IOperationBehavior.AddBindingParameters(OperationDescription operationDescription,
            BindingParameterCollection bindingParameters)
        {
        }

        void IOperationBehavior.ApplyClientBehavior(OperationDescription operationDescription,
            ClientOperation clientOperation)
        {
        }

        void IOperationBehavior.ApplyDispatchBehavior(OperationDescription operationDescription,
            DispatchOperation dispatchOperation)
        {
            dispatchOperation.ParameterInspectors.Add(new LogParameterInspector(dispatchOperation.Parent.Type.FullName));
        }

        void IOperationBehavior.Validate(OperationDescription operationDescription)
        {
        }
    }
}
