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
using System.Web.Routing;
using Microsoft.AspNet.Identity;
using Videos.Data;
using Videos.Models;
using Videos.Rest.Models.BindingModels;

namespace Videos.Rest.Controllers
{
    public class PlaylistsController : ApiController
    {
        private VideosDbContext db = new VideosDbContext();

        // POST: api/Playlists
        [HttpPost]
        public IHttpActionResult PostPlaylist([FromBody] CreatePlaylistBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model == null)
            {
                return BadRequest();
            }

            var userId = User.Identity.GetUserId();
            ApplicationUser user = null;
            if (userId != null)
            {
                user = db.Users.Find(userId);
            }

            if (user == null && userId != null)
            {
                return Unauthorized();
            }

            var newPlaylist = new Playlist()
            {
                Name = model.Name,
                OwnerId = userId

            };

            db.Playlists.Add(newPlaylist);
            db.SaveChanges();

            return this.Ok(new
            {
                id = newPlaylist.Id,
                Name = newPlaylist.Name,
                Owner = newPlaylist.Owner.UserName,
                tags = newPlaylist.Tags.Select(t => new
                {
                    name = t.Name,
                    isAdultContent = t.IsAdultContent
                }),
                videos = newPlaylist.Videos.Select(v => new
                {
                    title = v.Title,
                    status = v.Status

                })
            });
        }

        [Route("api/playlists/{id}/addVideo")]
        public IHttpActionResult AddVideoToPlaylist([FromUri]int id, [FromBody]AddVideoBindingModel model)
        {
            var currentUserId = User.Identity.GetUserId();
            var currentPlaylist = db.Playlists.Find(id);

            var currentVideo = db.Videos.Find(model.VideoId);
            var currentVideoId = model.VideoId;

            if (currentPlaylist == null)
            {
                return this.NotFound();
            }

            if (currentUserId == null)
            {
                return this.Unauthorized();
            }

            currentPlaylist.Videos.Add(currentVideo);
            db.SaveChanges();
            return this.Ok();
        }
    }
}
