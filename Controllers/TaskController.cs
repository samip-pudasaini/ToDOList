using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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


        //Handles quick create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult QuickCreate(String Title, int ListId)
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                TempData["Error"] = "Task name cannot be empty.";
                return RedirectToAction("SelectedList", "List", new { id = ListId });
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
            TempData["success"] = "Task created successfully.";

            return RedirectToAction("SelectedList", "List", new { id = ListId });
        }


        //Handles creation from the calendar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Tasks obj, string returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(obj.Title))
            {
                TempData["Error"] = "Task name cannot be empty.";
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("SelectedList", "List", new { id = obj.ListId });
            }

            // Make sure ListId is valid
            if (obj.ListId == 0 || !_db.Lists.Any(l => l.ListId == obj.ListId))
            {
                TempData["Error"] = "Please select a valid list.";
                if (!string.IsNullOrEmpty(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Index", "List");
            }

            obj.TaskId = 0; // Force EF to treat this as a new entity
            _db.Tasks.Add(obj);
            _db.SaveChanges();
            TempData["success"] = "Task created successfully.";

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("SelectedList", "List", new { id = obj.ListId });
        }

        public IActionResult GetTaskDetails(int id, string returnUrl = null, string date = null)
        {
            try
            {
                Tasks task;
                if (id == 0) //add directly from the calendar
                {
                    //New task with pre-filled date
                    task = new Tasks
                    {
                        TaskId = 0,
                        DueDate = date != null ? DateTime.Parse(date) : DateTime.Now,
                        Priority = "Low"
                    };
                }
                else
                {
                    task = _db.Tasks.FirstOrDefault(t => t.TaskId == id);

                    if (task == null)
                    {
                        return NotFound();
                    }
                }

                ViewData["ReturnUrl"] = returnUrl;
                ViewData["Lists"] = _db.Lists.ToList(); // pass lists for dropdown
                return PartialView("~/Views/List/_TaskForm.cshtml", task);
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message} | Inner: {ex.InnerException?.Message}");
            }
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Tasks obj, string returnUrl = null)
        {
            var TaskFromDb = _db.Tasks.Find(obj.TaskId);

            if (TaskFromDb == null)
            {
                TempData["Error"] = "Task not found.";
                return RedirectToAction("Index", "List");
            }

            TaskFromDb.Title = obj.Title;
            TaskFromDb.Description = obj.Description;
            TaskFromDb.Priority = obj.Priority;
            TaskFromDb.DueDate = obj.DueDate;
            TaskFromDb.IsCompleted = obj.IsCompleted;

            _db.SaveChanges();
            TempData["success"] = $"Task updated successfully | ListId: {TaskFromDb.ListId}";

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);  // <-- goes back to calendar if returnUrl is set
            }

            return RedirectToAction("SelectedList", "List", new { id = TaskFromDb.ListId });
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Tasks obj)
        {
            var TaskFromDb = _db.Tasks.Find(obj.TaskId);

            if (TaskFromDb == null)
            {
                TempData["Error"] = "Task not found.";
                return RedirectToAction("Index", "List");
            }

            _db.Tasks.Remove(TaskFromDb);
            _db.SaveChanges();
            TempData["success"] = $"Task deleted successfully | ListId: {TaskFromDb.ListId}";

            return RedirectToAction("SelectedList", "List", new { id = TaskFromDb.ListId });
        }
    }
}
