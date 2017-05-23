using System;
namespace Messages.Data.Models
{
    public class UserMessage
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public DateTime DateSent { get; set; }

        public virtual User Sender { get; set; }

        public string SenderId { get; set; }

        public virtual User Reciever { get; set; }

        public string RecieverId { get; set; }
    }
}
