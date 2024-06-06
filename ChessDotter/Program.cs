using CommandLine;
using System;
using System.Collections.Generic;

namespace ChessDotter
{
    class Program
    {
        public class Options
        {
            [Value(0, MetaName = "pat", HelpText = "Lichess personal access token", Required = true)]
            public string PersonalAccessToken { get; set; }

            [Value(1, MetaName = "studyId", HelpText = "The Lichess study id to use", Required = true)]
            public string StudyId { get; set; }

            [Option('p', "pawnsOnly", Required = false, HelpText = "Only draw pawns")]
            public bool PawnsOnly { get; set; }

            [Option('c', "displayPieceColors", Required = false, HelpText = "Display piece colors")]
            public bool DisplayPieceColors { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(x =>
            {
                LichessStudyClient lichessStudyClient = new LichessStudyClient(x.PersonalAccessToken);
                List<GamePgn> gamePgns = lichessStudyClient.GetStudyGames(x.StudyId).Result;
                PieceLocationDrawer.DrawPieceLocations(gamePgns, x.PawnsOnly, x.DisplayPieceColors);

                Console.WriteLine($"Generated images for {gamePgns.Count} study chapter(s)");
            }).WithNotParsed(x =>
            {
                Console.WriteLine("Invalid arguments...");
                Console.ReadKey();
            });
        }
    }
}
