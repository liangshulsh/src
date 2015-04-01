using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RouletteStrategyTest
{
    public class Piece
    {
        public bool? IsRed { get; set; }
        public int Number { get; set; }

        public BigSmall BigSmall
        {
            get
            {
                if (Number <= 0)
                {
                    return BigSmall.None;
                }
                else if (Number > 18)
                {
                    return BigSmall.Big;
                }
                else
                {
                    return BigSmall.Small;
                }
            }
        }

        public BigMiddleSmall BigMiddleSmall
        {
            get
            {
                if (Number <= 0)
                {
                    return BigMiddleSmall.None;
                }
                else if (Number <= 12)
                {
                    return BigMiddleSmall.Small;
                }
                else if (Number > 12 && Number <= 24)
                {
                    return BigMiddleSmall.Middle;
                }
                else
                {
                    return BigMiddleSmall.Big;
                }
            }
        }

        public OddEven OddEven
        {
            get
            {
                if (Number <= 0)
                {
                    return OddEven.None;
                }
                else if (Number % 2 == 0)
                {
                    return OddEven.Even;
                }
                else
                {
                    return OddEven.Odd;
                }
            }
        }

        public RedBlack RedBlack
        {
            get
            {
                if (IsRed.HasValue)
                {
                    if (IsRed.Value)
                    {
                        return RouletteStrategyTest.RedBlack.Red;
                    }
                    else
                    {
                        return RouletteStrategyTest.RedBlack.Black;
                    }
                }
                else
                {
                    return RouletteStrategyTest.RedBlack.None;
                }
            }
        }
    }

    public enum BigSmall
    {
        None,
        Big,
        Small
    }

    public enum BigMiddleSmall
    {
        None,
        Big,
        Middle,
        Small
    }

    public enum OddEven
    {
        None,
        Odd,
        Even
    }

    public enum RedBlack
    {
        None,
        Red,
        Black
    }
}
