using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndiePal.Models.ViewModels
{
    public class ListOfPositionsAndProjects : Message
    {
        public Dictionary<string, List<ProjectPosition>> SeperatePositions { get; set; }
        public List<SelectListItem> SelectProjects { get; set; }
    }
}
