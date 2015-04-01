using System;
using System.Runtime.InteropServices;

namespace InteractiveProcessFromService
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct WTS_SESSION_INFO
    {
        public int SessionId;
        public string pWinStationName;
        public int State;
    }
}
