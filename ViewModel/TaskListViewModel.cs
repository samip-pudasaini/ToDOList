using ToDoList.Models;

namespace ToDoList.ViewModel
{
    public class TaskListViewModel
    {
        public ICollection<List> List { get; set; }
        public List SelectedList { get; set; }

        public ICollection<Tasks> Tasks { get; set; }
    }
}
