using Microsoft.AspNetCore.Components;

namespace BlazorTamagotchi.Pages
{
    public class NavigationService
    {
        public readonly NavigationManager _navigationManager;
        public NavigationService(NavigationManager navigationManager) 
        { 
            _navigationManager = navigationManager;
        }
    }
}
