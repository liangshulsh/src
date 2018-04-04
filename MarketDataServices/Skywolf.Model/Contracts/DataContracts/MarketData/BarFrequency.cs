using System;
using System.Runtime.Serialization;

namespace Skywolf.Contracts.DataContracts.MarketData
{
    [DataContract(Namespace = Constants.NAMESPACE)]
    public enum BarFrequency
    {
        [EnumMember]
        Tick = 0,

        [EnumMember]
        Minute1 = 1,

        [EnumMember]
        Minute5 = 5,

        [EnumMember]
        Minute15 = 15,

        [EnumMember]
        Minute30 = 30,

        [EnumMember]
        Hour1 = 60,

        [EnumMember]
        Hour4 = 240,

        [EnumMember]
        Day1 = 1440,

        [EnumMember]
        Week1 = 10080,

        [EnumMember]
        Month1 = 43200
    }
}
