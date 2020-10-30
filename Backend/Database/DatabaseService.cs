using System;
using Microsoft.Data.Sqlite;
using Dapper;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Backend.Models;
using System.Collections.Generic;

namespace Backend.Database
{
    public class DatabaseService : IDatabaseService
    {
        private DatabaseConfig databaseConfig;
        public DatabaseService(DatabaseConfig dbConfig)
        {
            databaseConfig = dbConfig;
        }

        public void Setup()
        {
            using(SqliteConnection connection = new SqliteConnection(databaseConfig.Name))
            {
                var table = connection.Query<string>("SELECT Name FROM sqlite_master WHERE type='table' AND name = 'Games'");
                var tableName = table.FirstOrDefault();
                if(!string.IsNullOrEmpty(tableName) && tableName == "Games")
                {
                    return;
                }
                using(var sr = new StreamReader(databaseConfig.StructureFile)) 
                {
                        var queries = sr.ReadToEnd();
                        connection.Execute(queries);
                }
            }
        }

        public async Task<GameInfo> UpdateGame(GameInfo game)
        {   //This is why you don't code for 10 hours in a row, you do stupid stuff and think it's good when there are much simpler solutions like this...
            if (game.Id > -1)
            {
                using (var Connection = new SqliteConnection(databaseConfig.Name))
                {
                    int res = await Connection.ExecuteAsync("UPDATE Games SET Name=@Name, Description=@Description, Image=@Image, Grade=@Grade WHERE Id=@Id", game);
                    game.Id = res;
                }
            }
            else game.Id = -1;
            return game;
        }

        // This is now obsolete code as I've come up with a better way to do this, keeping it as an example of what to NOT do
        // public async Task<GameInfo> UpdateGame(GameInfo game)
        // {
        //     int UpdateThis = UpdateWhat(game);
        //     using (var Connection = new SqliteConnection(databaseConfig.Name))
        //     {
        //         int res;
        //         //Switch statement wizardry, see UpadeWhat() for further description.
        //         switch (UpdateThis)
        //         {   //1000 is update name, 0100 is update description, 0010 is update image, 0001 is update grade. Combine them, for example: 1010 would be both 1000 and 0010
        //             case 1111:
        //                 res = await Connection.ExecuteAsync("UPDATE Games SET Name=@Name, Description=@Description, Image=@Image, Grade=@Grade WHERE Id=@Id", game);
        //                 break;
        //             case 1110:
        //                 res = await Connection.ExecuteAsync("UPDATE Games SET Name=@Name, Description=@Description, Image=@Image WHERE Id=@Id", game);
        //                 break;
        //             case 1101:
        //                 res = await Connection.ExecuteAsync("UPDATE Games SET Name=@Name, Description=@Description, Grade=@Grade WHERE Id=@Id", game);
        //                 break;
        //             case 1011:
        //                 res = await Connection.ExecuteAsync("UPDATE Games SET Name=@Name, Image=@Image, Grade=@Grade WHERE Id=@Id", game);
        //                 break;
        //             //Equivalent to 0111
        //             case 111:
        //                 res = await Connection.ExecuteAsync("UPDATE Games SET Description=@Description, Image=@Image, Grade=@Grade WHERE Id=@Id", game);
        //                 break;
        //             case 1100:
        //                 res = await Connection.ExecuteAsync("UPDATE Games SET Name=@Name, Description=@Description, WHERE Id=@Id", game);
        //                 break;
        //             case 1001:
        //                 res = await Connection.ExecuteAsync("UPDATE Games SET Name=@Name, Grade=@Grade WHERE Id=@Id", game);
        //                 break;
        //             //Equivalent to 0011
        //             case 11:
        //                 res = await Connection.ExecuteAsync("UPDATE Games SET Image=@Image, Grade=@Grade WHERE Id=@Id", game);
        //                 break;
        //             case 1000:
        //                 res = await Connection.ExecuteAsync("UPDATE Games SET Name=@Name WHERE Id=@Id", game);
        //                 break;
        //             //Equivalent to 0100
        //             case 100:
        //                 res = await Connection.ExecuteAsync("UPDATE Games SET Description=@Description WHERE Id=@Id", game);
        //                 break;
        //             //Equivalent to 0010
        //             case 10:
        //                 res = await Connection.ExecuteAsync("UPDATE Games SET Image=@Image WHERE Id=@Id", game);
        //                 break;
        //             //Equivalent to 0001
        //             case 1:
        //                 res = await Connection.ExecuteAsync("UPDATE Games SET Grade=@Grade WHERE Id=@Id", game);
        //                 break;
        //             //Equivalent to 0000, change nothing. Through searching I've stumbled upon that the int return value is the number of modified rows.
        //             //So setting it to 0 is the equivalent of nothing changing, I also set the id to -1 to signify that the user didn't specify anything to change.
        //             //Though it should never get this far it's good to be prepared just in case...
        //             default:
        //                 res = 0;
        //                 game.Id = -1;
        //                 break;
        //         }
        //         if (game.Id > -1)
        //         {
        //             var LastUpdate = await Connection.QueryAsync<GameInfo>("SELECT Id FROM Games WHERE Id=@Id ORDER BY Id DESC", game);
        //             game = LastUpdate.FirstOrDefault();
        //         }
        //         return game;
        //     }
        // }
        // private int UpdateWhat(GameInfo game)
        // {   //Shut up wizard, mundane is fine
        //     //Wizardry Time! By doing this I basically create an int that can be interpreted as binary where 1 is true and 0 is false.
        //     //I did this to be able to use a switch statement instead of a horrendous 2^4 if statements since you can't switch with an array of bools or ints.
        //     //1000 is update name, 0100 is update description, 0010 is update image, 0001 is update grade. Combine for full extent of changes.
        //     int UpdateThis = 0;
        //     if (game.Name != null && game.Name != "")
        //     {   //Set name
        //         UpdateThis += 1000;
        //     }
        //     if (game.Description != null && game.Description != "")
        //     {   //Set Description
        //         UpdateThis += 100;
        //     }
        //     if (game.Image != null && game.Image != "")
        //     {   //Set Image
        //         UpdateThis += 10;
        //     }
        //     if (game.Grade > -1)
        //     {   //Set Grade
        //         UpdateThis += 1;
        //     }
        //     return UpdateThis;
        // }
        public async Task<GameInfo> AddGame(GameInfo game)
        {
            using (var Connection = new SqliteConnection(databaseConfig.Name))
            {
                var res = await Connection.ExecuteAsync("INSERT INTO Games (Name, Description, Image, Grade) VALUES (@Name, @Description, @Image, @Grade)", game);
                var lastInsert = await Connection.QueryAsync<GameInfo>("SELECT Id, Name, Description, Image, Grade FROM Games ORDER BY Id DESC");
                game.Id = lastInsert.FirstOrDefault<GameInfo>().Id;
                return game;
            }
        }
        public async Task<IEnumerable<GameInfo>> Get()
        {
            using (var Connection = new SqliteConnection(databaseConfig.Name))
            {
                var res = await Connection.QueryAsync<GameInfo>("SELECT Id, Name, Description, Image, Grade FROM Games ORDER BY Id DESC");
                return res;
            }
        }
        public async Task<IEnumerable<GameInfo>> Get(string Name)
        {
            using (var Connection = new SqliteConnection(databaseConfig.Name))
            {
                var res = await Connection.QueryAsync<GameInfo>("SELECT Id, Name, Description, Image, Grade FROM Games WHERE Name=@Name", new { Name });
                return res;
            }
        }
        public async Task<GameInfo> Get(int Id)
        {
            using (var Connection = new SqliteConnection(databaseConfig.Name))
            {
                var res = await Connection.QueryAsync<GameInfo>("SELECT Id, Name, Description, Image, Grade FROM Games WHERE Id=@Id", new { Id });
                return res.FirstOrDefault();
            }
        }
        public async Task<bool> RemoveGame(int Id)
        {
            using (var Connection = new SqliteConnection(databaseConfig.Name))
            {
                var res = await Connection.ExecuteAsync("DELETE FROM Games WHERE Id=@Id", new { Id });
                if (res > -1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
