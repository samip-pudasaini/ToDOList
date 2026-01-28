using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    public class List
    {
        [Key]
        public int ListId { get; set; }
        [Required]
        public string Name { get; set; }

        [Required(ErrorMessage = "List name is required.")]
        [StringLength(100, ErrorMessage = "List name is too long.")]

        // Navigation property for related tasks
        public ICollection<Tasks> Tasks { get; set; }
    }
}
