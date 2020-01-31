using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IndiePal.Models
{
    public class Director
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
        [Required]
        public ApplicationUser ApplicationUser { get; set; }
        public virtual ICollection<Project> Projects { get; set; }
    }
}
