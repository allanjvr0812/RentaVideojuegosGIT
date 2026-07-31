using DataModels;
using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;

namespace RentaVideojuegos.Pages
{
    public partial class GestionarAlquileres : System.Web.UI.Page
    {
        /*
            Esta página es para el administrador.
            Permite ver todos los alquileres del sistema y aplicar filtros
            por jugador y por rango de fechas.
        */

        protected void Page_Load(object sender, EventArgs e)
        {
            // Primero se valida que exista una sesión activa.
            if (Session["IdJugador"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx", true);
                return;
            }

            bool esAdministrador = Convert.ToBoolean(Session["EsAdministrador"]);

            /*
                Esta pantalla solo la puede usar un administrador.
                Si entra un jugador normal, se envía a la pantalla de sus propios alquileres.
            */
            if (!esAdministrador)
            {
                Response.Redirect("~/Pages/MisAlquileres.aspx", true);
                return;
            }

            // Los datos se cargan solo la primera vez que abre la página.
            if (!IsPostBack)
            {
                CargarJugadores();
                CargarAlquileres(null, null, null);
            }
        }

        private void CargarJugadores()
        {
            /*
                Carga la lista de jugadores activos para usarla como filtro.
                La primera opción permite consultar todos los jugadores.
            */

            try
            {
                using (RentaVideojuegosDB db = new RentaVideojuegosDB("DbRentaVideojuegos"))
                {
                    ddlJugador.DataSource = db.SpObtenerJugadoresActivos().ToList();
                    ddlJugador.DataTextField = "NombreCompleto";
                    ddlJugador.DataValueField = "IdJugador";
                    ddlJugador.DataBind();
                }

                ddlJugador.Items.Insert(0, new ListItem("Todos los jugadores", ""));
            }
            catch (Exception exc)
            {
                MostrarError("Error al cargar jugadores: " + exc.Message);
            }
        }

        private void CargarAlquileres(int? idJugador, DateTime? fechaInicio, DateTime? fechaDevolucion)
        {
            /*
                Consulta los alquileres del sistema.
                Si los filtros vienen vacíos, se muestran todos los alquileres.
                Si los filtros tienen datos, se envían al procedimiento almacenado.
            */

            try
            {
                using (RentaVideojuegosDB db = new RentaVideojuegosDB("DbRentaVideojuegos"))
                {
                    var lista = db.SpObtenerTodosAlquileres(idJugador, fechaInicio, fechaDevolucion)
                        .ToList()
                        .Select(a => new
                        {
                            a.IdAlquiler,
                            a.NombreCompleto,
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
                MostrarError("Error al cargar alquileres: " + exc.Message);
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            /*
                Este botón aplica los filtros de búsqueda.
                El jugador es opcional, pero si se usan fechas deben venir ambas.
            */

            lblMensajeError.Visible = false;

            int? idJugador = null;
            DateTime? fechaInicio = null;
            DateTime? fechaDevolucion = null;

            if (!string.IsNullOrEmpty(ddlJugador.SelectedValue))
            {
                idJugador = Convert.ToInt32(ddlJugador.SelectedValue);
            }

            /*
                Si el usuario escribe una fecha, debe escribir las dos.
                Esto evita búsquedas incompletas o confusas.
            */
            if (!string.IsNullOrEmpty(txtFechaInicio.Text.Trim()) || !string.IsNullOrEmpty(txtFechaDevolucion.Text.Trim()))
            {
                if (string.IsNullOrEmpty(txtFechaInicio.Text.Trim()) || string.IsNullOrEmpty(txtFechaDevolucion.Text.Trim()))
                {
                    MostrarError("Debe indicar fecha de inicio y fecha de devolución.");
                    return;
                }

                DateTime fechaInicioTemp;
                DateTime fechaDevolucionTemp;

                if (!DateTime.TryParseExact(txtFechaInicio.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaInicioTemp))
                {
                    MostrarError("La fecha de inicio no tiene un formato válido.");
                    return;
                }

                if (!DateTime.TryParseExact(txtFechaDevolucion.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaDevolucionTemp))
                {
                    MostrarError("La fecha de devolución no tiene un formato válido.");
                    return;
                }

                if (fechaDevolucionTemp.Date < fechaInicioTemp.Date)
                {
                    MostrarError("La fecha de devolución no puede ser menor que la fecha de inicio.");
                    return;
                }

                fechaInicio = fechaInicioTemp;
                fechaDevolucion = fechaDevolucionTemp;
            }

            CargarAlquileres(idJugador, fechaInicio, fechaDevolucion);
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            /*
                Limpia los filtros de búsqueda y vuelve a mostrar todos los alquileres.
                No elimina datos de la base de datos.
            */

            ddlJugador.SelectedIndex = 0;
            txtFechaInicio.Text = "";
            txtFechaDevolucion.Text = "";
            lblMensajeError.Visible = false;

            CargarAlquileres(null, null, null);
        }

        protected void btnCrear_Click(object sender, EventArgs e)
        {
            // Envía al administrador al formulario para crear un nuevo alquiler.
            Response.Redirect("~/Pages/CrearAlquiler.aspx", true);
        }

        private string ObtenerEstadoTexto(char estadoBD, DateTime fechaInicio, DateTime fechaDevolucion)
        {
            /*
                Convierte el estado guardado en base de datos en un texto entendible.
                Esto permite mostrar Cancelado, Finalizado, En proceso o En espera.
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

        private void MostrarError(string mensaje)
        {
            // Muestra un mensaje en pantalla cuando ocurre un error o una validación.
            lblMensajeError.Text = mensaje;
            lblMensajeError.Visible = true;
        }
    }
}