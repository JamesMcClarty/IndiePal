using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IndiePal.Models.ViewModels
{
    public class NewTalentAndSkills: Talent
    {
        public string SkillString { get; set; }

    }
}
