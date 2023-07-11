using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eventchain.Models
{
    public class Ticket
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string ticketId { get; set; }
        public Boolean ticketNotAvailable { get; set; }
        public Boolean ticketRedeemed { get; set; }

        //Relationships
        public int eventId { get; set; }
        public Event eventID { get; set; }
        public string? userId { get; set; }
        public ApplicationUser user { get; set; }
    }
}
