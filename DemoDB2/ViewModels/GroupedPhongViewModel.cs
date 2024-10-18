using System.Collections.Generic;
using DemoDB2.Models;

namespace DemoDB2.ViewModels
{
    public class GroupedPhongViewModel
    {
        public string LoaiP { get; set; }
        public List<Phong> Phongs { get; set; }
    }
}