using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Videos.Models
{
    public class Tag
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsAdultContent { get; set; }

        [Required]
        public string OwnerId { get; set; }

        public virtual ApplicationUser Owner { get; set; }
    }
}