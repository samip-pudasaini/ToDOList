using Microsoft.AspNetCore.Mvc;
using ToDoList.Data;
using ToDoList.ViewModel;

namespace ToDoList.Controllers
{
    public class CalendarController : Controller
    {
        public readonly ToDoDbContext _db;

        public CalendarController(ToDoDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult GetEvents()
        {
            var events = _db.Tasks
                .Where(t => t.DueDate != null)
                .Select(t => new {
                    title = t.Title,
                    start = t.DueDate
                });
            return Json(events);
        }
    }
}
