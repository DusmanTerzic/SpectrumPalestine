using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrekvencijeProject.JSON.Application
{
    public class ApplicationRange
    {
        public long low { get; set; }
        public long high { get; set; }
        public List<Application> applications { get; set; }
    }
}
