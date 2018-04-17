using System;
using System.Collections.Concurrent;
using System.Data;
using System.ServiceModel;
using System.Text;
using log4net;
using Skywolf.Contracts.Services;
using Skywolf.Utility;
using System.Globalization;
using System.Collections.Generic;
using Skywolf.Contracts.DataContracts.Instrument;

namespace Skywolf.TradingService
{
    [ServiceBehavior(Namespace = Constants.NAMESPACE, InstanceContextMode = InstanceContextMode.PerCall)]
    public class IBTradingService : EchoService, IIBTradingService
    {

    }
}
