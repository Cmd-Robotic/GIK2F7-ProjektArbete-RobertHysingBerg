using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;

namespace Backend.Repository
{
    public interface IImageRepository
    {
        Task<GameInfo> SaveImage(int GameId, IFormFile file);
        Task<ImageInfo> GetImage(int id);
    }

    public class ImageInfo 
    {
        public string ImgSrc { get; set; }
        public string ImgType { get; set; }
    }
}