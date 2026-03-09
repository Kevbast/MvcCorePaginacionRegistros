using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Models;

namespace MvcCorePaginacionRegistros.Data
{
    public class HospitalContext:DbContext
    {
        public HospitalContext(DbContextOptions<HospitalContext> options) : base(options) { }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<VistaDepartamento> VistaDepartamentos { get; set; }
        //Ejercicio en casa
        public DbSet<Hospital> Hospitales{ get; set; }
        public DbSet<Plantilla> EmpleadosPlantilla { get; set; }
    }
}
