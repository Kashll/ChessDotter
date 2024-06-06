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

public class PieceLocationDrawer
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
}