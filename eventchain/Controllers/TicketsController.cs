using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using eventchain.Data;
using eventchain.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Dynamic;
using System.Net.Mail;



namespace eventchain.Controllers
{
    
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }
        

        // GET: Tickets
        public async Task<IActionResult> Index(string id)
        {
            
            dynamic mydynamicmodel = new ExpandoObject();
            if (id != null)
            {
                ChangeAvailability(id);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var applicationDbContext = _context.Tickets.Where(s => s.userId == userId).Include(t => t.eventID).Include(t => t.user);
            mydynamicmodel.Tickets = await applicationDbContext.ToListAsync();
            var applicationDbContext2 = _context.Events;
            mydynamicmodel.Events = await applicationDbContext2.ToListAsync();
            return View(mydynamicmodel);

        }
        

        private IActionResult View(Func<Task<IActionResult>> index, List<Ticket> lists)
        {
            throw new NotImplementedException();
        }


        // GET: Tickets
        public async Task<IActionResult> Index_Admin()
        {            
            var applicationDbContext = _context.Tickets.Include(t => t.eventID).Include(t => t.user);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.eventID)
                .Include(t => t.user)
                .FirstOrDefaultAsync(m => m.ticketId == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["eventId"] = new SelectList(_context.Events, "eventId", "eventId");
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ticketId,ticketNotAvailable,ticketRedeemed,eventId,userId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["eventId"] = new SelectList(_context.Events, "eventId", "eventId", ticket.eventId);
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id", ticket.userId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["eventId"] = new SelectList(_context.Events, "eventId", "eventId", ticket.eventId);
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id", ticket.userId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ticketId,ticketNotAvailable,ticketRedeemed,eventId,userId")] Ticket ticket)
        {
            if (id != ticket.ticketId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.ticketId))
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
            ViewData["eventId"] = new SelectList(_context.Events, "eventId", "eventId", ticket.eventId);
            ViewData["userId"] = new SelectList(_context.Users, "Id", "Id", ticket.userId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.eventID)
                .Include(t => t.user)
                .FirstOrDefaultAsync(m => m.ticketId == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Redeemed(string id)
        {
            var user = User.FindFirstValue(ClaimTypes.Email);
            string recipientemail = user.ToString();
            ChangeRedemtionStatus(id, recipientemail);
            return View();
        }

        private bool TicketExists(string id)
        {
            return _context.Tickets.Any(e => e.ticketId == id);
        }

        static bool ChangeAvailability(string ticketID)
        {
            string connectionString = "Data Source=sqlserver;User ID=SA;Password=OcP2020123;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";
            Boolean status = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = System.Data.CommandType.Text;
                        command.CommandText = "select * from [dbo].[Tickets] where [ticketID] = '"+ ticketID + "';";

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        //reader.GetOrdinal("[ticketId]")
                                        status = (bool)reader["ticketNotAvailable"];}
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
            catch (Exception exc)
            {
                return false;
            }

            int updatedsatus = 1;
            if (status == false)
            {
                updatedsatus = 1;
            }
            else
            {
                updatedsatus = 0;
            }

            //Update Ticket Status
            string queryString = "UPDATE [dbo].[Tickets] SET[ticketNotAvailable] = " + updatedsatus + " WHERE[ticketId] = '" + ticketID + "'; ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
                command.Connection.Close();
            }
            return true;
        }

        static bool ChangeRedemtionStatus(string ticketID, string email)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587);

            smtpClient.Credentials = new System.Net.NetworkCredential("eventchainire@gmail.com", "Event_chain1!");
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            
            smtpClient.EnableSsl = true;

            MailMessage mailMessage = new MailMessage("eventchainire@gmail.com",email);
            mailMessage.Subject = "Eventchain Ticket ID:" + ticketID;
            mailMessage.Body = "<html>" +
                "<body>" +
                    "</h1>This is your Ticket for " + ticketID + ". </h1>" +
                    "</br>" +
                    "</h2>Please be prepare to produce this at admission.</h2>" +
                    "</br>" +
                    "<img alt='Barcode Generator TEC-IT' src = 'https://barcode.tec-it.com/barcode.ashx?data=" + ticketID + "&code=Code128&translate-esc=on' /> " +
                    "</body></html>";
            mailMessage.IsBodyHtml = true;

            try
            {
                smtpClient.Send(mailMessage);
                Console.WriteLine("email was sent successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed to send email with the following error:");
                Console.WriteLine(ex.Message);
            }



            string connectionString = "Data Source=sqlserver;User ID=SA;Password=OcP2020123;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;";
            int newstatus = 1;

            //Update Ticket Status
            string queryString = "UPDATE [dbo].[Tickets] SET[ticketRedeemed] = " + newstatus + " WHERE[ticketId] = '" + ticketID + "'; ";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
                command.Connection.Close();
            }
            return true;
        }
        

    }

    

}

