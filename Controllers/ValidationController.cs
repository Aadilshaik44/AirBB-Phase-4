using AirBB.Models.DataLayer;
using AirBB.Models.DomainModels;
using Microsoft.AspNetCore.Mvc;

namespace AirBB.Controllers
{
    public class ValidationController : Controller
    {
        private readonly IRepository<Client> _clientRepo;

        public ValidationController(IRepository<Client> clientRepo)
        {
            _clientRepo = clientRepo;
        }

        public IActionResult CheckOwner(int ownerId)
        {
            var options = new QueryOptions<Client>
            {
                Where = c => c.UserId == ownerId && c.UserType == "Owner"
            };

            bool exists = _clientRepo.List(options).Any();
            
            if (!exists)
            {
                return Json($"Owner ID {ownerId} is invalid or not registered as an Owner.");
            }
            
            return Json(true);
        }
    }
}