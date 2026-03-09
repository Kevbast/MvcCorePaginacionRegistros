using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.ViewComponents
{
    //heredamos
    public class MenuDepartamentosViewComponent:ViewComponent
    {
        private RepositoryHospital repo;

        public MenuDepartamentosViewComponent(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Departamento> departamentos = await this.repo.GetAllDepartamentosAsync();
            return View(departamentos);
        }





    }
}
