using pax.chess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ChessDotter;

public class GamePgn(string pgn)
{
    public string Pgn { get; } = pgn;

    public string Fen { get; } = ParseFen(pgn);

    public bool IsWhiteOrientation { get; } = !pgn.Contains("[Orientation \"black\"]");

    private static string ParseFen(string pgn)
    {
        string[] strings = pgn.Split("\n");
        string fenString = strings.FirstOrDefault(x => x.Contains("[FEN"));

        string[] subStrings = fenString?.Split("\"");
        return subStrings?[1];
    }
}