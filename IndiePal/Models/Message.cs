using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IndiePal.Models
{
    public class Message
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Header { get; set; }
        public string Body { get; set; }
        [Required]
        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }
        [Required]
        public string RecieverId { get; set; }
        public ApplicationUser Reciever { get; set; }
        public int? ProjectId { get; set; }
        public Project Project { get; set; }
        public int? ProjectPositionId { get; set; }
        public ProjectPosition ProjectPosition { get; set; }
    }
}
