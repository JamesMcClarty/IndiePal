using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IndiePal.Models
{
    public class ProjectPosition
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        [Required]
        public string Postion { get; set; }
        public int? TalentId { get; set; }
        public Talent Talent { get; set; }
    }
}
