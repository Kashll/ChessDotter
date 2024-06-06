using System;
using pax.chess;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChessDotter;

public class StudyApi
{
    private readonly HttpClient _lichessClient = new HttpClient();

    private const string StudyEndpoint = @"https://lichess.org/api/study/";

    public StudyApi(string personalAccessToken)
    {
        _lichessClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", personalAccessToken);
    }

    public Task<List<string>> GetStudyPgns(string studyId)
    {
        string endpoint = $"{StudyEndpoint}{studyId}.pgn";
        return _lichessClient.GetStringAsync(endpoint).ContinueWith(x => SplitPgnResponse(x.Result));
    }

    private static List<string> SplitPgnResponse(string pgnResponse)
    {
        return pgnResponse.Split("\n\n\n", StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}