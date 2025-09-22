using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AppRpgEtec.Models;
using AppRpgEtec.Services.Enderecos;
using AppRpgEtec.Services.Usuarios;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

using Map = Microsoft.Maui.Controls.Maps.Map;

namespace AppRpgEtec.ViewModels.Usuarios
{
    class LocalizacaoViewModel : BaseViewModel
    {
        private UsuarioService uService;

        public LocalizacaoViewModel()
        {
            string token = Preferences.Get("UsuarioToken", string.Empty);
            uService = new UsuarioService(token);
            _enderecoService = new EnderecoService();
            BuscarEnderecoCommand = new Command(BuscarEndereco);
        }
        private Map meuMapa;

        public Map MeuMapa
        {
            get => meuMapa;
            set
            {
                if (value != null)
                {
                    meuMapa = value;
                    OnPropertyChanged();
                }
            }

     
        }
        private readonly EnderecoService _enderecoService;
        private string _cep;
        public string Cep
        {
            get => _cep;
            set
            {
                _cep = value;
                OnPropertyChanged();
            }
        }
        public ICommand BuscarEnderecoCommand { get; }

        public async void InicializarMapa()
        {
            try
            {
                Location location = new Location(-23.5200241d, -46.596498d);
                Pin pinEtec = new Pin()
                {
                    Type = PinType.Place,
                    Label = "Etec Horácio",
                    Address = "Rua alcantara, 113, vila Guilherme",
                    Location = location
                };

                Map map = new Map();
                MapSpan mapSpan = MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(5));
                map.Pins.Add(pinEtec);
                map.MoveToRegion(mapSpan);

                MeuMapa = map;
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", e.Message, "OK");
            }
        }

        public async void ExibirUsuarioNoMapa()
        {
            try
            {
                ObservableCollection<Usuario> ocUsuarios = await uService.GetUsuariosAsync();
                List<Usuario> listaUsuarios = new List<Usuario>(ocUsuarios);
                Map map = new Map();

                foreach (Usuario u in listaUsuarios)
                {
                    if (u.Latitude != null && u.Longitude != null)
                    {
                        double latitude = (double)u.Latitude;
                        double longitude = (double)u.Longitude;
                        Location location = new Location(latitude, longitude);

                        Pin pinAtual = new Pin()
                        {
                            Type = PinType.Place,
                            Label = u.Username,
                            Address = $"E-mail: {u.Email}",
                            Location = location
                        };
                        map.Pins.Add(pinAtual);
                    }
                }
                MeuMapa = map;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Erro", ex.Message, "OK");
            }
        }
            private async void BuscarEndereco()
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(Cep))
                    {
                        await Application.Current.MainPage.DisplayAlert("Erro", "Informe um CEP válido", "OK");
                        return;
                    }
                    var endereco = await _enderecoService.ObterEnderecoPorCepAsync(Cep);
                    if (endereco != null)
                    {
                    Location location = new Location(endereco.Latitude, endereco.Longitude);
                        var pin = new Pin
                        {
                            Type = PinType.Place,
                            Label = endereco.Logradouro,
                            Address = $"{endereco.Bairro}, {endereco.Cidade} - {endereco.Estado}",
                            Location = location
                        };
                        MessagingCenter.Send(this, "AdicionarPin", pin);
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Aviso", "Endereço não encontrado!", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Erro", ex.Message, "OK");
                }
            }
        }
    }
