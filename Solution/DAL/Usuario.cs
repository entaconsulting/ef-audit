using System;


namespace DAL
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public bool Habilitado { get; set; }

        public int PaisId { get; set; }
        public Pais Pais { get; set; }

    }
}
