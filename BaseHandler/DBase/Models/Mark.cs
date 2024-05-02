using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseHandler.DBase.Models
{
    public class Mark
    {
        public int id {  get; set; }
        public int grade {  get; set; }
        public Subject subject { get; set; }
        public Pupil pupil {  get; set; }
    }
}
