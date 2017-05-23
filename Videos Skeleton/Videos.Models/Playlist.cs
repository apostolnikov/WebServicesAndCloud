using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Videos.Models
{
    public class Playlist
    {
        public Playlist()
        {
            this.Tags = new HashSet<Tag>();
            this.Videos = new HashSet<Video>();
        }
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string OwnerId { get; set; }

        public virtual ApplicationUser Owner { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }

        public virtual ICollection<Video> Videos { get; set; }
    }
}