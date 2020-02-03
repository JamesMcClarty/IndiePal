using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndiePal.Models.ViewModels
{
    public class UserHasDirectorViewModel
    {

        public bool hasDirectorAccout { get; set; }

        public ApplicationUser User { get; set; }
    }
}
