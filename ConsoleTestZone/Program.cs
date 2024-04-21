using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseHandler.DBase;
using BaseHandler.DBase.Models;

namespace ConsoleTestZone
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = DBaseConnector.getBaseConnection();
            Console.WriteLine(connection.ConnectionString);
            Console.WriteLine(connection.State);

            List<Teacher> teachers = TeacherController.GetTeachers();

            foreach (Teacher teacher in teachers) 
            {
                Console.WriteLine(teacher.teacher_fullname);
            }

            Console.ReadKey();
        }
    }
}
