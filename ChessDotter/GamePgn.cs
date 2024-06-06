using System;

namespace ChessDotter;

public class GamePgn(string pgn)
{
    public string Pgn { get; } = pgn;

    public bool IsWhiteOrientation { get; } = !pgn.Contains("[Orientation \"black\"]");
}