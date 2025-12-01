using Microsoft.AspNetCore.Mvc;
using AirBB.Models.DataLayer;
using AirBB.Models.DomainModels;

namespace AirBB.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LocationController : Controller
    {
        private readonly IRepository<Location> _locationRepo;

        public LocationController(IRepository<Location> locationRepo)
        {
            _locationRepo = locationRepo;
        }

        public IActionResult Index()
        {
            var options = new QueryOptions<Location> { OrderBy = l => l.Name };
            return View(_locationRepo.List(options));
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View(new Location());
        }

        [HttpPost]
        public IActionResult Add(Location location)
        {
            if (ModelState.IsValid)
            {
                _locationRepo.Insert(location);
                _locationRepo.Save();
                TempData["Message"] = "Location added successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var location = _locationRepo.Get(id);
            if (location == null) return NotFound();
            return View(location);
        }

        [HttpPost]
        public IActionResult Edit(Location location)
        {
            if (ModelState.IsValid)
            {
                _locationRepo.Update(location);
                _locationRepo.Save();
                TempData["Message"] = "Location updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(location);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var location = _locationRepo.Get(id);
            if (location == null) return NotFound();
            return View(location);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var location = _locationRepo.Get(id);
            if (location != null)
            {
                _locationRepo.Delete(location);
                _locationRepo.Save();
                TempData["Message"] = "Location deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}