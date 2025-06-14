using AgenciaDeViajes.Models;
using System.Collections.Generic;

namespace AgenciaDeViajes.ViewModels
{
    public class AdminPanelViewModel
    {
        public AdminEstadisticasViewModel Resumen { get; set; }
        public UsuariosPorMesViewModel UsuariosPorMes { get; set; }

        // NUEVO: Total boletos vendidos
        public int TotalBoletosVendidos { get; set; }

        // NUEVO: Lista de usuarios que compraron
        public List<string> UsuariosQueCompraron { get; set; }
    }
}
