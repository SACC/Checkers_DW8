using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers_DW8
{
    public class RowColl
    {
        public Int32 row { get; set; }
        public Int32 col { get; set; }

        public RowColl(int p)
        {
            col = p % 8;
            row = p / 8;
        }

        public int GetPosition()
        {
            return (row * 8) + col;
        }
    }

    public class Pieces
    {
        public string Name { get; set; }
        public int Position { get; set; }
        public ColorPeace Color { get; set; }
        public bool IsQueen { get; set; }
        public bool IsActive { get; set; }        

        public Pieces(string n, int p, ColorPeace c, bool q)
        {
            this.Name = n;
            this.Position = p;
            this.Color = c;
            this.IsQueen = q;
            this.IsActive = true;
        }
    }

    public enum ColorPeace
    {
        black,
        white
    }

    public class PlayMove
    {                        
        public static bool CanMove(Pieces p, List<Pieces> boardPieces)
        {
            if (!p.IsActive)
                return false;
            bool res = false;
            Pieces pc = null;
            RowColl rc = new RowColl(p.Position);
            if (p.Color == ColorPeace.black)
            {
                #region BlackPieces
                if (rc.row < 8)
                {
                    switch (rc.col)
                    {
                        case 0:
                            rc.col++;
                            pc = (from pos in boardPieces
                                  where pos.Position.Equals(rc.col + ((rc.row + 1) * 8))
                                  select pos).FirstOrDefault();
                            if (pc != null)
                            {                                
                                res = false;
                                if (pc.Color == ColorPeace.white && PlayMove.IsEmptyPlace(p.Position + 18, boardPieces))
                                {
                                    res = true;
                                }                                                                    
                            }
                            else
                            {
                                res = true;
                            }
                            break;
                        case 7:
                            rc.col--;
                            pc = (from pos in boardPieces
                                  where pos.Position.Equals(rc.col + ((rc.row + 1) * 8))
                                  select pos).FirstOrDefault();                             
                            if (pc != null)
                            {                                
                                res = false;
                                if (pc.Color == ColorPeace.white && PlayMove.IsEmptyPlace(p.Position + 14, boardPieces))
                                {
                                    res = true;
                                }                                                                    
                            }
                            else
                            {
                                res = true;
                            }
                            break;
                        default:
                            pc = (from pos in boardPieces
                                           where pos.Position.Equals((rc.col + 1) + ((rc.row + 1) * 8))
                                           select pos).FirstOrDefault();
                            if (pc != null)
                            {
                                res = false;
                                if (pc.Color == ColorPeace.white && PlayMove.IsEmptyPlace(p.Position + 18, boardPieces))
                                {
                                    res = true;
                                }
                            }
                            else
                            {
                                res = true;
                            }
                            if (res) break;
                            pc = (from pos in boardPieces
                                  where pos.Position.Equals((rc.col - 1) + ((rc.row + 1) * 8))
                                  select pos).FirstOrDefault();
                            if (pc != null)
                            {
                                res = false;
                                if (pc.Color == ColorPeace.white && PlayMove.IsEmptyPlace(p.Position + 14, boardPieces))
                                {
                                    res = true;
                                }
                            }
                            else
                            {
                                res = true;
                            }
                            break;
                    }
                }
                #endregion
            }
            else
            {
                #region WhitePieces
                if (rc.row > 0)
                {
                    switch (rc.col)
                    {
                        case 0:
                            rc.col++;
                            pc = (from pos in boardPieces
                                  where pos.Position.Equals(rc.col + ((rc.row - 1) * 8))
                                  select pos).FirstOrDefault();
                            if (pc != null)
                            {
                                res = false;
                                if (pc.Color == ColorPeace.black && PlayMove.IsEmptyPlace(p.Position - 14, boardPieces))
                                {
                                    res = true;
                                }
                            }
                            else
                            {
                                res = true;
                            }
                            break;
                        case 7:
                            rc.col--;
                            pc = (from pos in boardPieces
                                  where pos.Position.Equals(rc.col + ((rc.row - 1) * 8))
                                  select pos).FirstOrDefault();
                            if (pc != null)
                            {
                                res = false;
                                if (pc.Color == ColorPeace.black && PlayMove.IsEmptyPlace(p.Position - 18, boardPieces))
                                {
                                    res = true;
                                }
                            }
                            else
                            {
                                res = true;
                            }
                            break;
                        default:
                            pc = (from pos in boardPieces
                                  where pos.Position.Equals((rc.col + 1) + ((rc.row - 1) * 8))
                                  select pos).FirstOrDefault();
                            if (pc != null)
                            {
                                res = false;
                                if (pc.Color == ColorPeace.black && PlayMove.IsEmptyPlace(p.Position - 14, boardPieces))
                                {
                                    res = true;
                                }
                            }
                            else
                            {
                                res = true;
                            }
                            if (res) break;
                            pc = (from pos in boardPieces
                                  where pos.Position.Equals((rc.col - 1) + ((rc.row - 1) * 8))
                                  select pos).FirstOrDefault();
                            if (pc != null)
                            {
                                res = false;
                                if (pc.Color == ColorPeace.black && PlayMove.IsEmptyPlace(p.Position - 18, boardPieces))
                                {
                                    res = true;
                                }
                            }
                            else
                            {
                                res = true;
                            }
                            break;
                    }
                }
                #endregion
            }
            
            return res;
        }        

        public static bool IsEmptyPlace(int p, List<Pieces> boardPieces)
        {
            return null == (from b in boardPieces where b.Position.Equals(p) select b).SingleOrDefault();         
        }        

        public static bool TakePiece(int p, Pieces ps, List<Pieces> boardPieces)
        {
            bool res = false;

            Pieces pt = (from b in boardPieces where b.Position.Equals(p) select b).SingleOrDefault();

            if (pt.Color != ps.Color)
            {
                int newP = 0;
                RowColl rc = new RowColl(p);
                if (ps.Color == ColorPeace.black)
                {
                    if (((p - ps.Position) == 7 && rc.col > 1) || ((ps.Position - p) == 9 && rc.col < 6))
                    {
                        newP = p + (p - ps.Position);
                        res = PlayMove.IsEmptyPlace(newP, boardPieces);
                    }
                }
                else
                {
                    if (((ps.Position - p) == 9 && rc.col > 1) || ((p - ps.Position) == 7 && rc.col < 6))
                    {
                        newP = p - (ps.Position - p);
                        res = PlayMove.IsEmptyPlace(newP, boardPieces);
                    }
                }
                res = true;
            }

            return res;
        }

        public static bool IsCrow(int p, ColorPeace c)
        {
            bool res = false;
            RowColl rc = new RowColl(p);
            if (c == ColorPeace.black)
            {
                res = rc.row == 7;
            }
            else
            {
                res = rc.row == 0;
            }
            return res;
        }        
    }
}
