using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AuditCore.Models;
using AuditCore.Context;

namespace AuditCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly AuditCoreContext _audit;

        public HomeController(AuditCoreContext audit)
        {
            _audit = audit;
        }

        public IActionResult Index()
        {
            List<Person> pl = new List<Person>() {
                new Person(){ Name = "Teste1", Number = "c", Date = DateTime.Now},
                new Person(){ Name = "Teste2", Number = "b", Date = DateTime.Now},
                new Person(){ Name = "Teste3", Number = "a", Date = DateTime.Now}
            };
            
            _audit.Person.AddRange(pl);
            _audit.SaveWithAudit();
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
