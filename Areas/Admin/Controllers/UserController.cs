using Microsoft.AspNetCore.Mvc;
using AirBB.Models.DataLayer;
using AirBB.Models.DomainModels;

namespace AirBB.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly IRepository<Client> _clientRepo;
        public UserController(IRepository<Client> clientRepo) => _clientRepo = clientRepo;

        public IActionResult Index()
        {
            var options = new QueryOptions<Client> { OrderBy = c => c.Name };
            return View(_clientRepo.List(options));
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new Client());
        }

        [HttpPost]
        public IActionResult Add(Client client)
        {
            if (ModelState.IsValid)
            {
                _clientRepo.Insert(client);
                _clientRepo.Save();
                TempData["Message"] = "User added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var client = _clientRepo.Get(id);
            if (client == null) return NotFound();
            return View(client);
        }

        [HttpPost]
        public IActionResult Edit(Client client)
        {
            if (ModelState.IsValid)
            {
                _clientRepo.Update(client);
                _clientRepo.Save();
                TempData["Message"] = "User updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(client);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var client = _clientRepo.Get(id);
            if (client == null) return NotFound();
            return View(client);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var client = _clientRepo.Get(id);
            if (client != null)
            {
                _clientRepo.Delete(client);
                _clientRepo.Save();
                TempData["Message"] = "User deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}