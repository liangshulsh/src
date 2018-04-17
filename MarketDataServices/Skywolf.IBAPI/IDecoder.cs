using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Skywolf.IBApi
{
    public interface IDecoder
    {
        double ReadDouble();
        double ReadDoubleMax();
        long ReadLong();
        int ReadInt();
        int ReadIntMax();
        bool ReadBoolFromInt();
        string ReadString();
    }
}
