using AirBB.Models.DataLayer;
using AirBB.Models.DomainModels;
using AirBB.Models.ViewModels;
using AirBB.Models.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AirBB.Controllers
{
    public class ResidenceController : Controller
    {
        private readonly IRepository<Residence> _residenceRepo;
        private readonly IRepository<Location> _locationRepo;
        private readonly IRepository<Reservation> _reservationRepo;

        public ResidenceController(IRepository<Residence> residenceRepo, 
                                   IRepository<Location> locationRepo,
                                   IRepository<Reservation> reservationRepo)
        {
            _residenceRepo = residenceRepo;
            _locationRepo = locationRepo;
            _reservationRepo = reservationRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var sess = new AirBBSession(HttpContext.Session);

            if (sess.GetReservationIds().Count == 0)
            {
                var jar = new AirBBCookies(Request.Cookies, Response.Cookies);
                sess.SetReservationIds(jar.GetReservationIds());
            }

            var model = new HomeViewModel
            {
                ActiveLocationId = sess.GetLoc(),
                ActiveGuests     = sess.GetGuests(),
                ActiveStart      = sess.GetStart(),
                ActiveEnd        = sess.GetEnd(),
                Locations        = BuildLocationList()
            };

            model.Residences = ApplyFilter(model);
            model.Reservations = GetReservationsForBadge(sess.GetReservationIds());
            return View("Index", model);
        }

        [HttpPost]
        public IActionResult Filter(HomeViewModel model)
        {
            var sess = new AirBBSession(HttpContext.Session);
            sess.SetFilters(model.ActiveLocationId, model.ActiveGuests, model.ActiveStart, model.ActiveEnd);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var options = new QueryOptions<Residence>
            {
                Where = r => r.ResidenceId == id,
                Includes = new[] { "Location" }
            };

            var res = _residenceRepo.Get(options);
            if (res == null) return RedirectToAction(nameof(Index));

            var sess = new AirBBSession(HttpContext.Session);
            ViewData["loc"] = sess.GetLoc();
            ViewData["gu"]  = sess.GetGuests();
            ViewData["st"]  = sess.GetStart();
            ViewData["en"]  = sess.GetEnd();

            return View(res);
        }

        private List<SelectListItem> BuildLocationList()
        {
            var options = new QueryOptions<Location> { OrderBy = l => l.Name };
            var locations = _locationRepo.List(options);

            var items = locations.Select(l => new SelectListItem 
            { 
                Text = l.Name, 
                Value = l.LocationId.ToString() 
            }).ToList();

            items.Insert(0, new SelectListItem { Text = "All", Value = "All" });
            return items;
        }

        private List<Residence> ApplyFilter(HomeViewModel m)
        {
            // 1. Identify Blocked Residences due to Reservations
            List<int> blockedIds = new List<int>();
            if (DateTime.TryParse(m.ActiveStart, out var s) && DateTime.TryParse(m.ActiveEnd, out var e))
            {
                var resOptions = new QueryOptions<Reservation>
                {
                    Where = rv => rv.ReservationStartDate <= e && rv.ReservationEndDate >= s
                };
                blockedIds = _reservationRepo.List(resOptions)
                                             .Select(rv => rv.ResidenceId)
                                             .Distinct()
                                             .ToList();
            }

            // 2. Parse Other Filters
            bool hasLocation = int.TryParse(m.ActiveLocationId, out var locId);
            bool hasGuests = int.TryParse(m.ActiveGuests, out var guests) && guests > 1;

            // 3. Build Residence QueryOptions
            var options = new QueryOptions<Residence>
            {
                Includes = new[] { "Location" },
                OrderBy = r => r.Location!.Name 
            };

            // 4. Construct Predicate
            options.Where = r => 
                (!hasLocation || r.LocationId == locId) &&
                (!hasGuests || r.GuestNumber >= guests) &&
                (!blockedIds.Contains(r.ResidenceId));

            // 5. Fetch and Apply Secondary Sort (Name) in memory
            return _residenceRepo.List(options)
                                 .OrderBy(r => r.Location?.Name)
                                 .ThenBy(r => r.Name)
                                 .ToList();
        }

        private List<Reservation> GetReservationsForBadge(List<int> ids)
        {
            if (ids.Count == 0) return new List<Reservation>();

            var options = new QueryOptions<Reservation>
            {
                Includes = new[] { "Residence" },
                Where = r => ids.Contains(r.ReservationId)
            };
            return _reservationRepo.List(options).ToList();
        }
    }
}