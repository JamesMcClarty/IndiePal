using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IndiePal.Models.ViewModels
{
    public class TalentListAndTalentViewModel
    {
        public ICollection<Talent> AllTalents { get; set; }

        public Talent Talent { get; set; }
    }
}
