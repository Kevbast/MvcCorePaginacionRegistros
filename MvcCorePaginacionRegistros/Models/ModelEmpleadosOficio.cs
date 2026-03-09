using System.ComponentModel.DataAnnotations.Schema;

namespace MvcCorePaginacionRegistros.Models
{
    public class ModelEmpleadosOficio
    {//PARA RECIBIR LOS VALORES DEL PROCEDURE
        public List<Empleado> Empleados { get; set; }
        public int NumeroRegistros { get; set; }

    }
}
