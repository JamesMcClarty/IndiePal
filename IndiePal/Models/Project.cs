using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IndiePal.Models
{
    public class Project
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        [Required]
        public decimal Budget { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int DirectorId {get; set;}
        public virtual Director Director { get; set; }
        public virtual ICollection<ProjectLog> ProjectLogs { get; set; }
        public virtual ICollection<ProjectPosition> CurrentPositions { get; set; }
    }
}
