using BlazorApp.Services;
using HackTheBlazor.Models;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace HackTheBlazor.Services
{
    public interface IAuthenticationService
    {
        User User { get; }
        Task Initialize();
        Task Login(string username, string password);
        Task Logout();
        Task Register(string firstname, string lastname, string password, string email);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private IHttpService _httpService;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;

        public User User { get; private set; }

        public AuthenticationService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService
        )
        {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
        }

        public async Task Initialize()
        {
            User = await _localStorageService.GetItem<User>("user");
        }

        public async Task Login(string username, string password)
        {
            User = await _httpService.Post<User>("/api/auth/login", new { Email = username, Password = password });
            await _localStorageService.SetItem("user", User);
        }

        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItem("user");
            _navigationManager.NavigateTo("login");
        }

        public async Task Register(string firstname, string lastname, string password, string email)
        {
            User = await _httpService.Post<User>("/api/auth/register", new { Email = email, Password = password, FirstName = firstname, LastName = lastname });
            await _localStorageService.SetItem("user", User);
        }
    }
}