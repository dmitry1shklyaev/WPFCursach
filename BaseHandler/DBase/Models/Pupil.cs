using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseHandler.DBase.Models
{
    public class Pupil
    {
        public int pupil_id {  get; set; }
        public string pupil_name { get; set; }
        public Schoolclass pupil_class {  get; set; }
        public List<Subject> pupil_subjects { get; private set; }

        public Pupil()
        {
            pupil_class = new Schoolclass();
            pupil_class.class_grade = SchoolclassesController.GetSchoolclassByID(pupil_class.class_id).class_grade;
            pupil_subjects = SubjectsController.GetSubject();
        }
    }
}
