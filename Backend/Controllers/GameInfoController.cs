using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend.Models;
using Backend.Database;
using Backend.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.AspNetCore.Hosting;
using System.ComponentModel;

namespace Backend.Controllers
{
    [ApiController]
    [Route("games")]
    public class GameInfoController : ControllerBase
    {

        private readonly IGameRepository gameRepository;
        private readonly IImageRepository imageRepository;
        private readonly IWebHostEnvironment env;
        
        /**
        * To use IGameRepository, we need to tell dotnet to use dependency injection
        * to inject the repositories we need. 
        * just like we inject IWebHostEnvironment IHostEnv in the constructor right now.
        */
        public GameInfoController(IWebHostEnvironment IHostEnv, IGameRepository gameRepository, IImageRepository imageRepository)
        {
            env = IHostEnv;
            this.gameRepository = gameRepository;
            this.imageRepository = imageRepository;
        }

        //IGameRepository is a repo to handle game information,
        //The usual CRUD operations is awailable
        [HttpGet("GetGames")]
        public async Task<IEnumerable<GameInfo>> GetAll()
        {
            return await gameRepository.Get();
        }
        [HttpGet("GetGame/{Id}")]
        public async Task<GameInfo> GetGame(int Id)
        {
            return await gameRepository.Get(Id);
        }
        [HttpPost("AddGame")]
        public async Task<GameInfo> AddGame(GameInfo NewGame)
        {
            return await gameRepository.Add(NewGame);
        }
        [HttpPut("UpdateGame")]
        public async Task<GameInfo> UpdateGame(GameInfo UpdatedGame)
        {
            return await gameRepository.Update(UpdatedGame);
        }
        [HttpDelete("DeleteGame/{Id}")]
        public async Task<bool> DeleteGame(int Id)
        {
            return await gameRepository.Delete(Id);
        }

        //IImageRepository is a repo to handle upload and fetching
        //image from the api.
        [HttpGet("GetGameImage/{Id}")]
        public async Task<IActionResult> GetImage(int Id)
        {   //Efter att sett de andra presentera förstår jag nu vad ImageInfo faktiskt gör, detta är så mycket enklare än jag trodde.
            //Even clearer now that I look at the comments below...
            GameInfo game = await gameRepository.Get(Id);
            if (game == null)
            {
                throw new ArgumentException("Felaktigt id");
            }
            ImageInfo PathAndType = await imageRepository.GetImage(Id);
            if (System.IO.File.Exists(PathAndType.ImgSrc))
            {
                return PhysicalFile(PathAndType.ImgSrc, PathAndType.ImgType);
            }
            else
            {
                throw new FileNotFoundException();
            }
        }
        [HttpPost("SaveImage/{Id}")]
        public async Task<GameInfo> SaveImage(int Id,[FromForm] IFormFile Image)
        {
            return await imageRepository.SaveImage(Id, Image);
        }

        //To recieve a image from post we can use FromForm
        //PostGameImage is defined in Models/GameInfo.cs
        // public async Task<GameInfo> AddGameWithImage([FromForm] PostGameImage GameInfo)
        // {
        // }
        
        //To send a image as response we can return a PhysicaFile
        // public async Task<IActionResult> GetImage(int Id)
        // {
        //     return PhysicalFile(k.ImgSrc, k.ImgType);
        // }
    }
}
