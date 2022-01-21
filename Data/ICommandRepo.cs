using System.Collections.Generic;
using CommandsService.Models;

namespace CommandService.Data
{
    public interface ICommandRepo
    {
        bool SaveChanges();

        IEnumerable<Platform> GetAllPlatforms();
        void CreatePlatform(Platform platform);
        bool PlatformExists(string platformId);
        
        IEnumerable<Command?>? GetCommandsForPlatform(string platformId);
        Command? GetCommand(string platformId, string commandId);
        void CreateCommand(string platformId, Command command);

    }

}