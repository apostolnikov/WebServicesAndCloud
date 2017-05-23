using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Videos.Models
{
    public class Video
    {
        public Video()
        {
            this.Tags = new HashSet<Tag>();
        }

        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public VideoStatus Status { get; set; }

        [Required]
        public int LocationId { get; set; }

        public virtual Location Location { get; set; }

        [Required]
        public string OwnerId { get; set; }

        public virtual ApplicationUser Owner { get; set; }

        public DateTime PostedOn { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }

    }
}
