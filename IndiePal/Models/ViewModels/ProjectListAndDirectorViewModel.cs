using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndiePal.Models.ViewModels
{
    public class ProjectListAndDirectorViewModel
    {
        public ICollection<Project> AllProjects { get; set; }

        public Director Director { get; set; }
    }
}
