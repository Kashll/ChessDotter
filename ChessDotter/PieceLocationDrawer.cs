using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using ChessDotter.Properties;
using pax.chess;

namespace ChessDotter;

class PieceLocationDrawer
{
    public static void DrawPieceLocations(List<string> gamePgns)
    {
        int i = 0;
        IEnumerable<List<Piece>> piecesPerGame = gamePgns.Select(GetPieces);
        foreach (List<Piece> pieces in piecesPerGame)
        {
            DrawPieceLocations(pieces, Path.Combine(Directory.GetCurrentDirectory(), $"Chapter{++i}.png"));
        }
    }

    private static void DrawPieceLocations(List<Piece> gamePieces, string destinationPath)
    {
        byte[] boardBytes = Resources.board;
        using (MemoryStream boardStream = new MemoryStream(boardBytes))
        {
            Image boardImage = Image.FromStream(boardStream);

            Bitmap boardBitmap = new Bitmap(boardImage);

            double squareWidth = boardBitmap.Width / 8.0;
            double squareHeight = boardBitmap.Height / 8.0;

            foreach (Piece piece in gamePieces)
            {
                int positionX = (int) Math.Ceiling((piece.Position.X + 1) * squareWidth - squareWidth / 2.0);
                int positionY = (int) Math.Ceiling((8 - piece.Position.Y)  * squareHeight - squareHeight / 2.0);
                DrawCircle(boardBitmap, new Point(positionX, positionY), 10);
            }

            boardBitmap.Save(destinationPath);
        }
    }

    private static void DrawCircle(Bitmap image, Point center, int radius)
    {
        for (int i = 0; i < 360; i++)
        {
            double x = center.X - radius * Math.Cos(2 * Math.PI / 360 * i);
            double y = center.Y - radius * Math.Sin(2 * Math.PI / 360 * i);
            image.SetPixel((int) x, (int) y, Color.Red);
        }
    }

    private static List<Piece> GetPieces(string pgn)
    {
        Game game = Pgn.MapString(pgn);

        List<Piece> statePieces = game.State.Pieces;

        return statePieces;
    }
}