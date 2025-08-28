using I_Car_Services.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace I_Car_Services.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Admin/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var admin = _context.Users.FirstOrDefault(u => u.Email == email && u.Role == "Admin");

            if (admin != null && admin.PasswordHash == password)
            {
                HttpContext.Session.SetString("UserRole", "Admin");
                HttpContext.Session.SetInt32("UserId", admin.Id);
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid credentials or not an admin.";
            return View();
        }

        // GET: Admin/Dashboard
        [HttpGet]
        public IActionResult Dashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login");

            return View();
        }

        // GET: Admin/Balance
        [HttpGet]
        public IActionResult Balance()
        {
            // Fetch all necessary raw data
            var payments = _context.Payments.ToList();
            var serviceRequests = _context.ServiceRequests.ToList();
            var providers = _context.MyServiceProviders.ToList();
            var balances = _context.Balances.ToList();
            var users = _context.Users.ToList(); // to find Admin user

            // Assuming only one Admin exists
            var admin = users.FirstOrDefault(u => u.Role == "Admin");
            decimal adminTotal = 0;

            // Calculate total payments and commission for each provider
            var providerBalances = (from p in payments
                                    join sr in serviceRequests on p.ServiceRequestId equals sr.Id
                                    join sp in providers on sr.ServiceProviderId equals sp.Id
                                    group p by new { sp.Id, sp.BusinessName } into g
                                    select new
                                    {
                                        BusinessName = g.Key.BusinessName,
                                        TotalPayments = g.Sum(x => x.Amount),
                                        Commission = g.Sum(x => x.Amount) * 0.05m
                                    }).ToList<object>();

            // Calculate admin total balance
            if (admin != null)
            {
                var adminBalance = balances.FirstOrDefault(b => b.UserId == admin.Id);
                adminTotal = adminBalance != null ? adminBalance.Amount : 0;
            }

            ViewData["ProviderInfoWithBalance"] = providerBalances;
            ViewData["AdminBalance"] = adminTotal;

            return View();
        }

        // GET: Admin/ServiceProviders
        public IActionResult ServiceProviders()
        {
            var providers = _context.MyServiceProviders.ToList();
            return View(providers);
        }

        // POST: Admin/ToggleApproval/5
        [HttpPost]
        public IActionResult ToggleApproval(int id)
        {
            var provider = _context.MyServiceProviders.FirstOrDefault(p => p.Id == id);
            if (provider != null)
            {
                provider.IsApproved = !provider.IsApproved;
                _context.SaveChanges();

                // Send an email if the provider is banned (approval toggled off)
                if (!provider.IsApproved)
                {
                    try
                    {
                        var smtpHost = "mail.hoichoy.com";
                        var smtpPort = 587;
                        var smtpEmail = "icarservice@hoichoy.com";
                        var smtpPassword = "nLnqngfCG33UKNk";

                        MailMessage message = new MailMessage();
                        message.From = new MailAddress(smtpEmail);
                        message.To.Add(provider.Email);
                        message.Subject = "Account Banned - iCar Service Platform";
                        message.Body = $@"
Dear {provider.BusinessName},

Your account on iCar Service Platform has been *banned* by the admin.

If you believe this was a mistake or have any inquiries, please contact us at admin@icar.com.

Thank you,
iCar Admin Team
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
            }

            return RedirectToAction("ServiceProviders");
        }

            [HttpGet]
        public IActionResult Customers()
        {
            // Get all customers with their approval status
            var customers = _context.Users.OfType<Customer>().ToList();
            return View(customers);
        }

        // POST: Admin/ToggleCustomerApproval/5
        [HttpPost]
        public IActionResult ToggleCustomerApproval(int id)
        {
            var customer = _context.Users.OfType<Customer>().FirstOrDefault(c => c.Id == id);
            if (customer != null)
            {
                // Toggle the approval status of the customer
                customer.Customer_IsApproved = !customer.Customer_IsApproved;
                _context.SaveChanges();
            }

            return RedirectToAction("Customers");
        }
    }
}
