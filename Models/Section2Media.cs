using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pitstop.Models
{
    public class Section2Media
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? FileName { get; set; }
        
        public string? FileType { get; set; }
        
        public string? Caption { get; set; }
        
        public DateTime? UploadDate { get; set; }

        public int CarouselNumber { get; set; }
    }
}
