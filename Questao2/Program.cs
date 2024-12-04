using Newtonsoft.Json;

public class Program
{
    public static void Main()
    {
        string teamName = "Paris Saint-Germain";
        int year = 2013;
        int totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        teamName = "Chelsea";
        year = 2014;
        totalGoals = getTotalScoredGoals(teamName, year);

        Console.WriteLine("Team " + teamName + " scored " + totalGoals.ToString() + " goals in " + year);

        // Output expected:
        // Team Paris Saint - Germain scored 109 goals in 2013
        // Team Chelsea scored 92 goals in 2014
    }

    public static int getTotalScoredGoals(string team, int year)
    {
        int totalGoals = 0;
        int currentPage = 1;
        bool hasMorePages = true;

        using (HttpClient client = new HttpClient())
        {
            while (hasMorePages)
            {
                string apiUrlTeam1 = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team1={team}&page={currentPage}";
                string apiUrlTeam2 = $"https://jsonmock.hackerrank.com/api/football_matches?year={year}&team2={team}&page={currentPage}";

                // Get matches where the team is team1
                var responseTeam1 = client.GetStringAsync(apiUrlTeam1).Result;
                var dataTeam1 = JsonConvert.DeserializeObject<ApiResponse>(responseTeam1);

                foreach (var match in dataTeam1.data)
                {
                    totalGoals += int.Parse(match.team1goals);
                }

                // Get matches where the team is team2
                var responseTeam2 = client.GetStringAsync(apiUrlTeam2).Result;
                var dataTeam2 = JsonConvert.DeserializeObject<ApiResponse>(responseTeam2);

                foreach (var match in dataTeam2.data)
                {
                    totalGoals += int.Parse(match.team2goals);
                }

                // Determine if more pages exist
                hasMorePages = dataTeam1.page < dataTeam1.total_pages || dataTeam2.page < dataTeam2.total_pages;
                currentPage++;
            }
        }

        return totalGoals;
    }
}
