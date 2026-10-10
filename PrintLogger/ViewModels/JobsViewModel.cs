using PrintLogger.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintLogger.ViewModels
{
    class JobsViewModel
    {
        public List<Job> Jobs { get; set; }
        public List<Job> GetJobs()
        {
            List<Job> jobs = new Job().GetAllJobs();
            return jobs;
        }
    }
}
