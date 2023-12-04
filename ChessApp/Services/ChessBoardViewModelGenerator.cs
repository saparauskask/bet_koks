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
                            strB.AppendLine($"<td class={sqP.currentColor}>{sqP.ButtStart}{x} {y}{sqP.ButtMiddle}{sqP.GetPawn(pawn.IsWhite)}{sqP.ButtEnd}</td>");
                            break;
                        case Rook rook:
                            strB.AppendLine($"<td class={sqP.currentColor}>{sqP.ButtStart}{x} {y}{sqP.ButtMiddle}{sqP.GetRook(rook.IsWhite)}{sqP.ButtEnd}</td>");
                            break;
                        case Knight knight:
                            strB.AppendLine($"<td class={sqP.currentColor}>{sqP.ButtStart}{x} {y}{sqP.ButtMiddle}{sqP.GetKnight(knight.IsWhite)}{sqP.ButtEnd}</td>");
                            break;
                        case Bishop bishop:
                            strB.AppendLine($"<td class={sqP.currentColor}>{sqP.ButtStart}{x} {y}{sqP.ButtMiddle}{sqP.GetBishop(bishop.IsWhite)}{sqP.ButtEnd}</td>");
                            break;
                        case Queen queen:
                            strB.AppendLine($"<td class={sqP.currentColor}>{sqP.ButtStart}{x} {y}{sqP.ButtMiddle}{sqP.GetQueen(queen.IsWhite)}{sqP.ButtEnd}</td>");
                            break;
                        case King king:
                            strB.AppendLine($"<td class={sqP.currentColor}>{sqP.ButtStart}{x} {y}{sqP.ButtMiddle}{sqP.GetKing(king.IsWhite)}{sqP.ButtEnd}</td>");
                            break;
                        default:
                            strB.AppendLine($"<td class={sqP.currentColor}>{sqP.ButtStart}{x} {y}{sqP.ButtMiddle}{sqP.ButtEnd}</td>");
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
