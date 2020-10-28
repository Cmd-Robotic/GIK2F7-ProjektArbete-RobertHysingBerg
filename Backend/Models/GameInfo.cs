using Microsoft.AspNetCore.Http;

namespace Backend.Models
{
    public class GameInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Grade { get; set; }
        public string Image { get; set; }
    }

    public class PostGame 
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Grade { get; set; }
    }
    
    public class PostGameImage 
    {
        public int Id { get; set; }
        public IFormFile PostImage { get; set; }
    }
}