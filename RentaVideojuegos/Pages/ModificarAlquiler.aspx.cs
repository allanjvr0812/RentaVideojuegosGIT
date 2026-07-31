using DataModels;
using System;
using System.Globalization;
using System.Linq;

namespace RentaVideojuegos.Pages
{
    public partial class ModificarAlquiler : System.Web.UI.Page
    {
        /*
            Esta página permite modificar las fechas de un alquiler.
            Antes de permitir la edición, se revisa si el alquiler está activo,
            si pertenece al usuario o si el usuario es administrador.
        */

        protected void Page_Load(object sender, EventArgs e)
        {
            // Se valida que haya una sesión activa antes de mostrar la página.
            if (Session["IdJugador"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx", true);
                return;
            }

            // Solo se carga la información la primera vez que se abre la página.
            if (!IsPostBack)
            {
                CargarAlquiler();
            }
        }

        private void CargarAlquiler()
        {
            /*
                Se toma el id del alquiler desde la URL.
                Si el id no viene o no es válido, se devuelve al usuario a su lista.
            */

            int idAlquiler;

            if (!int.TryParse(Request.QueryString["id"], out idAlquiler))
            {
                RegresarSegunRol();
                return;
            }

            try
            {
                using (RentaVideojuegosDB db = new RentaVideojuegosDB("DbRentaVideojuegos"))
                {
                    // Se consulta el detalle del alquiler que se quiere modificar.
                    var detalle = db.SpObtenerDetalleAlquiler(idAlquiler).FirstOrDefault();

                    if (detalle == null)
                    {
                        RegresarSegunRol();
                        return;
                    }

                    bool esAdministrador = Convert.ToBoolean(Session["EsAdministrador"]);
                    int idJugadorSesion = Convert.ToInt32(Session["IdJugador"]);

                    /*
                        Si es jugador normal, solo puede modificar sus propios alquileres.
                        El administrador sí puede modificar alquileres de cualquier jugador activo.
                    */
                    if (!esAdministrador && detalle.IdJugador != idJugadorSesion)
                    {
                        Response.Redirect("~/Pages/MisAlquileres.aspx", true);
                        return;
                    }

                    // Si el alquiler ya está cancelado, no se puede modificar.
                    if (detalle.Estado == 'I')
                    {
                        RegresarSegunRol();
                        return;
                    }

                    // Si ya se cumplió la fecha de devolución, tampoco se permite modificar.
                    if (detalle.FechaDevolucion.Date <= DateTime.Today)
                    {
                        RegresarSegunRol();
                        return;
                    }

                    /*
                        Si el alquiler ya inició, el jugador normal no puede modificarlo.
                        Esta restricción no se aplica igual al administrador.
                    */
                    if (!esAdministrador && detalle.FechaInicio.Date <= DateTime.Today)
                    {
                        Response.Redirect("~/Pages/MisAlquileres.aspx", true);
                        return;
                    }

                    // Se guardan estos datos para usarlos al guardar los cambios.
                    ViewState["IdAlquiler"] = detalle.IdAlquiler;
                    ViewState["FechaInicioOriginal"] = detalle.FechaInicio;

                    // Se muestran los datos principales del alquiler.
                    lblIdAlquiler.Text = detalle.IdAlquiler.ToString();
                    lblJugador.Text = detalle.NombreCompleto;
                    lblSucursal.Text = detalle.NombreSucursal;
                    lblVideojuego.Text = detalle.Titulo;

                    txtFechaInicio.Text = detalle.FechaInicio.ToString("yyyy-MM-dd");
                    txtFechaDevolucion.Text = detalle.FechaDevolucion.ToString("yyyy-MM-dd");

                    /*
                        Si la fecha de inicio ya se cumplió, se bloquea el campo.
                        Esto evita que se cambie una fecha de inicio pasada o del día actual.
                    */
                    if (detalle.FechaInicio.Date <= DateTime.Today)
                    {
                        txtFechaInicio.Enabled = false;
                    }
                }
            }
            catch (Exception exc)
            {
                MostrarMensaje("Error al cargar el alquiler: " + exc.Message, true);
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            /*
                Este evento valida las fechas digitadas y luego llama al procedimiento
                almacenado que actualiza el alquiler y registra la bitácora.
            */

            if (!Page.IsValid)
                return;

            if (ViewState["IdAlquiler"] == null)
            {
                RegresarSegunRol();
                return;
            }

            DateTime fechaInicio;
            DateTime fechaDevolucion;

            /*
                Si el campo fecha de inicio está bloqueado, se conserva la fecha original.
                Si está habilitado, se valida lo que digitó el usuario.
            */
            if (!txtFechaInicio.Enabled)
            {
                fechaInicio = Convert.ToDateTime(ViewState["FechaInicioOriginal"]);
            }
            else
            {
                if (!DateTime.TryParseExact(txtFechaInicio.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaInicio))
                {
                    MostrarMensaje("La fecha de inicio no tiene un formato válido.", true);
                    return;
                }

                if (fechaInicio.Date <= DateTime.Today)
                {
                    MostrarMensaje("La fecha de inicio debe ser mayor a la fecha actual.", true);
                    return;
                }
            }

            if (!DateTime.TryParseExact(txtFechaDevolucion.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaDevolucion))
            {
                MostrarMensaje("La fecha de devolución no tiene un formato válido.", true);
                return;
            }

            if (fechaDevolucion.Date <= DateTime.Today)
            {
                MostrarMensaje("La fecha de devolución debe ser mayor a la fecha actual.", true);
                return;
            }

            if (fechaDevolucion.Date < fechaInicio.Date)
            {
                MostrarMensaje("La fecha de devolución no puede ser menor que la fecha de inicio.", true);
                return;
            }

            try
            {
                int idAlquiler = Convert.ToInt32(ViewState["IdAlquiler"]);
                int idJugadorSesion = Convert.ToInt32(Session["IdJugador"]);

                using (RentaVideojuegosDB db = new RentaVideojuegosDB("DbRentaVideojuegos"))
                {
                    db.SpModificarAlquiler(idAlquiler, fechaInicio, fechaDevolucion, idJugadorSesion).FirstOrDefault();
                }

                // Después de modificar, se muestra nuevamente el comprobante del alquiler.
                Response.Redirect("~/Pages/DetalleAlquiler.aspx?id=" + idAlquiler, true);
            }
            catch (Exception exc)
            {
                MostrarMensaje("No se pudo modificar el alquiler: " + exc.Message, true);
            }
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            // Si veníamos de un alquiler válido, regresamos al detalle. Si no, regresamos a la lista.
            if (ViewState["IdAlquiler"] != null)
                Response.Redirect("~/Pages/DetalleAlquiler.aspx?id=" + ViewState["IdAlquiler"], true);
            else
                RegresarSegunRol();
        }

        private void RegresarSegunRol()
        {
            // Regresa al usuario a la pantalla que le corresponde según su tipo de cuenta.
            bool esAdministrador = Convert.ToBoolean(Session["EsAdministrador"]);

            if (esAdministrador)
                Response.Redirect("~/Pages/GestionarAlquileres.aspx", true);
            else
                Response.Redirect("~/Pages/MisAlquileres.aspx", true);
        }

        private void MostrarMensaje(string mensaje, bool esError)
        {
            // Muestra mensajes de error o confirmación usando los estilos definidos en la Master Page.
            lblMensaje.Text = mensaje;
            lblMensaje.Visible = true;
            lblMensaje.CssClass = esError ? "mensaje-error" : "mensaje-correcto";
        }
    }
}