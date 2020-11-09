using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Backend.Models;
using Backend.Database;

namespace Backend.Repository
{
    public class GameRepository : IGameRepository
    {
        private DatabaseService DBService;
        /* Unnecessary and unused, so commented it out
        public  GameRepository()
        {
            DatabaseConfig DBConfig = new DatabaseConfig();
            DBConfig.Name = "Data source=./Database/GamesDB.sqlite";
            DBConfig.StructureFile = "./Database/DatabaseStructure.sql";
            DBService = new DatabaseService(DBConfig);
        }*/
        public  GameRepository(DatabaseConfig DBConfig)
        {
            DBService = new DatabaseService(DBConfig);
        }
        public  Task<GameInfo> Add(GameInfo game)
        {
            return DBService.AddGame(game);
        }
        public  Task<GameInfo> Update(GameInfo game)
        {
            return DBService.UpdateGame(game);
        }
        public  Task<GameInfo> Get(int Id)
        {
            return DBService.Get(Id);
        }
        public  Task<IEnumerable<GameInfo>> Get()
        {
            return DBService.Get();
        }
        public  Task<bool> Delete(int Id)
        {
            return DBService.RemoveGame(Id);
        }
    }
}
