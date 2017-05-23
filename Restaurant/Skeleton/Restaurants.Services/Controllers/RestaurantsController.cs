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
using Restaurants.Data;
using Restaurants.Models;
using Restaurants.Services.Models.BindingModels;

namespace Restaurants.Services.Controllers
{ 
    [RoutePrefix("api/restaurants")]
    public class RestaurantsController : ApiController
    {
        private RestaurantsContext context = new RestaurantsContext();

        [AllowAnonymous]
        [HttpGet]
        // GET: api/Restaurants
        public IHttpActionResult GetRestaurants([FromUri]int townId)
        {
           // if (townId == null)
           //{
           //     return this.BadRequest("Model cannot be null.");
           //}

            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }


            var allRestorantsByTownId = context.Restaurants
                .Where(r => r.TownId == townId)
                .OrderByDescending(r => r.Ratings.Average(rt => rt.Stars))
                .Select(r => new
                {
                    id = r.Id,
                    name = r.Name,
                    rating = r.Ratings,
                    town = new
                    {
                        id = r.Town.Id,
                        name = r.Town.Name
                    },
                    Rating = r.Ratings.Average(rt => rt.Stars)
                });

            return this.Ok(allRestorantsByTownId);
        }

        // GET: api/Restaurants/5
        [ResponseType(typeof(Restaurant))]
        public IHttpActionResult GetRestaurant(int id)
        {
            Restaurant restaurant = context.Restaurants.Find(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            return Ok(restaurant);
        }

        // PUT: api/Restaurants/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutRestaurant(int id, Restaurant restaurant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Restaurants
        [HttpPost]
        public IHttpActionResult PostRestaurant(CreateRestaurantBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var currentUserId = User.Identity.GetUserId();

            var newRestaurant = new Restaurant()
            {
                Name = model.Name,
                TownId = model.TownId,
                OwnerId = currentUserId 
            };

            context.Restaurants.Add(newRestaurant);
            context.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = model.TownId }, model);
        }

        // DELETE: api/Restaurants/5
        [ResponseType(typeof(Restaurant))]
        public IHttpActionResult DeleteRestaurant(int id)
        {
            Restaurant restaurant = context.Restaurants.Find(id);
            if (restaurant == null)
            {
                return NotFound();
            }

            context.Restaurants.Remove(restaurant);
            context.SaveChanges();

            return Ok(restaurant);
        }

    }
}