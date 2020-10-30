using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Backend.Models;

namespace Backend.Repository
{
    public class ImageRepository : IImageRepository
    {
        private IGameRepository gr;
        private readonly IWebHostEnvironment env;
        private Dictionary<string, string> MimeTypes = new Dictionary<string, string>()
        {
            { "jpeg", "image/jpeg" },
            { "jpg", "image/jpeg" },
            { "png", "image/png" },
            { "gif", "image/gif" },
        };
        public ImageRepository(IGameRepository repo, IWebHostEnvironment IHostEnv)
        {
            gr = repo;
            env = IHostEnv;
        }

        /**
        * A method to get ImageInfo from a game id.
        * Returns a ImageInfo with the source to the image
        * on disk and also a content type for the image
        * can be used with the PhysicalFile() method in a controller
        * to return a image to the user.
        */
        public async Task<ImageInfo> GetImage(int id)
        {
            var gameInfo = await gr.Get(id);
            if(gameInfo == null) 
            {                
                throw new ArgumentException(string.Format("Game with id: {0} was not found", id));
            }
            var ext = Path.GetExtension(gameInfo.Image);
            var imgSrc = Path.Combine(env.ContentRootPath, gameInfo.Image);
            if(System.IO.File.Exists(imgSrc)) 
            {
                var fileType = ext.Replace(".", "");
                if(MimeTypes.ContainsKey(fileType)) 
                {
                    return new ImageInfo { ImgSrc = imgSrc, ImgType = MimeTypes[fileType] };
                }
                else 
                {
                    throw new ArgumentException("File not found or wrong file type.");
                }
            }
            else 
            {
                throw new ArgumentException("File not found or wrong file type.");
            }
        }

        /**
        * A method to Save a image to disk
        * Recieves a GameId and the File the user posted 
        * returns the updated information about the file.
        */
        public async Task<GameInfo> SaveImage(int GameId, IFormFile file)
        {
            GameInfo savedInfo = await gr.Get(GameId);
            if(savedInfo == null) 
            {
                throw new ArgumentException(string.Format("Game with id {0} does not exists", GameId));
            }
            var ext = Path.GetExtension(file.FileName);
            if(!MimeTypes.ContainsKey(ext.Replace(".", ""))) 
            {
                throw new ArgumentException("Wrong type of file, only png, jpg and gif are supported.");
            }
            string fName = GameId + ext;
            string path = Path.Combine(env.ContentRootPath, "Uploads\\" + fName);
            using (var stream = new FileStream(path, FileMode.OpenOrCreate))
            {
                await file.CopyToAsync(stream);
            }
            string url = @"Uploads\" + fName;
            savedInfo.Image = url;
            await gr.Update(savedInfo);
            return savedInfo;
        }
    }
}