using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Messages.Data;
using Messages.Data.Models;
using Messages.RestServices.BindingModels;
using Messages.RestServices.ViewModels;
using Microsoft.AspNet.Identity;

namespace Messages.RestServices.Controllers
{
    [RoutePrefix("api/channel-messages")]

    public class ChannelMessagesController : ApiController
    {
        private MessagesDbContext db = new MessagesDbContext();

        [Route("{channelName}")]
        // GET: api/ChannelMessages
        public IHttpActionResult GetChannelMessagesByName(string channelName)
        {
            var channel = db.Channels.FirstOrDefault(c => c.Name == channelName);
            
            if (channel == null)
            {
                return NotFound();
            }

            var messages = channel
                .ChannelMessages
                .OrderByDescending(cm => cm.DateSent)
                .Select(cm => new ChannelMessagesViewModel()
                {
                    Id = cm.Id,
                    Text = cm.Text,
                    DateSent = cm.DateSent,
                    Sender = cm.Sender == null ? null : cm.Sender.UserName
                });

            return Ok(messages);
        }

        [Route("{channelName}")]
        // GET: api/ChannelMessages
        public IHttpActionResult GetChannelMessagesByName(string channelName, [FromUri]string limit)
        {
            var channel = db.Channels.FirstOrDefault(c => c.Name == channelName);
            
            var userId = User.Identity.GetUserId();
            var user = db.Users.Find(userId);
            
            if (channel == null)
            {
                return NotFound();
            }

            int messagesLimit = 0;

            try
            {
                messagesLimit = int.Parse(limit);
            }
            catch (FormatException ex)
            {
                return BadRequest();
            }

            if (messagesLimit <= 0 || messagesLimit > 1000)
            {
                return BadRequest();
            }

            var messages = channel
                .ChannelMessages
                .OrderBy(cm => cm.Text)
                .ThenByDescending(cm => cm.DateSent)
                .Select(cm => new ChannelMessagesViewModel()
                {
                    Id = cm.Id,
                    Text = cm.Text,
                    DateSent = cm.DateSent,
                    Sender = cm.Sender == null ? null : cm.Sender.UserName
                })
                .Take(messagesLimit);

            return Ok(messages);
        }

        // POST: api/ChannelMessages
        [Route("{channelName}")]
        public IHttpActionResult PostChannelMessage(string channelName, [FromBody] ChannelMessageBindingModel channelMessageModel)
        {
            var userId = User.Identity.GetUserId();
            
            var channel = db.Channels.FirstOrDefault(c => c.Name == channelName);
            User user = null;
            if (userId != null)
            {
                user = db.Users.Find(userId);
            }

            if (channel == null)
            {
                return NotFound();
            }

            if (channelMessageModel == null)
            {
                return BadRequest("Null text");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (user == null && userId != null)
            {
                return Unauthorized();    
            }

            var channelMessage = new ChannelMessage()
            {
                Text = channelMessageModel.Text,
                DateSent = DateTime.Now,
                Sender = user
            };

            channel.ChannelMessages.Add(channelMessage);

            db.SaveChanges();

            if (user == null)
            {
                return this.Ok(new
                {
                    Id = channelMessage.Id,
                    Message = "Anonymous message sent to channel " + channel.Name + "."
                });
            }

            return this.Ok(new
            {
                Id = channelMessage.Id,
                Sender = user.UserName,
                Message = "Message sent to channel " + channel.Name + "."
            });
        }

    }
}