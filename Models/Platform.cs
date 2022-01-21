using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CommandsService.Models
{
    public class Platform
    {
        [Required]
        public string Id { get; set; } = $"platform:{Guid.NewGuid().ToString()}";

        [Required]
        public int ExternalID { get; set; }

        [Required]
        public string Name { get; set; } = String.Empty;

        public ICollection<Command> Commands { get; set; } = new List<Command>();
     }
}