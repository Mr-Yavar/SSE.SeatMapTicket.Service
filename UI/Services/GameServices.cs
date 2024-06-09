using SeatMapService.Models;
using System.Text.Json;

namespace SeatMapService.Services
{
    public class GameServices
    {
        public Game[] LoadGames()
        {
            using var streamReader = new StreamReader("Data/GamesData.json");
            var gamesData = streamReader.ReadToEnd();
            var games = JsonSerializer.Deserialize<Game[]>(gamesData);
            return games;
        }
    }
}
