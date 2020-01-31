using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IndiePal.Models
{
    public class TalentSkill
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int TalentID { get; set; }

        public Talent Talent { get; set; }

        [Required]
        public int SkillId { get; set; }

        public Skill Skill { get; set; }
    }
}
