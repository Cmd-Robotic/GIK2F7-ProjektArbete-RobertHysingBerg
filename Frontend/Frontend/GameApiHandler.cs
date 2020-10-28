using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Net.Http;

namespace Frontend
{
    class GameApiHandler
    {
        private string BaseUrl;
        public GameApiHandler(string Url)
        {
            BaseUrl = Url;
        }
        public GameInfo GetGame(int Id)
        {
            GameInfo Game = null;
            using (WebClient webClient = new WebClient())
            {
                string GetGameUrl = BaseUrl + "GetGame/" + Id.ToString();
                var jsonData = webClient.DownloadString(GetGameUrl);
                Game = JsonSerializer.Deserialize<GameInfo>(jsonData);
            }
            return Game;
        }
        public ObservableCollection<GameInfo> GetAllGames()
        {
            ObservableCollection<GameInfo> games = null;
            using (WebClient webClient = new WebClient())
            {
                string GetGamesUrl = BaseUrl + "GetGames";
                var jsonData = webClient.DownloadString(GetGamesUrl);
                games = JsonSerializer.Deserialize<ObservableCollection<GameInfo>>(jsonData);
            }
            return games;
        }
        public async void AddGame(GameInfo game)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string AddGameUrl = BaseUrl + "AddGame";
                var dataToSend = new StringContent(JsonSerializer.Serialize(game), Encoding.UTF8, "Application/json");
                var jsonData = await httpClient.PostAsync(AddGameUrl, dataToSend);
            }
        }
        public async void UpdateGame(GameInfo game)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string UpdateGameUrl = BaseUrl + "UpdateGame";
                var dataToSend = new StringContent(JsonSerializer.Serialize(game), Encoding.UTF8, "Application/json");
                var jsonData = await httpClient.PutAsync(UpdateGameUrl, dataToSend);
            }
        }
        public async void DeleteGame(int Id)
        {
            bool deleted = false;
            using (HttpClient httpClient = new HttpClient())
            {
                string DeleteGameUrl = BaseUrl + "DeleteGame/" + Id;
                var jsonData = await httpClient.DeleteAsync(DeleteGameUrl);
            }
        }
    }
}
