using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eventchain.Data;
using eventchain.Models;
using eventchain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;
using System.Security.Claims;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text;

namespace eventchain.Controllers
{
    [Authorize]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        //private readonly IConfiguration _config;
        /*
        public EventsController(IConfiguration config)
        {
            _config = config;
        }*/


        public EventsController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;

        }
        [AllowAnonymous]
        public async Task<IActionResult> Index_public()
        {
            return View(await _context.Events.ToListAsync());
        }
        [AllowAnonymous]
        public async Task<IActionResult> Details_public(int? id, string astr)
        {
            if (id == null)
            {
                return NotFound();
            }

            var secret = await _context.Events.FirstOrDefaultAsync(m => m.eventId == id);
            if (secret == null)
            {
                return NotFound();
            }
            ViewBag.TicketNotAvailable = astr;
            return View(secret);
        }
        
        public IActionResult Buy_Success(int id)
        {
            int quantity = 1;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            string purchaserId = userId.ToString();
            Boolean complete = Buy(_context, quantity, purchaserId, id);
            if (complete == true)
            {
                return View();
            }
            else
            {
                string availability = "Sorry! Ticket You have chosen isn't currently available. Try Again Soon!";
                return RedirectToAction("Details_public", new { id = id, astr = availability });
            }
            //return View();
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Events.Include(u => u.user);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(u => u.user)
                .FirstOrDefaultAsync(m => m.eventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("eventId,eventName,eventDesc,eventVenue,eventStart,eventEnd,eventGenre,eventPrice,photoName,photoURL,userId")] Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id", @event.userId);
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id", @event.userId);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("eventId,eventName,eventDesc,eventVenue,eventStart,eventEnd,eventGenre,eventPrice,photoName,photoURL,userId")] Event @event)
        {
            if (id != @event.eventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.eventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id", @event.userId);
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(u => u.user)
                .FirstOrDefaultAsync(m => m.eventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.eventId == id);
        }

        


        static bool Buy(ApplicationDbContext context, int quantity, string purchaserId, int id)
        {
            string connectionString = "Data Source=sqlserver;User ID=SA;Password=OcP2020123;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";

            //Search for available tickets

            List<string> lstAvailableTickets = new List<string>();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        command.CommandText = "select * from [dbo].[Tickets] where [eventId] = "+id+" AND [ticketNotAvailable] = 0";

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows) {
                                while (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        //reader.GetOrdinal("[ticketId]")
                                        string citem = reader.GetString(0);
                                        // -- Adding single string to list 
                                        lstAvailableTickets.Add(citem);
                                    }
                                    reader.NextResult();

                                }
                            }
                            else
                            {
                                return false;
                            }
                            
                        }
                        connection.Close();
                    }

                }
            }
            catch (Exception exc) {
                Console.WriteLine(exc);
                return false; 
            }

            //Check there are enough for order
            if (quantity > lstAvailableTickets.Count){
                return false;
            }
            int i = 0;
            //Update Ownership
            while (i < quantity) {
                string queryString = "UPDATE [dbo].[Tickets] SET[ticketNotAvailable] = 1, [userId]= N'" + purchaserId + "' WHERE[ticketId] = '" + lstAvailableTickets[i] + "'; ";
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(queryString, connection);
                        command.Connection.Open();
                        command.ExecuteNonQuery();
                        command.Connection.Close();
                    }
                    
                }
                catch (Exception exc)
                {
                    return false;
                }


                //Call API
                try
                {
                    HttpClient endpoint = new HttpClient();
                    // Update port # in the following line.
                    endpoint.BaseAddress = new Uri("http://node_servicer:8080");
                    endpoint.DefaultRequestHeaders.Accept.Clear();
                    endpoint.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    // Get the product
                    endpoint.GetAsync("/mint/" + lstAvailableTickets[i]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                /*try
                {
                    // Update port # in the following line.
                    client.BaseAddress = new Uri("http://node_servicer:8080");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    // Get the product
                    
                    var url = MintWithGet("/mint/" + lstAvailableTickets[i]);
               }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                } */
                i++;
            }
            /*
        try{
                // Update port # in the following line.
                client.BaseAddress = new Uri("http://node_service");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));


                // Create a new ticket on the blockchain
                string address = null;
                string pk = null;
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandType = System.Data.CommandType.Text;
                            command.CommandText = "select * from [dbo].[AspNetUsers] where [Id] = " + id;

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                if (reader.HasRows)
                                {
                                    while (reader.HasRows)
                                    {
                                        address = reader.GetString(4);
                                        pk = reader.GetString(3);
                                        // reader.GetString(0);
                                    }
                                }
                                else
                                {
                                    return false;
                                }

                            }
                            connection.Close();
                        }

                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc);
                    return false;
                }





                tickettrans deatails = new tickettrans
                {
                    id = lstAvailableTickets[i-1],
                    address = address,
                    privateKey = pk
                    };

                    var url = MintTicket(deatails);
                    Console.WriteLine("Sent to API: " + url);

                }
            catch (Exception exc)
        {
                Console.WriteLine("Didnt send to API: " + exc);
            }
            */




            return true;
        }

        public class tickettrans
        {

            public string id { get; set; }
            public string address { get; set; }
            public string privateKey { get; set; }
        }

        static HttpClient client = new HttpClient();

        static async Task<Uri> MintWithGet(string path)
        {
            //Product product = null;
            Console.WriteLine("Calling api link: " + path);
            HttpResponseMessage response = await client.GetAsync(path);
            
            return response.Headers.Location;
        }

        static async Task<Uri> MintTicket(tickettrans details)
        {

            

            StringContent Content = new StringContent(JsonSerializer.Serialize(details), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync("/buyticket/", Content);

            Console.WriteLine("Text Sent: " + JsonSerializer.Serialize(details));
            Console.WriteLine("Response: " + response);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

    }


}
