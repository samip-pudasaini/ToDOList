using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ToDoList.Data;
using ToDoList.ViewModel;

namespace ToDoList.Controllers
{
    public class ListController : Controller
    {
        public readonly ToDoDbContext _db;

        public ListController(ToDoDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var firstList = _db.Lists.OrderBy(l => l.ListId).FirstOrDefault();

            var viewModel = new TaskListViewModel
            {
                List = _db.Lists.ToList(),
                SelectedList = firstList,  // Set the first list as selected
                Tasks = firstList != null
                    ? _db.Tasks.Where(t => t.ListId == firstList.ListId).ToList()
                    : new List<ToDoList.Models.Tasks>()
            };

            return View(viewModel);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(String Name)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                // Input is invalid
                TempData["Error"] = "List name cannot be empty.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                var obj = _db.Lists.FirstOrDefault(l => l.Name == Name);

                if (obj != null)
                {
                    TempData["Error"] = "A list with this name already exists.";
                    return RedirectToAction("Index");
                }

                var ListItem = new ToDoList.Models.List
                {
                    Name = Name
                };

                _db.Lists.Add(ListItem);
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult SelectedList(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var selectedList = _db.Lists.FirstOrDefault(l => l.ListId == id);
            var tasks = _db.Tasks.Where(t => t.ListId == id).ToList();

            var viewModel = new TaskListViewModel
            {
                List = _db.Lists.ToList(),
                SelectedList = selectedList,
                Tasks = tasks,
            };

            return View("Index", viewModel);
        }
    }
}
