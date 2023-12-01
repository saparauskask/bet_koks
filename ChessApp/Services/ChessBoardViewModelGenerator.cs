using System.Text;
using ChessApp.ChessLogic;
using ChessApp.ChessLogic.Pieces;

namespace ChessApp.Services
{
    public class ChessBoardViewModelGenerator
    {
        private readonly string htmlStart = "<table class=\"chess-board\"><tbody>";
        private readonly string htmlEnd = "</tbody></table>";
        
        public string GenerateHtml(Board board)
        {
            var strB = new StringBuilder();
            var sq = board.GetBoardCopy().Squares;
            var sqP = new SquarePainter(true);
            strB.Append(htmlStart);

            for (int y = 7; y >= 0; --y)
            {
                strB.AppendLine("<tr>");
                for (int x = 0; x < 8; ++x)
                {
                    var currentSquare = sq[x, y];
                    switch (currentSquare.Piece)
                    {
                        case Pawn pawn:
                            strB.AppendLine($"<td class={sqP.currentColor}>{sqP.GetPawn(pawn.IsWhite)}</td>");
                            break;
                        case Rook rook:
                            strB.AppendLine($"<td class={sqP.currentColor}>{sqP.GetPawn(rook.IsWhite)}</td>");
                            break;
                        case Knight knight:
                            strB.AppendLine($"<td class={sqP.currentColor}>{sqP.GetKnight(knight.IsWhite)}</td>");
                            break;
                        case Bishop bishop:
                            strB.AppendLine($"<td class={sqP.currentColor}>{sqP.GetBishop(bishop.IsWhite)}</td>");
                            break;
                        case Queen queen:
                            strB.AppendLine($"<td class={sqP.currentColor}>{sqP.GetQueen(queen.IsWhite)}</td>");
                            break;
                        case King king:
                            strB.AppendLine($"<td class={sqP.currentColor}>{sqP.GetKing(king.IsWhite)}</td>");
                            break;
                        default:
                            strB.AppendLine($"<td class={sqP.currentColor}></td>");
                            break;
                    }

                    if (x != 7)
                    {
                        sqP.switchColor();
                    }
                }
                strB.AppendLine("</tr>");
            }

            strB.Append(htmlEnd);

            return strB.ToString();
        }


    }
}
