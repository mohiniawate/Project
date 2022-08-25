using ChatBotApp.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace ChatBotApp.Repository
{
    public class studentrepo
    {
        public List<Student> GetAllStudent()
        {
            var students = new List<Student>();
            string connectionString =
                 "Server=tcp:collegechatbotdb.database.windows.net,1433;Initial Catalog=chatbot;Persist Security Info=False;User ID=mohini;Password=Database@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            //"Data Source=DESKTOP-353T3MA;Initial Catalog=chatbot;User ID=sa;Password=mohini420";

           
            string queryString =
                "SELECT StudentID,studentName, Stream, Year,password,EmailId,MobileNo,Attendance,PendingFees from dbo.student ";
           

            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
           
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine("\t{0}\t{1}\t{2}",
                            reader[0], reader[1], reader[2]);
                        var student = new Student();
                        student.StudentId = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("studentId")));
                        student.StudentName = Convert.ToString(reader.GetValue(reader.GetOrdinal("StudentName")));
                        student.Stream = Convert.ToString(reader.GetValue(reader.GetOrdinal("Stream")));
                        student.Year = Convert.ToString(reader.GetValue(reader.GetOrdinal("Year")));
                        student.Password = Convert.ToString(reader.GetValue(reader.GetOrdinal("Password")));
                        student.EmailId= Convert.ToString(reader.GetValue(reader.GetOrdinal("EmailId")));
                        student.MobileNo = Convert.ToString(reader.GetValue(reader.GetOrdinal("MobileNo")));
                        student.Attendance = Convert.ToString(reader.GetValue(reader.GetOrdinal("Attendance")));
                        student.PendingFees = Convert.ToString(reader.GetValue(reader.GetOrdinal("PendingFees")));

                        students.Add(student);

                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return students;
            }
        }

        public Student getstudent(string mobileno, string password)
        {

            var student = new Student();
            string connectionString =
                 "Server=tcp:collegechatbotdb.database.windows.net,1433;Initial Catalog=chatbot;Persist Security Info=False;User ID=mohini;Password=Database@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            //"Data Source=DESKTOP-353T3MA;Initial Catalog=chatbot;User ID=sa;Password=mohini420";
            string queryString =
                "SELECT StudentID,studentName, Stream, Year,password,EmailId,MobileNo,Attendance,PendingFees from dbo.student" +
               $" where MobileNo={mobileno} AND password={password} ";
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        Console.WriteLine("\t{0}\t{1}\t{2}",
                            reader[0], reader[1], reader[2]);
                        student.StudentId = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("studentId")));
                        student.StudentName = Convert.ToString(reader.GetValue(reader.GetOrdinal("StudentName")));
                        student.Stream = Convert.ToString(reader.GetValue(reader.GetOrdinal("Stream")));
                        student.Year = Convert.ToString(reader.GetValue(reader.GetOrdinal("Year")));
                        student.Password = Convert.ToString(reader.GetValue(reader.GetOrdinal("Password")));
                        student.EmailId = Convert.ToString(reader.GetValue(reader.GetOrdinal("EmailId")));
                        student.MobileNo = Convert.ToString(reader.GetValue(reader.GetOrdinal("MobileNo")));
                        student.Attendance = Convert.ToString(reader.GetValue(reader.GetOrdinal("Attendance")));
                        student.PendingFees= Convert.ToString(reader.GetValue(reader.GetOrdinal("PendingFees")));


                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return student;
            }
        }
        
    }
}
