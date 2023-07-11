using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eventchain.Models
{
    public class Event
    {
        public int eventId { get; set; }  
 
        [Display(Name = "Event Name")]
        public string eventName { get; set; }
        [Display(Name = "Description")]
        public string eventDesc { get; set; }
        public string eventVenue { get; set; }
        [Display(Name = "Start Date")]
        public DateTime eventStart { get; set; }
        [Display(Name = "End Date")]
        public DateTime eventEnd { get; set; }
        [Display(Name = "Genre")]
        public string eventGenre { get; set; }
        [Display(Name = "Price €")]
        public int eventPrice { get; set; }
        public string photoName { get; set; }
        public string photoURL { get; set; }

        //Relationships
        public string userId { get; set; }
        public ApplicationUser user { get; set; }
        public List<Ticket> Tickets { get; set; }
    }
}
