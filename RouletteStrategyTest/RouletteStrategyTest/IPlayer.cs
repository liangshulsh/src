using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RouletteStrategyTest
{
    public interface IPlayer
    {
        bool Result(Piece piece);
    }
}
