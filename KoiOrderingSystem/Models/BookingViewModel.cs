using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KoiOrderingSystem.Models
{
    public class BookingViewModel
    {
        [Required(ErrorMessage = "CustomerId is required.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "StartDate is required.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "EndDate is required.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
    
    public IEnumerable<SelectListItem> Customers { get; set; } // Danh sách khách hàng
    }
}
