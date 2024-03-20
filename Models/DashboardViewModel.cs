using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pitstop.Models;

namespace Pitstop.Models
{
public class DashboardViewModel
{
    public List<Product> Products { get; set; }
    public long TotalQuantity { get; set; }
    public List<Category> Categorys { get; set; }
    public List<string> Statuses { get; set; }
}

}
