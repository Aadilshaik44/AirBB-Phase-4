using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AirBB.Models.DataLayer;
using AirBB.Models.DomainModels;

namespace AirBB.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ResidenceController : Controller
    {
        private readonly IRepository<Residence> _residenceRepo;
        private readonly IRepository<Location> _locationRepo;
        private readonly IRepository<Client> _clientRepo;

        public ResidenceController(IRepository<Residence> residenceRepo, 
                                   IRepository<Location> locationRepo,
                                   IRepository<Client> clientRepo)
        {
            _residenceRepo = residenceRepo;
            _locationRepo = locationRepo;
            _clientRepo = clientRepo;
        }

        public IActionResult Index()
        {
            var options = new QueryOptions<Residence>
            {
                Includes = new[] { "Location", "Owner" },
                OrderBy = r => r.Name
            };
            return View(_residenceRepo.List(options));
        }
        
        private void LoadLocations(int? selectedId = null)
        {
            var options = new QueryOptions<Location> { OrderBy = l => l.Name };
            ViewBag.Locations = new SelectList(_locationRepo.List(options), "LocationId", "Name", selectedId);
        }

        private void LoadOwners(int? selectedId = null)
        {
            var options = new QueryOptions<Client>
            {
                Where = c => c.UserType == "Owner",
                OrderBy = c => c.Name
            };
            ViewBag.Owners = new SelectList(_clientRepo.List(options), "UserId", "Name", selectedId); 
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Action = "Add";
            LoadLocations();
            LoadOwners();
            ModelState.Clear(); 
            return View(new Residence());
        }

        [HttpPost]
        public IActionResult Add(Residence residence)
        {
            if (ModelState.IsValid)
            {
                _residenceRepo.Insert(residence);
                _residenceRepo.Save();
                TempData["Message"] = "Residence added successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Action = "Add";
            LoadLocations(residence.LocationId);
            LoadOwners(residence.OwnerId);
            return View(residence);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var residence = _residenceRepo.Get(id);
            if (residence == null) return NotFound();

            ViewBag.Action = "Edit";
            LoadLocations(residence.LocationId);
            LoadOwners(residence.OwnerId);
            return View(residence);
        }

        [HttpPost]
        public IActionResult Edit(Residence residence)
        {
            if (ModelState.IsValid)
            {
                _residenceRepo.Update(residence);
                _residenceRepo.Save();
                TempData["Message"] = "Residence updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.Action = "Edit";
            LoadLocations(residence.LocationId);
            LoadOwners(residence.OwnerId);
            return View(residence);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var options = new QueryOptions<Residence>
            {
                Where = r => r.ResidenceId == id,
                Includes = new[] { "Location", "Owner" }
            };
            var residence = _residenceRepo.Get(options);
            if (residence == null) return NotFound();
            return View(residence);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var residence = _residenceRepo.Get(id);
            if (residence != null)
            {
                _residenceRepo.Delete(residence);
                _residenceRepo.Save();
                TempData["Message"] = "Residence deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}