using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AppRpgEtec.Services.Enderecos
{
    public class EnderecoService
    {
        private readonly HttpClient _httpClient;
        public EnderecoService()
        {
            _httpClient = new HttpClient();
        }
        public async Task<Endereco> ObterEnderecoPorCepAsync(string cep)
        {
            
            cep = cep.Replace("-", "").Trim();
            
            string url = $"https://viacep.com.br/ws/{cep}/json/";
            
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                return null;
            
            var json = await response.Content.ReadAsStringAsync();
            var endereco = JsonConvert.DeserializeObject<Endereco>(json);
            return endereco;
        }
    }
    
    public class Endereco
    {
        [JsonProperty("cep")]
        public string Cep { get; set; }
        [JsonProperty("logradouro")]
        public string Logradouro { get; set; }
        [JsonProperty("complemento")]
        public string Complemento { get; set; }
        [JsonProperty("bairro")]
        public string Bairro { get; set; }
        [JsonProperty("localidade")]
        public string Cidade { get; set; }
        [JsonProperty("uf")]
        public string Estado { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
