using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IndiePal.Models
{
    public class ProjectLog
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Log { get; set; }
        [Required]
        public DateTime DateMade { get; set; }
        [Required]
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
