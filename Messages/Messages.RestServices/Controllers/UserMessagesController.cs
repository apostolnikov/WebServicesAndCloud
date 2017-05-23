using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Messages.Data;
using Messages.Data.Models;
using Messages.RestServices.BindingModels;
using Messages.RestServices.ViewModels;
using Microsoft.AspNet.Identity;

namespace Messages.RestServices.Controllers
{
    [RoutePrefix("api/user")]
    public class UserMessagesController : ApiController
    {
        private MessagesDbContext db = new MessagesDbContext();

        [Route("personal-messages")]

        public IHttpActionResult GetPersonalMessages()
        {
            var userId = User.Identity.GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }
            var user = db.Users.Find(userId);

            if (user == null)
            {
                return Unauthorized();
            }

            var messages = db.UserMessages
                .Where(um => um.RecieverId == user.Id)
                .OrderByDescending(um => um.DateSent)
                .Select(um => new UserMessageViewModel()
                {
                    Id = um.Id,
                    Text = um.Text,
                    DateSent = um.DateSent,
                    Sender = um.Sender == null ? null : um.Sender.UserName
                });
            return this.Ok(messages);
        }

        [Route("personal-messages")]
        public IHttpActionResult PostPersonalMessage([FromBody] UserMessageBindingModel userMessageModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (userMessageModel == null)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(userMessageModel.Text))
            {
                return BadRequest();
            }

            var recipient = db.Users.FirstOrDefault(u => u.UserName == userMessageModel.Recipient);

            if (recipient == null)
            {
                return NotFound();
            }

            var userMessage = new UserMessage()
            {
                Text = userMessageModel.Text,
                DateSent = DateTime.Now,
                Reciever = recipient,
                Sender = null
            };

            db.UserMessages.Add(userMessage);
            db.SaveChanges();

            return this.Ok(new
            {
                Id = userMessage.Id,
                Message = "Anonymous message sent successfully to user " + userMessageModel.Recipient + "."
            });
        }
    }
}
