using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Videos.Rest.Models.BindingModels
{
    public class CreateVideoBindingModel
    {
        public string Title { get; set; }

        public int LocationId { get; set; }
    }
}