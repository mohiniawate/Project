
using dialogbot.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace dialogbot.Services
{
    public class ProfessorService
    {
        public List<Professor> getProfessorByDepartment(string department)
        {
            using (var client = new HttpClient())
            {
                var professorUrl = new Uri($"https://dilkapchatbotapi.azurewebsites.net/api/Professor/GetProfessorByDepartment/{department}");
               // var professorUrl = new Uri($"https://localhost:44341/api/Professor/GetProfessorByDepartment/{department}");

                var response = client.GetAsync(professorUrl);
                response.Wait();

                var result = response.Result;
                var professorList = new List<Professor>();
                if (result.IsSuccessStatusCode)
                {
                    professorList = JsonConvert.DeserializeObject<List<Professor>>(result.Content.ReadAsStringAsync().Result);
                }
                return professorList;

            }

        }

    }
}

