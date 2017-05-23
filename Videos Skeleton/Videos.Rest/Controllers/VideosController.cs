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
using Microsoft.AspNet.Identity;
using Videos.Data;
using Videos.Models;
using Videos.Rest.Models.BindingModels;

namespace Videos.Rest.Controllers
{
    public class VideosController : ApiController
    {
        private VideosDbContext db = new VideosDbContext();

        // GET: api/Videos
        [HttpGet]
        public IHttpActionResult GetVideos([FromUri]GetVideosBindingModel locationId)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var allVideosByLocationId = db.Videos
                .Where(v => v.LocationId == locationId.LocationId && v.Status == VideoStatus.Published)
                .OrderBy(v => v.Owner.UserName)
                .ThenBy(v => v.Title)
                .Select(v => new
                {
                    id = v.Id,
                    title = v.Title,
                    country = v.Location.Country,
                    city = v.Location.City,
                    owner = v.Owner.UserName
                });

            return Ok(allVideosByLocationId);
        }


        // POST: api/Videos
        [HttpPost]
        public IHttpActionResult PostVideo([FromBody]CreateVideoBindingModel model)
        {
            if (model == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
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

            var video = new Video()
            {
                Title = model.Title,
                LocationId = model.LocationId,
                OwnerId = userId,
                Status = VideoStatus.Pending,
                PostedOn = DateTime.Now
            };

            db.Videos.Add(video);
            db.SaveChanges();

            return this.Ok(new
            {
                Id = video.Id,
                Title = video.Title,
                status = video.Status,
                country = video.Location.Country,
                City = video.Location.City,
                Owner = video.Owner.UserName
            });
        }

        [HttpPost]
        [Route("api/videos/{id}/addTag")]
        public IHttpActionResult AddTagToVideo([FromUri] int id, [FromBody] AddTagBindingModel model)
        {
            var currentLoggedUserId = User.Identity.GetUserId();

            var currentVideo = db.Videos.Find(id);

            var currentTag = db.Tags.Find(model.TagId);

            if (currentVideo == null)
            {
                return NotFound(); 
            }

            if (currentTag == null)
            {
                return NotFound();
            }

            if (currentLoggedUserId == null)
            {
                return Unauthorized();
            }

            currentVideo.Tags.Add(currentTag);
            db.SaveChanges();
            return this.Ok();
        }

    }
}