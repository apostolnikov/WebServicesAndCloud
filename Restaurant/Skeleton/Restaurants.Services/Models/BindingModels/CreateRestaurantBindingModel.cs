using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurants.Services.Models.BindingModels
{
    public class CreateRestaurantBindingModel
    {
        public string Name { get; set; }

        public int TownId { get; set; }
    }
}