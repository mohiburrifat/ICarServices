using I_Car_Services.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

namespace I_Car_Services.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Load the payment page
        [HttpGet]
        public IActionResult Payment(int serviceRequestId, int serviceId)
        {
            var service = _context.Services.FirstOrDefault(s => s.Id == serviceId);
            var request = _context.ServiceRequests.FirstOrDefault(r => r.Id == serviceRequestId);

            if (service == null || request == null)
                return NotFound();

            ViewBag.ServiceName = service.Name;
            ViewBag.Price = service.Price;
            ViewBag.ServiceId = service.Id;
            ViewBag.ServiceRequestId = request.Id;

            return View();
        }

        // POST: Process the payment
        [HttpPost]
        public IActionResult Payment(int serviceRequestId, int serviceId, string paymentMethod)
        {
            var service = _context.Services.FirstOrDefault(s => s.Id == serviceId);
            var request = _context.ServiceRequests.FirstOrDefault(r => r.Id == serviceRequestId);

            if (service == null || request == null)
                return NotFound();

            // Save payment
            var payment = new Payment
            {
                ServiceRequestId = serviceRequestId,
                Amount = service.Price,
                PaidAt = DateTime.Now,
                PaymentMethod = paymentMethod
            };

            _context.Payments.Add(payment);

            // Update request status
            request.Status = "Confirmed";

            // Balance distribution
            var serviceProvider = _context.MyServiceProviders.FirstOrDefault(p => p.Id == request.ServiceProviderId);
            var admin = _context.Users.FirstOrDefault(u => u.Role == "Admin");

            if (serviceProvider != null && admin != null)
            {
                var total = service.Price;
                var providerShare = total * 0.95m;
                var adminShare = total * 0.05m;

                var providerBalance = _context.Balances.FirstOrDefault(b => b.UserId == serviceProvider.Id);
                if (providerBalance == null)
                {
                    providerBalance = new Balance { UserId = serviceProvider.Id, Amount = 0 };
                    _context.Balances.Add(providerBalance);
                }
                providerBalance.Amount += providerShare;

                var adminBalance = _context.Balances.FirstOrDefault(b => b.UserId == admin.Id);
                if (adminBalance == null)
                {
                    adminBalance = new Balance { UserId = admin.Id, Amount = 0 };
                    _context.Balances.Add(adminBalance);
                }
                adminBalance.Amount += adminShare;
            }

            _context.SaveChanges();

            // Send email
            var customer = _context.Customers.FirstOrDefault(c => c.Id == request.CustomerId);
            if (serviceProvider != null)
            {
                try
                {
                    var smtpHost = "mail.hoichoy.com";
                    var smtpPort = 587;
                    var smtpEmail = "icarservice@hoichoy.com";
                    var smtpPassword = "nLnqngfCG33UKNk";

                    MailMessage message = new MailMessage();
                    message.From = new MailAddress(smtpEmail);
                    message.To.Add(serviceProvider.Email);
                    message.Subject = "New Service Request";
                    message.Body = $@"
You have a new service request!

Customer: {customer?.Name}
Service Type: {request.ServiceType}
Address: {request.Address}

View the request here:
http://localhost:5245/ServiceCenter/ViewServiceRequest/{request.Id}
";

                    using (SmtpClient client = new SmtpClient(smtpHost, smtpPort))
                    {
                        client.Credentials = new NetworkCredential(smtpEmail, smtpPassword);
                        client.EnableSsl = true;
                        client.Send(message);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Email error: " + ex.Message);
                }
            }

            TempData["SuccessMessage"] = "Payment completed and request sent.";
            return RedirectToAction("Dashboard", "Customer");
        }
    }
}
