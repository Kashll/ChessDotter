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
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(x =>
            {
                StudyApi studyApi = new StudyApi(x.PersonalAccessToken);
                List<GamePgn> gamePgns = studyApi.GetStudyGames(x.StudyId).Result;
                PieceLocationDrawer.DrawPieceLocations(gamePgns);

                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }).WithNotParsed(x =>
            {
                Console.WriteLine("Invalid arguments...");
                Console.ReadKey();
            });
        }
    }
}
