using Oiski.School.THOP.App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Oiski.School.THOP.App.Services
{
    public class ApiService
    {
        private readonly HTTPService _service;

        public ApiService(HTTPService service)
        {
            _service = service;
        }

        public async Task<List<HumidexDto>> GetAllAsync()
        {
            return await _service.Client.GetFromJsonAsync<List<HumidexDto>>("thop/humidex");
        }
    }
}
