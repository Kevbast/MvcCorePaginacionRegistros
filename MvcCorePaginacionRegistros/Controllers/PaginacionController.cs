using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;
using System.Threading.Tasks;

namespace MvcCorePaginacionRegistros.Controllers
{
    public class PaginacionController : Controller
    {
        private RepositoryHospital repo;

        public PaginacionController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> RegistroVistaDepartmento(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            int numRegistros = await this.repo.GetNumerosRegistrosVistaDepartamentosAsync();
            //EL PRIMERO ES 1
            //ULTIMO =4
            //ANTERIOR = POSICION -1
            //SIGUIENTE POSICION-1
            int siguiente = posicion.Value + 1;
            if (siguiente > numRegistros)
            {
                siguiente = numRegistros;
            }
            int anterior = posicion.Value - 1;
            if(anterior < 1)
            {//la posicion ya son efectos visuales 
                anterior = 1;
            }
            ViewData["ULTIMO"] = numRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;

            VistaDepartamento departamento = await this.repo.GetVistaDepartamentoAsync(posicion.Value);

            return View(departamento);
        }

        public async Task<IActionResult> GrupoVistaDepartamentos(int? posicion)
        {
            if (posicion == null)
                posicion = 1;
            //LO SIGUIENTE SERA QUE DEBEMOS D LOS NUMERO DE PAGINA EN LOS LINKS
            //<a href='grupodepts?posicion=1'>Pagina 1</a> 
            //<a href='grupodepts?posicion=3'>Pagina 2</a> 
            //<a href='grupodepts?posicion=5'>Pagina 3</a>
            //NECESITAMOS UNA VARIABLE PARA EL NUMNUMERO DE PAGINA
            //VOY A REALIZAR EL DIBUJO DESDE AQUÍ,NO DESDE RAZOR
            ViewData["NUMREGISTRO"] = await this.repo.GetNumerosRegistrosVistaDepartamentosAsync();
            List<VistaDepartamento> departamentos =
                await this.repo.GetGrupoVistaDepartamento(posicion.Value);
            return View(departamentos);
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GrupoDepartamentos(int? posicion)
        {
            if (posicion == null)
                posicion = 1;
            //LO SIGUIENTE SERA QUE DEBEMOS D LOS NUMERO DE PAGINA EN LOS LINKS
            //<a href='grupodepts?posicion=1'>Pagina 1</a> 
            //<a href='grupodepts?posicion=3'>Pagina 2</a> 
            //<a href='grupodepts?posicion=5'>Pagina 3</a>
            //NECESITAMOS UNA VARIABLE PARA EL NUMNUMERO DE PAGINA
            //VOY A REALIZAR EL DIBUJO DESDE AQUÍ,NO DESDE RAZOR
            ViewData["NUMREGISTRO"] = await this.repo.GetNumerosRegistrosVistaDepartamentosAsync();
            List<Departamento> departamentos =
                await this.repo.GetGrupoDepartamentosAsync(posicion.Value);
            return View(departamentos);
        }

        public async Task<IActionResult> GrupoEmpleados(int? posicion)
        {
            if (posicion == null)
                posicion = 1;
            ViewData["NUMREGISTRO"] = await this.repo.GetEmpleadosCountAsync();//numero de registros en EMPLEADO
            List<Empleado> empleados =
                await this.repo.GetGrupoEmpleadosAsync(posicion.Value);
            return View(empleados);
        }

        //PAGINACION POR OFICIOS
        public async Task<IActionResult> GrupoEmpleadosOficios(int? posicion,string? oficio)
        {
            if (posicion == null)
            {
                posicion = 1;
                return View();
            }
            else
            {

            List<Empleado> empleados =
                await this.repo.GetGrupoEmpleadosOficiosAsync(posicion.Value,oficio);

            ViewData["NUMREGISTRO"] = await this.repo.GetEmpleadosOficioCountAsync(oficio);//numero de registros en EMPLEADO
            ViewData["OFICIO"] = oficio;
            
            return View(empleados);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GrupoEmpleadosOficios( string oficio)
        {

                List<Empleado> empleados =
                    await this.repo.GetGrupoEmpleadosOficiosAsync(1, oficio);

                ViewData["NUMREGISTRO"] = await this.repo.GetEmpleadosOficioCountAsync(oficio);//numero de registros en EMPLEADO
                ViewData["OFICIO"] = oficio;

                return View(empleados);
            
        }

        //PAGINACION POR OFICIOS OUT
        public async Task<IActionResult> GrupoEmpleadosOficiosOut(int? posicion, string? oficio)
        {
            if (posicion == null)
            {
                posicion = 1;
                return View();
            }
            else
            {

                ModelEmpleadosOficio model=
                    await this.repo.GetGrupoEmpleadosOficiosOUTAsync(posicion.Value, oficio);

                ViewData["NUMREGISTRO"] = model.NumeroRegistros; //numero de registros en EMPLEADO
                ViewData["OFICIO"] = oficio;

                return View(model);
            }
        }
        [HttpPost]
        public async Task<IActionResult> GrupoEmpleadosOficiosOut(string oficio)
        {

            ModelEmpleadosOficio model =
                    await this.repo.GetGrupoEmpleadosOficiosOUTAsync(1, oficio);//cambiamos pos por un 1

            ViewData["NUMREGISTRO"] = model.NumeroRegistros; //numero de registros en EMPLEADO
            ViewData["OFICIO"] = oficio;

            return View(model);

        }




    }
}
