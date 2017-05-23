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
    public class TagsController : ApiController
    {
        private VideosDbContext db = new VideosDbContext();

        // PUT: api/Tags/5
        [HttpPut]
        public IHttpActionResult PutTag([FromUri]int id, [FromBody]CreateTagBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model == null)
            {
                return BadRequest();
            }

            var dbTag = db.Tags.Find(id);

            if (dbTag == null)
            {
                return NotFound();
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

            dbTag.Name = model.Name;
            dbTag.IsAdultContent = model.IsAdultContent;
            db.SaveChanges();

            return this.Ok(new
            {
                id = dbTag.Id,
                name = dbTag.Name,
                isAdultContent = dbTag.IsAdultContent
            });



        }

        // POST: api/Tags
        [HttpPost]
        public IHttpActionResult PostTag([FromBody]CreateTagBindingModel model)
        {
            var userId = User.Identity.GetUserId();

            if (model == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (db.Tags.Any(t => t.Name == model.Name))
            {
                return BadRequest();
            }

            var newTag = new Tag()
            {
                Name = model.Name,
                IsAdultContent = model.IsAdultContent,
                OwnerId = userId

            };

            db.Tags.Add(newTag);
            db.SaveChanges();

            return this.Ok(new
            {
                id = newTag.Id,
                name = newTag.Name,
                isAdultContent = newTag.IsAdultContent
            });
        }

        // DELETE: api/Tags/5
        [HttpDelete]
        public IHttpActionResult DeleteTag([FromUri]int id)
        {
            var currentTag = db.Tags.Find(id);

            var currentUser = User.Identity.GetUserId();

            if (currentTag == null)
            {
                return this.NotFound();
            }


            if (currentUser != currentTag.OwnerId)
            {
                this.Unauthorized();
            }

            var videoWithSameId = db.Videos.Select(v => v.Tags.Select(t => t.Id)).ToString();

            if (videoWithSameId == id.ToString())
            {
                return this.Conflict();
            }

            db.Tags.Remove(currentTag);
            db.SaveChanges();

            return this.Ok(new
            {
                message = "Tag #" + currentTag.Id + " deleted"
            });
        }
    }
}