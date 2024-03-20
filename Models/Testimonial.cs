using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pitstop.Models
{
    public class Testimonial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Name { get; set; }
        
        public string? Description { get; set; }
        
        public string? CreatedDate { get; set; }
        
        public string? Image { get; set; }

    }
}
