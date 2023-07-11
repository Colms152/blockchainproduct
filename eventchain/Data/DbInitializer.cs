using eventchain.Data;
using eventchain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eventchain.Controllers
{
    public class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Events.Any())
            {
                return;   // DB has been seeded
            }

            var @events = new Event[]
                {
            new Event{photoName = "GAA", eventName="Munster Football Championship - Season Ticket",eventDesc = "This ticket will give you admission to all munster football championship matches in the 2021 season."},
            new Event{photoName = "Bicep", eventName="Bicep Live",eventDesc = "The Belfast Duo will preform they're latest Album, Isles, live from District 8, Dublin - 20 May 2021"},
            new Event{photoName = "Dave", eventName="Dave UK & Ireland Tour",eventDesc = "Dave will be preforming his latest singles along with his most popular songs from his hit albums. 3 Area - 31 August 2021"},
            new Event{photoName = "20032021", eventName="Andrea Bocelli",eventDesc = "20 March in Cork Oprea House Andrea Bocelli Will Preform his Greatest Songs"},
            new Event{photoName = "13062021",  eventName="Dua Lipa",eventDesc = "13 June in Croke Park Dua Lipa Will Preform Her New Album"},
            new Event{photoName = "10072021", eventName="Ed Sheeran",eventDesc = "10 July in Pairc Uí Caomhe Ed Sheeran Will Preform his New Album"},
            new Event{photoName = "01092021", eventName="Electric Picinic",eventDesc = "1 September Electric Picnic will take place for 3 day with many acts"},
            new Event{photoName = "20102021", eventName="Guinness Jazz Festival",eventDesc = "20 October a Number of Jazz preformance will take place across Cork City"}

            };
            /*@events.Append(
                new Event { photoName = "20102021", eventName = "Guinness Jazz Festival", eventDesc = "20 October a Number of Jazz preformance will take place across Cork City" }

            ); */
            
            foreach (Event s in @events)
            {
                context.Events.Add(s);
            }
            context.SaveChanges();


            int i = 2;
            List<Ticket> tickets = new List<Ticket>{
                    new Ticket{eventId = 1, ticketId= "SeasonTicket1"},
                    new Ticket{eventId = 2, ticketId= "Bicep1"},
                    new Ticket{eventId = 3, ticketId= "Dave1"},
                    new Ticket{eventId = 4, ticketId= "Andrea Bocelli1"},
                    new Ticket{eventId = 5, ticketId= "Dua Lipa1"},
                    new Ticket{eventId = 6, ticketId= "Ed Sheeran1"},
                    new Ticket{eventId = 7, ticketId= "Electric Picinic1"},
                    new Ticket{eventId = 8, ticketId= "Guinness Jazz Festival1"}
             }; 
            while (i != 100) {
                /*tickets.Add(
                new Ticket { eventId = 1, ticketId = "Andrea Bocelli" + i }
                );*/
                tickets.Add(
                new Ticket { eventId = 1, ticketId = "SeasonTicket" + i }
                );
                tickets.Add(
                new Ticket { eventId = 2, ticketId = "Bicep" + i }
                );
                tickets.Add(
                new Ticket { eventId = 3, ticketId = "Dave" + i }
                );
                tickets.Add(
                new Ticket { eventId = 5, ticketId = "Dua Lipa" + i }
                );
                tickets.Add(
                new Ticket { eventId = 6, ticketId = "Ed Sheeran" + i }
                );
                tickets.Add(
                new Ticket { eventId = 7, ticketId = "Electric Picinic" + i }
                );
                tickets.Add(
                new Ticket { eventId = 8, ticketId = "Guinness Jazz Festival" + i }
                );
                i++;
            }
            


            foreach (Ticket t in tickets)
                {
                    context.Tickets.Add(t);
                }
            context.SaveChanges();





        }
    
}
}
