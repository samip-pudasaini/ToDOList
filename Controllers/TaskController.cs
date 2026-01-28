using Microsoft.AspNetCore.Mvc;
using ToDoList.Data;
using ToDoList.Models;
using ToDoList.ViewModel;

namespace ToDoList.Controllers
{
    public class TaskController : Controller
    {
        public readonly ToDoDbContext _db;

        public TaskController(ToDoDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            var viewModel = new TaskListViewModel
            {
                Tasks = _db.Tasks.ToList()
            };
            return View(viewModel);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(String Title, int ListId)
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                // Input is invalid
                TempData["Error"] = "List name cannot be empty.";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                var obj = _db.Tasks.FirstOrDefault(t => t.Title == Title);

                if (obj != null)
                {
                    TempData["Error"] = "A list with this name already exists.";
                    return RedirectToAction("SelectedList", "List", new {id = ListId});
                }

                var TaskItem = new ToDoList.Models.Tasks
                {
                    Title = Title,
                    ListId = ListId,
                    Priority = "Low",
                    DueDate = DateTime.Now.AddDays(1)
                };

                _db.Tasks.Add(TaskItem);
                _db.SaveChanges();
            }

            return RedirectToAction("SelectedList", "List", new { id = ListId });
        }
    }
}
