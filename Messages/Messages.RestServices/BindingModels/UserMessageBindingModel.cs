using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Messages.RestServices. BindingModels
{
    public class UserMessageBindingModel
    {
        [Required]
        public string Text { get; set; }

        [Required]
        public string Recipient { get; set; }
    }
}
