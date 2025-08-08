using System.Collections.Generic;
using DemoDB2.Models;
using DemoDB2.Models.VisitorPattern;

namespace DemoDB2.ViewModels
{
    public class GroupedPhongViewModel
    {
        public string LoaiP { get; set; }
        public List<IPhong> Phongs { get; set; }
    }
}