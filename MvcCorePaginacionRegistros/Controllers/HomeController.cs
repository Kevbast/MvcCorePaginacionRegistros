using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.Controllers
{
    public class HomeController : Controller
    {
        private RepositoryHospital repo;

        public HomeController(RepositoryHospital repo)
        {
            this.repo = repo;
        }


        public async Task<IActionResult> Index()
        {
          
                return View();
            
        }
        //public async Task<IActionResult> GetEmpleadosDept(int iddept)
        //{
        //    if (iddept != null)
        //    {
        //        List<Empleado> empleados = await this.repo.GetEmpleadosPorDeptAsync(iddept);
        //        return View(empleados);
        //    }
        //    else
        //    {
        //        return View();
        //    }

        //}

        public async Task<IActionResult> RegistroVistaEmpleado(int iddept,int? posicion)
        {

            ViewData["DEPT_NO"] = iddept;

            ModelEmpleadoDeptRegistro empleado = await this.repo.GetGrupoVistaEmpleado(posicion.Value,iddept);

            if (posicion == null)
            {
                posicion = 1;
            }
            if (iddept == null)
            {
                iddept = 0;
            }



            int numRegistros = empleado.Registros;

            int siguiente = posicion.Value + 1;
            if (siguiente > numRegistros)
            {
                siguiente = numRegistros;
            }
            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {//la posicion ya son efectos visuales 
                anterior = 1;
            }
            ViewData["ULTIMO"] = numRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;


            return View(empleado);
        }




        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
