using Artium.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Artium.Models.Objects
{
    public class Report
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public int Resolved { get; set; }
        public int? WallPostId { get; set; }
        public WallPost? WallPost { get; set; }
        
    }
}
