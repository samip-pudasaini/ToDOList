using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoList.Models
{
    public class Tasks
    {
        [Key]
        public int TaskId { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; } = false;
        
        [DataType(DataType.Date)]
        [Column(TypeName = "date")]
        public DateTime DueDate { get; set; }

        [Required]
        public string Priority { get; set; } = "Low";

        // Navigation property for related list
        public List List { get; set; }

        // Foreign key for List
        public int ListId { get; set; }
    }
}
