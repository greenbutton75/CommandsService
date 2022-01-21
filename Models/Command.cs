using System;
using System.ComponentModel.DataAnnotations;

namespace CommandsService.Models
{
    public class Command
    {
        [Key]
        [Required]
        public string Id { get; set; } = $"command:{Guid.NewGuid().ToString()}";

        [Required]
        public string HowTo { get; set; } = String.Empty;

        [Required]
        public string CommandLine { get; set; } = String.Empty;

        [Required]
        public string PlatformId { get; set; } = String.Empty;

        public Platform Platform { get; set; } = new Platform();
    }
}