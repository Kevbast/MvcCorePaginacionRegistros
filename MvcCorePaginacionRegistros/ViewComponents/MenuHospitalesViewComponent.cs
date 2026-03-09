using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.ViewComponents
{
    public class MenuHospitalesViewComponent:ViewComponent
    {
        private RepositoryHospital repo;

        public MenuHospitalesViewComponent(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Hospital> hospitales = await this.repo.GetAllHospitalesAsync();
            return View(hospitales);
        }

    }
}
