using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodoApp.Models

{
    public class Todo
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Summary { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        public bool IsActive { get; set; } 
        public string? AppUserId { get; set; }

        [ForeignKey("AppUserId")]
        public AppUser? User { get; set; }

    }
}
