using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Videos.Rest.Models.BindingModels
{
    public class CreateTagBindingModel
    {
        public string Name { get; set; }

        public bool IsAdultContent { get; set; }
    }
}