using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RouletteStrategyTest
{
    public class StrategyPlayer : IPlayer
    {
        public List<Piece> _historicalPieces = new List<Piece>();
        public List<int> _cashFlow = new List<int>();

        public int _capital = 0;

        bool _bEnroll = false;

        BigSmall _begBigSmall = BigSmall.None;

        int _begQuantity = 0;

        public bool Result(Piece piece)
        {
            if (_bEnroll)
            {
                if (piece.BigSmall == _begBigSmall)
                {
                    _capital += _begQuantity * 2;
                    _begQuantity = 0;
                    _bEnroll = false;

                    if (_capital >= 120)
                    {
                        return false;
                    }
                }
            }

            _historicalPieces.Add(piece);

            if (_historicalPieces.Count > 10)
            {
                _cashFlow.Add(_capital);

                if (_bEnroll)
                {
                    if (_begQuantity == 1)
                    {
                        _begQuantity = 2;
                    }
                    else if (_begQuantity == 2)
                    {
                        _begQuantity = 5;
                    }
                    else if (_begQuantity == 5)
                    {
                        _begQuantity = 10;
                    }
                    else if (_begQuantity == 10)
                    {
                        _begQuantity = 22;
                    }
                    else
                    {
                        return false;
                    }

                    _capital -= _begQuantity;
                }
                else
                {
                    int iCount = 0;
                    int iIdx = _historicalPieces.Count - 1;
                    BigSmall firstBigSmall = BigSmall.None;

                    for (int i = _historicalPieces.Count -1; i >= 0; i--)
                    {
                        if (_historicalPieces[i].BigSmall != BigSmall.None)
                        {
                            firstBigSmall = _historicalPieces[i].BigSmall;
                            iIdx = i;
                            break;
                        }
                    }

                    while (iCount < 3)
                    {
                        if (_historicalPieces[iIdx].BigSmall == firstBigSmall)
                        {
                            iIdx--;
                            iCount++;
                        }
                        else if (_historicalPieces[iIdx].BigSmall == BigSmall.None)
                        {
                            iIdx--;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (iCount == 3)
                    {
                        _bEnroll = true;
                        _begBigSmall = firstBigSmall == BigSmall.Big ? BigSmall.Small : BigSmall.Big;
                        _begQuantity = 1;
                        _capital -= _begQuantity;
                    }
                }
            }

            return true;
        }


    }
}
