using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Database
{
    public interface IDatabaseService
    {
        void Setup();
        Task<GameInfo> AddGame(GameInfo game);
        Task<GameInfo> UpdateGame(GameInfo game);
        Task<IEnumerable<GameInfo>> Get();
        Task<GameInfo> Get(int id);
        Task<bool> RemoveGame(int id);
    }
}
