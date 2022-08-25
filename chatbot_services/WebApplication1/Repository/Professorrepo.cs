using ChatBotApp.Models;
using System;
using System.Data;
using System.Data.SqlClient;

namespace ChatBotApp.Repository
{
    public class Professorrepo
    {
        public List<Professor> GetAllProfessor()
        {
            var Professor = new List<Professor>();
            string connectionString =
                "Data Source=tcp:collegechatbotdb.database.windows.net,1433;Initial Catalog=chatbot;User Id=mohini@collegechatbotdb;Password=Database@123";
              //  "Server=tcp:collegechatbotdb.database.windows.net,1433;Initial Catalog=chatbot;Persist Security Info=False;User ID=mohini;Password=Database@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
          // "Data Source=DESKTOP-353T3MA;Initial Catalog=chatbot;User ID=sa;Password=mohini420";

             string queryString =
                "SELECT Id, Name, MobileNo,EmailId,Department,Subject from dbo.Professor ";

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
                        var professor = new Professor();
                        professor.Id = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Id")));
                        professor.Name = Convert.ToString(reader.GetValue(reader.GetOrdinal("Name")));
                        professor.MobileNo = Convert.ToString(reader.GetValue(reader.GetOrdinal("MobileNo")));
                        professor.EmailId = Convert.ToString(reader.GetValue(reader.GetOrdinal("EmailId")));
                        professor.Department = Convert.ToString(reader.GetValue(reader.GetOrdinal("Department")));
                        professor.Subject = Convert.ToString(reader.GetValue(reader.GetOrdinal("Subject")));


                        Professor.Add(professor);

                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return Professor;
            }
        }

        public List<Professor> getProfessorByDepartment(string department)
        {

            var ProfessorList = new List<Professor>();
            string connectionString =
                 "Server=tcp:collegechatbotdb.database.windows.net,1433;Initial Catalog=chatbot;Persist Security Info=False;User ID=mohini;Password=Database@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
           // "Data Source=DESKTOP-353T3MA;Initial Catalog=chatbot;User ID=sa;Password=mohini420";
            string queryString =
                "SELECT Id,Name, MobileNo, EmailId,Department,Subject from dbo.Professor" +
               $" where Department='{department}' ";
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
                        var Professor = new Professor();
                        Professor.Id = Convert.ToInt32(reader.GetValue(reader.GetOrdinal("Id")));
                        Professor.Name = Convert.ToString(reader.GetValue(reader.GetOrdinal("Name")));
                        Professor.MobileNo = Convert.ToString(reader.GetValue(reader.GetOrdinal("MobileNo")));
                        Professor.EmailId = Convert.ToString(reader.GetValue(reader.GetOrdinal("EmailId")));
                        Professor.Department = Convert.ToString(reader.GetValue(reader.GetOrdinal("Department")));
                        Professor.Subject = Convert.ToString(reader.GetValue(reader.GetOrdinal("Subject")));

                        ProfessorList.Add(Professor);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return ProfessorList;
            }
        }

    }
}
