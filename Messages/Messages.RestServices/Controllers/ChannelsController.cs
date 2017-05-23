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
using Messages.RestServices.ViewModels;

namespace Messages.RestServices.Controllers
{
    [RoutePrefix("api/channels")]
    public class ChannelsController : ApiController
    {
        private MessagesDbContext db = new MessagesDbContext();

        // GET: api/Channels
        public IQueryable<ChannelViewModel> GetChannels()
        {
            return db.Channels.OrderBy(c => c.Name).Select(c => new ChannelViewModel
            {
                Id = c.Id,
                Name = c.Name
            });
        }

        
        // GET: api/Channels/5
        [ResponseType(typeof(ChannelViewModel))]
        public IHttpActionResult GetChannel(int id)
        {
            Channel channel = db.Channels.Find(id);
            if (channel == null)
            {
                return NotFound();
            }

            return Ok(new ChannelViewModel()
            {
                Id = channel.Id,
                Name = channel.Name
            });
        }

        // PUT: api/Channels/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutChannel(int id, Channel channel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (channel == null)
            {
                return BadRequest();
            }

            var dbChannel = db.Channels.Find(id);

            if (dbChannel == null)
            {
                return NotFound();
            }

            if (db.Channels.Any(c => c.Name == channel.Name && c.Id != id))
            {
                return Conflict();
            }

            dbChannel.Name = channel.Name;
            db.SaveChanges();
            return this.Ok(new
            {
                Message = "Channel #" + dbChannel.Id +" edited successfully."
            });


        }

        // POST: api/Channels
        [ResponseType(typeof(ChannelViewModel))]
        public IHttpActionResult PostChannel(Channel channel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (channel == null)
            {
                return BadRequest();
            }

            if (db.Channels.Any(c => c.Name == channel.Name))
            {
                return Conflict();
            }

            db.Channels.Add(channel);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = channel.Id }, 
                new ChannelViewModel()
                {
                    Id = channel.Id,
                    Name = channel.Name
                });
        }

        // DELETE: api/Channels/5
        [ResponseType(typeof(Channel))]
        public IHttpActionResult DeleteChannel(int id)
        {
            Channel channel = db.Channels.Find(id);
            if (channel == null)
            {
                return NotFound();
            }

            if (channel.ChannelMessages.Any())
            {
                return ResponseMessage(
                    Request.CreateResponse
                    (HttpStatusCode.Conflict, 
                    new
                    {
                        Message = "Cannot delete channel #" + channel.Id + " because it is not empty."
                    }));
            }

            db.Channels.Remove(channel);
            db.SaveChanges();

            return Ok(new
            {
                Message = "Channel #" + channel.Id +" deleted."
            });
        }

        private bool ChannelExists(int id)
        {
            return db.Channels.Count(e => e.Id == id) > 0;
        }
    }
}