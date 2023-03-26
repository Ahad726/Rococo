using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Rococo.Models
{
    public class ShoppingCart
    {
        [Key]
        public int Id { get; set; }

        [Required]  // Also Forces Cascade delete
        public string ApplicationUserId { get; set; }
       
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }


        [Required]  //<======= Forces Cascade delete
        public int ProductId { get; set; }
       
        [ForeignKey("ProductId")]
        public Product Product { get; set; }

        [Range(1, 1000, ErrorMessage = ("Please Enter a value between 1 o 1000"))]
        public int Count { get; set; } = 1;

        [NotMapped]
        public double Price { get; set; }


    }
}
