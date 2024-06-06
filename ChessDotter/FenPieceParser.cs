using System;
using System.Collections.Generic;
using pax.chess;

namespace ChessDotter;

partial class PieceLocationDrawer
{
    public static class FenPieceParser
    {
        public static List<Piece> GetPiecesFromFen(string? fen = null)
        {
            if (string.IsNullOrEmpty(fen))
                fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
            string[] fenLines = fen.Split("/");
            string[] strArray = fenLines[7].Split(" ");
            fenLines[7] = strArray[0];
            return FenPieceParser.MapPieces(fenLines);
        }

        private static List<Piece> MapPieces(string[] fenLines)
        {
            List<Piece> pieceList = new List<Piece>();
            for (int index1 = 0; index1 < 8; ++index1)
            {
                int x = 0;
                for (int index2 = 0; index2 < fenLines[index1].Length; ++index2)
                {
                    string c1 = (string) null;
                    char c2 = fenLines[index1][index2];
                    int result;
                    if (int.TryParse(new string(c2, 1), out result))
                        x += result - 1;
                    else
                        c1 = c2.ToString();
                    if (!string.IsNullOrEmpty(c1))
                        pieceList.Add(new Piece(GetPieceType(c1), char.IsLower(c1[0]), x, 7 - index1));
                    ++x;
                }
            }
            return pieceList;
        }

        private static PieceType GetPieceType(string c)
        {
            switch (c.ToUpperInvariant())
            {
                case "P":
                    return PieceType.Pawn;
                case "N":
                    return PieceType.Knight;
                case "B":
                    return PieceType.Bishop;
                case "R":
                    return PieceType.Rook;
                case "Q":
                    return PieceType.Queen;
                case "K":
                    return PieceType.King;
                default:
                    throw new ArgumentOutOfRangeException("invalid piece char " + c);
            }
        }
    }
}