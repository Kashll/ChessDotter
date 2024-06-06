using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ChessDotter.Properties;
using pax.chess;
using pax.chess.Validation;

namespace ChessDotter;

class PieceLocationDrawer
{
    public static void DrawPieceLocations(List<GamePgn> games, bool pawnsOnly, bool displayPieceColors)
    {
        int i = 0;
        foreach (GamePgn gamePgn in games)
        {
            List<Piece> pieces = gamePgn.Fen != null ? FenPieceParser.GetPiecesFromFen(gamePgn.Fen) : GetPieces(gamePgn.Pgn);
            DrawPieceLocations(pieces, gamePgn.IsWhiteOrientation, pawnsOnly, displayPieceColors, Path.Combine(Directory.GetCurrentDirectory(), $"Chapter{++i}.png"));
        }
    }

    private static void DrawPieceLocations(List<Piece> gamePieces, bool isWhiteOrientation, bool pawnsOnly, bool displayPieceColors, string destinationPath)
    {
        byte[] boardBytes = Resources.board;
        using (MemoryStream boardStream = new MemoryStream(boardBytes))
        {
            if (pawnsOnly)
            {
                gamePieces = gamePieces.Where(x => x.Type == PieceType.Pawn).ToList();
            }

            Image boardImage = Image.FromStream(boardStream);
            double squareWidth = boardImage.Width / 8.0;
            double squareHeight = boardImage.Height / 8.0;

            foreach (Piece piece in gamePieces)
            {
                if (!isWhiteOrientation)
                {
                    piece.Position.X = (byte) int.Abs(piece.Position.X - 7);
                    piece.Position.Y = (byte) int.Abs(piece.Position.Y - 7);
                }

                int positionX = (int) Math.Ceiling((piece.Position.X + 1) * squareWidth - squareWidth / 2.0);
                int positionY = (int) Math.Ceiling((8 - piece.Position.Y) * squareHeight - squareHeight / 2.0);

                Brush brush = getBrush(piece, displayPieceColors);
                DrawCircle(boardImage, new Point(positionX, positionY), 12, brush);
            }

            boardImage.Save(destinationPath);
        }
    }

    private static Brush getBrush(Piece piece, bool displayPieceColors)
    {
        if (!displayPieceColors)
        {
            return Brushes.Red;
        }

        return piece.IsBlack ? Brushes.Black : Brushes.White;
    }

    private static void DrawCircle(Image image, Point center, int radius, Brush brush)
    {
        using (Graphics graphics = Graphics.FromImage(image))
        {
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.FillEllipse(brush, center.X - radius, center.Y - radius, radius * 2, radius * 2);
        }
    }

    private static List<Piece> GetPieces(string pgn)
    {
        Game game = Pgn.MapString(pgn);
        List<Piece> statePieces = game.State.Pieces;

        return statePieces;
    }

    public static class FenPieceParser
    {
        internal static List<Piece> GetPiecesFromFen(string? fen = null)
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