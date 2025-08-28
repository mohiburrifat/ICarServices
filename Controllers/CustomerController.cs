using I_Car_Services.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;

namespace I_Car_Services.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            
            return View();
        }

        [HttpPost]
        public IActionResult Register(Customer customer)
        {
            if (!ModelState.IsValid)
                return View(customer);

            customer.Role = "Customer";

            _context.Customers.Add(customer);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful. You can now login.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
[HttpPost]
public IActionResult Login(string email, string password)
{
    var user = _context.Customers
        .FirstOrDefault(u => u.Email == email && u.PasswordHash == password);

    if (user == null)
    {
        ViewBag.Message = "Invalid email or password.";
        return View();
    }

    // Check if the customer is approved (Customer_IsApproved)
    if (!user.Customer_IsApproved)
    {
        ViewBag.Message = "Your account has been banned. Please contact support.";
        return View();
    }

    // If the user is approved, log them in
    HttpContext.Session.SetString("UserRole", "Customer");
    HttpContext.Session.SetInt32("UserId", user.Id);

    return RedirectToAction("Dashboard");
}
public IActionResult Dashboard()
{
    var serviceList = _context.Services
        .Join(_context.MyServiceProviders,
              s => s.ServiceProviderId,
              sp => sp.Id,
              (s, sp) => new ServiceWithProviderViewModel
              {
                  Service = s,
                  ServiceProvider = sp
              })
        .ToList();

    return View(serviceList);
}

    [HttpGet]
        public IActionResult RequestService(int serviceId)
        {
            var service = _context.Services.FirstOrDefault(s => s.Id == serviceId);
            if (service == null)
            {
                return NotFound();
            }

            var serviceProvider = _context.MyServiceProviders.FirstOrDefault(sp => sp.Id == service.ServiceProviderId);
            if (serviceProvider == null)
            {
                return NotFound();
            }

            ViewBag.ServiceId = serviceId;
            ViewBag.ServiceProviderEmail = serviceProvider.Email;
            ViewBag.ServiceName = service.Name;

            return View();
        }

 [HttpPost]
public IActionResult RequestService(ServiceRequest request, int serviceId)
{
    var service = _context.Services.FirstOrDefault(s => s.Id == serviceId);
    if (service == null) return NotFound();

    var serviceProvider = _context.MyServiceProviders.FirstOrDefault(sp => sp.Id == service.ServiceProviderId);
    if (serviceProvider == null) return NotFound();

    int? customerId = HttpContext.Session.GetInt32("UserId");
    if (customerId == null) return RedirectToAction("Login");

    request.CustomerId = customerId.Value;
    request.ServiceProviderId = service.ServiceProviderId;
    request.RequestTime = DateTime.Now;
    request.Status = "Pending";

    _context.ServiceRequests.Add(request);
    _context.SaveChanges();

    if (request.PaidOnline) // If online payment selected
    {
        return RedirectToAction("Payment", "Payment", new
        {
            serviceRequestId = request.Id,
            serviceId = service.Id
        });
    }

    // For future: If cash allowed again
    TempData["SuccessMessage"] = "Request submitted.";
    return RedirectToAction("Dashboard");
}

    }
}
