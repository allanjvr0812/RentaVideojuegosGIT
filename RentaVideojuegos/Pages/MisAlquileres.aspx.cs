using DataModels;
using System;
using System.Linq;

namespace RentaVideojuegos.Pages
{
    public partial class MisAlquileres : System.Web.UI.Page
    {
        /*
            Esta página muestra únicamente los alquileres del usuario que inició sesión.
            Es la vista principal para los jugadores normales.
        */

        protected void Page_Load(object sender, EventArgs e)
        {
            // Si no hay sesión, el usuario vuelve al login.
            if (Session["IdJugador"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx", true);
                return;
            }

            // La lista se carga solo la primera vez que se abre la página.
            if (!IsPostBack)
            {
                CargarAlquileres();
            }
        }

        private void CargarAlquileres()
        {
            /*
                Se usa el id del jugador guardado en sesión.
                Así el usuario normal solo ve sus propios alquileres.
            */

            try
            {
                int idJugador = Convert.ToInt32(Session["IdJugador"]);

                using (RentaVideojuegosDB db = new RentaVideojuegosDB("DbRentaVideojuegos"))
                {
                    var lista = db.SpObtenerAlquileresPorJugador(idJugador)
                        .ToList()
                        .Select(a => new
                        {
                            a.IdAlquiler,
                            a.NombreSucursal,
                            a.Titulo,
                            FechaInicioTexto = a.FechaInicio.ToString("dd/MM/yyyy"),
                            FechaDevolucionTexto = a.FechaDevolucion.ToString("dd/MM/yyyy"),
                            CostoTotalTexto = a.CostoTotal.ToString("N2"),
                            EstadoTexto = ObtenerEstadoTexto(a.Estado, a.FechaInicio, a.FechaDevolucion)
                        })
                        .ToList();

                    gvAlquileres.DataSource = lista;
                    gvAlquileres.DataBind();
                }
            }
            catch (Exception exc)
            {
                Response.Write("<script>alert('Error al cargar los alquileres: " + exc.Message.Replace("'", "") + "');</script>");
            }
        }

        protected void btnCrear_Click(object sender, EventArgs e)
        {
            // Envía al formulario donde se registra un nuevo alquiler.
            Response.Redirect("~/Pages/CrearAlquiler.aspx", true);
        }

        private string ObtenerEstadoTexto(char estadoBD, DateTime fechaInicio, DateTime fechaDevolucion)
        {
            /*
                Convierte el estado técnico de la base de datos en un texto más claro.
                La base guarda A o I, pero en pantalla se muestra Cancelado, Finalizado,
                En proceso o En espera.
            */

            DateTime hoy = DateTime.Today;

            if (estadoBD == 'I')
                return "Cancelado";

            if (estadoBD == 'A' && fechaDevolucion.Date < hoy)
                return "Finalizado";

            if (estadoBD == 'A' && fechaInicio.Date <= hoy && fechaDevolucion.Date >= hoy)
                return "En proceso";

            if (estadoBD == 'A' && fechaInicio.Date > hoy)
                return "En espera";

            return "Sin estado";
        }
    }
}