using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCorePaginacionRegistros.Models
{

    public class ModelEmpleadoDeptRegistro
    {//ES EL MODEL
        public Empleado Empleado { get; set; }
        public int Registros { get; set; }
    }
}
