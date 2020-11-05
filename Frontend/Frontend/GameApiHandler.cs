using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.Json;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

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
        public async Task<int> AddGame(GameInfo game)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string AddGameUrl = BaseUrl + "AddGame";
                var dataToSend = new StringContent(JsonSerializer.Serialize(game), Encoding.UTF8, "Application/json");
                var httpResponse = await httpClient.PostAsync(AddGameUrl, dataToSend);
                return (int)httpResponse.StatusCode;
            }
        }
        public async Task<int> UpdateGame(GameInfo game)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string UpdateGameUrl = BaseUrl + "UpdateGame";
                var dataToSend = new StringContent(JsonSerializer.Serialize(game), Encoding.UTF8, "Application/json");
                var httpResponse = await httpClient.PutAsync(UpdateGameUrl, dataToSend);
                return (int)httpResponse.StatusCode;
            }
        }
        public async Task<int> DeleteGame(int Id)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string DeleteGameUrl = BaseUrl + "DeleteGame/" + Id;
                var httpResponse = await httpClient.DeleteAsync(DeleteGameUrl);
                return (int)httpResponse.StatusCode;
            }
        }
        public async Task<HttpResponseMessage> GetGameImage(int Id)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string GetGameImageUrl = BaseUrl + "GetGameImage/" + Id;
                var httpResponse = await httpClient.GetAsync(GetGameImageUrl);
                return httpResponse;
            }
        }
        public async Task<int> PostGameImage(int Id, string FilePath)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string PostGameImageUrl = BaseUrl + "SaveImage/" + Id;
                using (MultipartFormDataContent content = new MultipartFormDataContent())
                {
                    //FileStream fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.None, bufferSize: 4096, useAsync: true);
                    var image = await File.ReadAllBytesAsync(FilePath);
                    var imageName = Path.GetFileName(FilePath);
                    content.Add(new StreamContent(new MemoryStream(image)), "Image", imageName);
                    var httpResponse = await httpClient.PostAsync(PostGameImageUrl, content);
                    return (int)httpResponse.StatusCode;
                }
            }
        }
    }
}
