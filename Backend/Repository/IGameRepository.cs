using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Models;

namespace Backend.Repository
{
    public interface IGameRepository
    {
        Task<GameInfo> Add(GameInfo game);
        Task<GameInfo> Update(GameInfo game);
        Task<GameInfo> Get(int Id);
        Task<IEnumerable<GameInfo>> Get();
        Task<bool> Delete(int id);
    }
}