using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using ChessDotter.Properties;
using pax.chess;

namespace ChessDotter;

class PieceLocationDrawer
{
    public static void DrawPieceLocations(List<GamePgn> games)
    {
        int i = 0;
        foreach (GamePgn gamePgn in games)
        {
            List<Piece> pieces = GetPieces(gamePgn.Pgn);
            DrawPieceLocations(pieces, gamePgn.IsWhiteOrientation, Path.Combine(Directory.GetCurrentDirectory(), $"Chapter{++i}.png"));
        }
    }

    private static void DrawPieceLocations(List<Piece> gamePieces, bool isWhiteOrientation, string destinationPath)
    {
        byte[] boardBytes = Resources.board;
        using (MemoryStream boardStream = new MemoryStream(boardBytes))
        {
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
                int positionY = (int) Math.Ceiling((8 - piece.Position.Y)  * squareHeight - squareHeight / 2.0);
                DrawCircle(boardImage, new Point(positionX, positionY), 12);
            }

            boardImage.Save(destinationPath);
        }
    }

    private static void DrawCircle(Image image, Point center, int radius)
    {
        using (Graphics graphics = Graphics.FromImage(image)) { 
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.FillEllipse(Brushes.Red, center.X - radius, center.Y - radius, radius * 2, radius * 2);
        }
    }

    private static List<Piece> GetPieces(string pgn)
    {
        Game game = Pgn.MapString(pgn);
        List<Piece> statePieces = game.State.Pieces;

        return statePieces;
    }
}