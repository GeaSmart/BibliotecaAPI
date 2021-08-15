using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibliotecaAPI.Entidades
{
    public class Autor
    {
        /// <summary>
        /// Identificador del autor
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Nombre del autor
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// fecha de inscripcion del autor en el sindicato
        /// </summary>
        public DateTime FechaInscripcion { get; set; }

        /// <summary>
        /// Colección de libros del autor
        /// </summary>
        public List<Libro> Libros { get; set; }//propiedad de navegación
    }
}
