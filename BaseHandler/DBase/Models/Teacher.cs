using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseHandler.DBase.Models
{
    public class Teacher
    {
        public int teacher_id { get; set; }
        public string teacher_fullname { get; set; }
        public int teacher_spec { get; set; }
        public int teacher_auditory { get; set; }
        public Subject teacher_subject;

        public Teacher() 
        {
            teacher_subject = SubjectsController.GetSubjectByID(teacher_spec);
        }

        public Subject GetSubject()
        {
            return SubjectsController.GetSubjectByID(teacher_spec);
        }



    }
}
