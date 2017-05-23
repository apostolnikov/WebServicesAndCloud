using System;

namespace Messages.Data.Models
{
    public class ChannelMessage
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime DateSent { get; set; }

        public virtual User Sender { get; set; }

        public string SenderId { get; set; }
    }
}
