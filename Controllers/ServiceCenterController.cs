using Microsoft.AspNetCore.Mvc;
using I_Car_Services.Models;

namespace I_Car_Services.Controllers
{
    public class ServiceCenterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceCenterController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(MyServiceProvider serviceProvider)
        {
            if (!ModelState.IsValid)
                return View(serviceProvider);

            serviceProvider.Role = "ServiceProvider";
            serviceProvider.IsApproved = true;

            _context.MyServiceProviders.Add(serviceProvider);
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
    var user = _context.MyServiceProviders
        .FirstOrDefault(u => u.Email == email && u.PasswordHash == password);

    if (user == null)
    {
        ViewBag.Message = "Invalid email or password.";
        return View();
    }

    if (!user.IsApproved)
    {
        ViewBag.Message = "Your account has been banned by the admin.";
        return View();
    }

  
    HttpContext.Session.SetString("UserRole", "ServiceCenter");
    HttpContext.Session.SetInt32("UserId", user.Id);

    return RedirectToAction("Dashboard");
}

        public IActionResult Dashboard()
        {
            return View();
        }

             public IActionResult Manage()
        {
            var providerId = HttpContext.Session.GetInt32("UserId"); // assuming you store provider ID
            var services = _context.Services.Where(s => s.ServiceProviderId == providerId).ToList();
            return View(services);
        }

        // GET: Service/Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: Service/Add
        [HttpPost]
        public IActionResult Add(Service service)
        {
            if (ModelState.IsValid)
            {
                service.ServiceProviderId = (int)HttpContext.Session.GetInt32("UserId");
                _context.Services.Add(service);
                _context.SaveChanges();
                return RedirectToAction("Manage");
            }
            return View(service);
        }

        // GET: Service/Edit/5
        public IActionResult Edit(int id)
        {
            var service = _context.Services.Find(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }

        // POST: Service/Edit/5
        [HttpPost]
        public IActionResult Edit(Service service)
        {
            if (ModelState.IsValid)
            {
                _context.Services.Update(service);
                _context.SaveChanges();
                return RedirectToAction("Manage");
            }
            return View(service);
        }

        // GET: Service/Delete/5
        public IActionResult Delete(int id)
        {
            var service = _context.Services.Find(id);
            if (service == null)
            {
                return NotFound();
            }
            return View(service);
        }

        // POST: Service/DeleteConfirmed/5
        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            var service = _context.Services.Find(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                _context.SaveChanges();
            }
            return RedirectToAction("Manage");
        }

        // Service Requests
        public IActionResult ServiceRequests()
        {
            // Get the service provider's ID from the session
            int serviceProviderId = (int)HttpContext.Session.GetInt32("UserId");

            
            var requests = _context.ServiceRequests
                .Where(r => r.ServiceProviderId == serviceProviderId)
                .ToList();

         
            return View(requests);
        }


        public IActionResult ViewServiceRequest(int id)
        {
            var serviceRequest = _context.ServiceRequests
                                         .FirstOrDefault(sr => sr.Id == id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

    
            var customer = _context.Customers.FirstOrDefault(c => c.Id == serviceRequest.CustomerId);
            var serviceProvider = _context.MyServiceProviders
                                          .FirstOrDefault(sp => sp.Id == serviceRequest.ServiceProviderId);

            ViewBag.CustomerName = customer?.Name;
            ViewBag.ServiceProviderName = serviceProvider?.BusinessName;

            return View(serviceRequest);
        }


        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var serviceRequest = _context.ServiceRequests
                                         .FirstOrDefault(sr => sr.Id == id);

            if (serviceRequest == null)
            {
                return NotFound();
            }

            // Update the status of the service request
            serviceRequest.Status = status;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Service request status updated successfully.";

            return RedirectToAction("ViewServiceRequest", new { id = id });
        }

                public IActionResult Balance()
        {
            
            var userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "ServiceCenter"); 
            }

     
            var serviceRequests = _context.ServiceRequests.Where(sr => sr.ServiceProviderId == userId).ToList();

            var payments = _context.Payments.Where(p => serviceRequests.Select(sr => sr.Id).Contains(p.ServiceRequestId)).ToList();

            
            decimal totalPayments = payments.Sum(p => p.Amount);

            decimal adminCommission = totalPayments * 0.05M;

            
            decimal balance = totalPayments - adminCommission;

            
            ViewBag.TotalPayments = totalPayments;
            ViewBag.AdminCommission = adminCommission;
            ViewBag.Balance = balance;
            ViewBag.Payments = payments; 
            return View();
        }
    }
}
