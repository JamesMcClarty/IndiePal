using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IndiePal.Models
{
    public class Talent
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [Required]
        public string Biography { get; set; }
        [Required]
        public decimal Wage { get; set; }
        public virtual List<TalentSkill> Skills { get; set; }
    }
}
