using AirBB.Models.DataLayer;
using AirBB.Models.DomainModels;
using AirBB.Models.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace AirBB.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly IRepository<Reservation> _reservationRepo;

        public ReservationsController(IRepository<Reservation> reservationRepo)
        {
            _reservationRepo = reservationRepo;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var sess = new AirBBSession(HttpContext.Session);
            var ids = sess.GetReservationIds();

            if (ids.Count == 0) return View(new List<Reservation>());

            var options = new QueryOptions<Reservation>
            {
                Includes = new[] { "Residence", "Residence.Location" },
                Where = r => ids.Contains(r.ReservationId),
                OrderBy = r => r.ReservationId,
                OrderByDirection = "desc"
            };

            var list = _reservationRepo.List(options).ToList();
            return View(list);
        }

        [HttpPost]
        public IActionResult Reserve(int id, string start, string end)
        {
            if (!DateTime.TryParse(start, out var sdt) || !DateTime.TryParse(end, out var edt) || sdt > edt)
            {
                TempData["message"] = "Please select a valid date range.";
                return RedirectToAction("Details", "Residence", new { id });
            }

            var options = new QueryOptions<Reservation>
            {
                Where = r => r.ResidenceId == id && r.ReservationStartDate <= edt && r.ReservationEndDate >= sdt
            };
            bool overlap = _reservationRepo.List(options).Any();

            if (overlap)
            {
                TempData["message"] = "Sorry, those dates are unavailable.";
                return RedirectToAction("Details", "Residence", new { id });
            }

            var res = new Reservation
            {
                ResidenceId = id,
                ReservationStartDate = sdt.Date,
                ReservationEndDate   = edt.Date
            };
            
            _reservationRepo.Insert(res);
            _reservationRepo.Save();

            var jar = new AirBBCookies(Request.Cookies, Response.Cookies);
            var ids = jar.GetReservationIds();
            ids.Add(res.ReservationId);
            jar.SaveReservationIds(ids);

            var sess = new AirBBSession(HttpContext.Session);
            sess.SetReservationIds(ids);

            TempData["message"] = "Reservation confirmed!";
            return RedirectToAction("Index", "Residence"); 
        }

        [HttpPost]
        public IActionResult Cancel(int id)
        {
            var r = _reservationRepo.Get(id);
            if (r != null)
            {
                _reservationRepo.Delete(r);
                _reservationRepo.Save();

                var jar = new AirBBCookies(Request.Cookies, Response.Cookies);
                var ids = jar.GetReservationIds();
                ids.Remove(id);
                jar.SaveReservationIds(ids);

                new AirBBSession(HttpContext.Session).SetReservationIds(ids);
                TempData["message"] = "Reservation canceled.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}