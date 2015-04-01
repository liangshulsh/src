using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RouletteStrategyTest
{
    public class RouletteMachine
    {
        private List<IPlayer> _playerList = new List<IPlayer>();
        private List<Piece> _piece = new List<Piece>();

        public RouletteMachine()
        {
            _piece.Add(new Piece() { IsRed = null, Number = 0 });
            _piece.Add(new Piece() { IsRed = true, Number = 27 });
            _piece.Add(new Piece() { IsRed = false, Number = 10 });
            _piece.Add(new Piece() { IsRed = true, Number = 25 });
            _piece.Add(new Piece() { IsRed = false, Number = 29 });
            _piece.Add(new Piece() { IsRed = true, Number = 12 });
            _piece.Add(new Piece() { IsRed = false, Number = 8 });
            _piece.Add(new Piece() { IsRed = true, Number = 19 });
            _piece.Add(new Piece() { IsRed = false, Number = 31 });
            _piece.Add(new Piece() { IsRed = true, Number = 18 });
            _piece.Add(new Piece() { IsRed = false, Number = 6 });
            _piece.Add(new Piece() { IsRed = true, Number = 21 });
            _piece.Add(new Piece() { IsRed = false, Number = 33 });
            _piece.Add(new Piece() { IsRed = true, Number = 16 });
            _piece.Add(new Piece() { IsRed = false, Number = 4 });
            _piece.Add(new Piece() { IsRed = true, Number = 23 });
            _piece.Add(new Piece() { IsRed = false, Number = 35 });
            _piece.Add(new Piece() { IsRed = true, Number = 14 });
            _piece.Add(new Piece() { IsRed = false, Number = 2 });
            _piece.Add(new Piece() { IsRed = null, Number = -1 });
            _piece.Add(new Piece() { IsRed = false, Number = 28 });
            _piece.Add(new Piece() { IsRed = true, Number = 9 });
            _piece.Add(new Piece() { IsRed = false, Number = 26 });
            _piece.Add(new Piece() { IsRed = true, Number = 30 });
            _piece.Add(new Piece() { IsRed = false, Number = 11 });
            _piece.Add(new Piece() { IsRed = true, Number = 7 });
            _piece.Add(new Piece() { IsRed = false, Number = 20 });
            _piece.Add(new Piece() { IsRed = true, Number = 32 });
            _piece.Add(new Piece() { IsRed = false, Number = 17 });
            _piece.Add(new Piece() { IsRed = true, Number = 5 });
            _piece.Add(new Piece() { IsRed = false, Number = 22 });
            _piece.Add(new Piece() { IsRed = true, Number = 34 });
            _piece.Add(new Piece() { IsRed = false, Number = 15 });
            _piece.Add(new Piece() { IsRed = true, Number = 3 });
            _piece.Add(new Piece() { IsRed = false, Number = 24 });
            _piece.Add(new Piece() { IsRed = true, Number = 36 });
            _piece.Add(new Piece() { IsRed = false, Number = 13 });
            _piece.Add(new Piece() { IsRed = true, Number = 1 });
        }

        public void AddPlayer(IPlayer player)
        {
            _playerList.Add(player);
        }

        public void Run(int nRound)
        {
            Random rand = new Random(Convert.ToInt32(DateTime.Now.Ticks % 1000000000));
            for (int i = 0; i < nRound; i++)
            {
                int pieceIdx = rand.Next(0, _piece.Count - 1);
                foreach (IPlayer player in _playerList)
                {
                    if (!player.Result(_piece[pieceIdx]))
                    {
                        return;
                    }
                }
            }
        }
    }
}
