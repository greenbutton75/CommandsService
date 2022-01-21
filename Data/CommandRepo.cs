using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using CommandsService.Data;
using CommandsService.Models;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

namespace CommandService.Data
{
    public class CommandRepo : ICommandRepo
    {
        private readonly IConnectionMultiplexer _context;
        public CommandRepo(IConnectionMultiplexer context)
        {
            _context = context;
        }

        void ICommandRepo.CreateCommand(string platformId, Command command)
        {
            if (command == null) throw new ArgumentNullException(nameof(command));

            command.PlatformId = platformId;

            var db = _context.GetDatabase();

            var j = JsonSerializer.Serialize(command);

            db.HashSet($"platformcommands:{platformId}", new HashEntry[] { new HashEntry(command.Id, command.Id) });
            db.StringSet(command.Id, j);
            //_context.Commands.Add(command);
        }

        void ICommandRepo.CreatePlatform(Platform platform)
        {
            if (platform == null) throw new ArgumentNullException(nameof(platform));

            var db = _context.GetDatabase();

            var j = JsonSerializer.Serialize(platform);

            db.HashSet("platforms", new HashEntry[] { new HashEntry(platform.Id, platform.Id) });
            db.StringSet(platform.Id, j);
            //_context.Platforms.Add(platform);
        }

        IEnumerable<Platform?> ICommandRepo.GetAllPlatforms()
        {
            var db = _context.GetDatabase();

            var he = db.HashGetAll("platforms");
            List<Platform?> res = new List<Platform?>();

            foreach (var item in he)
            {
                var plat = db.StringGet(item.Name.ToString());
                if (!string.IsNullOrEmpty(plat)) res.Add(JsonSerializer.Deserialize<Platform>(plat));
            }

            return res;
            //return _context.Platforms.ToList();
        }

        Command? ICommandRepo.GetCommand(string platformId, string commandId)
        {
            var db = _context.GetDatabase();

            var command = db.StringGet(commandId);
            if (!string.IsNullOrEmpty(command))
            {
                return JsonSerializer.Deserialize<Command>(command);
            }

            return null;
            //return _context.Commands.Where(p => p.PlatformId == platformId && p.Id == commandId).FirstOrDefault();
        }

        IEnumerable<Command?> ICommandRepo.GetCommandsForPlatform(string platformId)
        {
            var db = _context.GetDatabase();

            var he = db.HashGetAll($"platformcommands:{platformId}");
            List<Command?> res = new List<Command?>();

            foreach (var item in he)
            {
                var cmd = db.StringGet(item.Name.ToString());
                if (!string.IsNullOrEmpty(cmd)) res.Add(JsonSerializer.Deserialize<Command>(cmd));
            }

            return res;

            //return _context.Commands.Where(p => p.PlatformId == platformId).ToList();
        }

        bool ICommandRepo.PlatformExists(string platformId)
        {
            var db = _context.GetDatabase();
            var plat = db.StringGet(platformId);
            if (!string.IsNullOrEmpty(plat))
            {
                return true;
            }

            return false;

            //return _context.Platforms.Any(p => p.Id == platformId);
        }

        bool ICommandRepo.ExternalPlatformExists(int externalPlatformId)
        {
            var db = _context.GetDatabase();

            var he = db.HashGetAll("platforms");
            List<Platform> res = new List<Platform>();

            foreach (var item in he)
            {
                var plat = db.StringGet(item.Name.ToString());
                if (!string.IsNullOrEmpty(plat))
                    if (JsonSerializer.Deserialize<Platform>(plat)?.ExternalID == externalPlatformId)
                        return true;
            }

            return false;
        }

        bool ICommandRepo.SaveChanges()
        {
            return true;
            //return (_context.SaveChanges() >= 0);
        }
    }

}