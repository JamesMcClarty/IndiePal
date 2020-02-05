using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndiePal.Models.ViewModels
{
    public class NewProjectAndPositions: Project
    {
        public List<string> Positions { get; set; }
    }
}
