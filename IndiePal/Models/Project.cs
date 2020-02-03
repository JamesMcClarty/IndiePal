﻿using System;
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
        public int Title { get; set; }
        [Required]
        public bool Active { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Required]
        public decimal Budget { get; set; }
        [Required]
        public string Description { get; set; }
        public virtual ICollection<ProjectLog> ProjectLogs { get; set; }
    }
}