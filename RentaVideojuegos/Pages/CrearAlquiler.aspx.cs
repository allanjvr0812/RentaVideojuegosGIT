using DataModels;
using System;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;

namespace RentaVideojuegos.Pages
{
    public partial class CrearAlquiler : System.Web.UI.Page
    {
        /*
            Esta página permite registrar un nuevo alquiler.
            El usuario selecciona la sucursal, el jugador y las fechas.
            El videojuego no se selecciona manualmente, porque el procedimiento
            almacenado lo asigna automáticamente según disponibilidad.
        */

        protected void Page_Load(object sender, EventArgs e)
        {
            // Si no hay una sesión activa, el usuario vuelve al login.
            if (Session["IdJugador"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx", true);
                return;
            }

            // Los combos se cargan solo la primera vez que se abre la página.
            if (!IsPostBack)
            {
                CargarSucursales();
                CargarJugadores();
            }
        }

        private void CargarSucursales()
        {
            /*
                Carga las sucursales activas desde la base de datos.
                El usuario debe escoger una sucursal para registrar el alquiler.
            */

            try
            {
                using (RentaVideojuegosDB db = new RentaVideojuegosDB("DbRentaVideojuegos"))
                {
                    ddlSucursal.DataSource = db.SpObtenerSucursalesActivas().ToList();
                    ddlSucursal.DataTextField = "Nombre";
                    ddlSucursal.DataValueField = "IdSucursal";
                    ddlSucursal.DataBind();
                }

                ddlSucursal.Items.Insert(0, new ListItem("Seleccione una sucursal", ""));
            }
            catch (Exception exc)
            {
                MostrarMensaje("Error al cargar las sucursales: " + exc.Message, true);
            }
        }

        private void CargarJugadores()
        {
            /*
                Si el usuario es administrador, puede seleccionar a cualquier jugador activo.
                Si es jugador normal, el sistema coloca automáticamente su propio usuario.
            */

            try
            {
                bool esAdministrador = Convert.ToBoolean(Session["EsAdministrador"]);
                int idJugadorSesion = Convert.ToInt32(Session["IdJugador"]);
                string nombreCompleto = Convert.ToString(Session["NombreCompleto"]);

                ddlJugador.Items.Clear();

                if (esAdministrador)
                {
                    using (RentaVideojuegosDB db = new RentaVideojuegosDB("DbRentaVideojuegos"))
                    {
                        ddlJugador.DataSource = db.SpObtenerJugadoresActivos().ToList();
                        ddlJugador.DataTextField = "NombreCompleto";
                        ddlJugador.DataValueField = "IdJugador";
                        ddlJugador.DataBind();
                    }

                    ddlJugador.Items.Insert(0, new ListItem("Seleccione un jugador", ""));
                    ddlJugador.Enabled = true;
                }
                else
                {
                    ddlJugador.Items.Add(new ListItem(nombreCompleto, idJugadorSesion.ToString()));
                    ddlJugador.SelectedValue = idJugadorSesion.ToString();
                    ddlJugador.Enabled = false;
                }
            }
            catch (Exception exc)
            {
                MostrarMensaje("Error al cargar los jugadores: " + exc.Message, true);
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            /*
                Este evento valida la información del alquiler.
                Si todo está correcto, llama al procedimiento almacenado que crea el alquiler,
                calcula los montos y asigna automáticamente un videojuego disponible.
            */

            if (!Page.IsValid)
                return;

            try
            {
                int idSucursal = Convert.ToInt32(ddlSucursal.SelectedValue);
                int idJugadorAlquiler = Convert.ToInt32(ddlJugador.SelectedValue);
                int idJugadorSesion = Convert.ToInt32(Session["IdJugador"]);

                DateTime fechaInicio;
                DateTime fechaDevolucion;

                /*
                    Los TextBox de fecha usan formato yyyy-MM-dd porque así trabaja
                    el control TextMode="Date" en ASP.NET.
                */
                if (!DateTime.TryParseExact(txtFechaInicio.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaInicio))
                {
                    MostrarMensaje("La fecha de inicio no tiene un formato válido.", true);
                    return;
                }

                if (!DateTime.TryParseExact(txtFechaDevolucion.Text.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaDevolucion))
                {
                    MostrarMensaje("La fecha de devolución no tiene un formato válido.", true);
                    return;
                }

                /*
                    Según las reglas del proyecto, la fecha de inicio debe ser mayor
                    a la fecha actual. Por eso no se permite crear alquiler para hoy.
                */
                if (fechaInicio.Date <= DateTime.Today)
                {
                    MostrarMensaje("La fecha de inicio debe ser mayor a la fecha actual.", true);
                    return;
                }

                if (fechaDevolucion.Date < fechaInicio.Date)
                {
                    MostrarMensaje("La fecha de devolución no puede ser menor que la fecha de inicio.", true);
                    return;
                }

                using (RentaVideojuegosDB db = new RentaVideojuegosDB("DbRentaVideojuegos"))
                {
                    /*
                        El procedimiento almacenado se encarga de escoger el videojuego
                        disponible de la sucursal. También registra la bitácora de creación.
                    */
                    var resultado = db.SpCrearAlquiler(
                        idJugadorAlquiler,
                        idSucursal,
                        fechaInicio,
                        fechaDevolucion,
                        idJugadorSesion
                    ).FirstOrDefault();

                    if (resultado != null && resultado.IdAlquiler.HasValue)
                    {
                        Response.Redirect("~/Pages/DetalleAlquiler.aspx?id=" + resultado.IdAlquiler.Value, true);
                    }
                    else
                    {
                        MostrarMensaje("El alquiler fue creado, pero no se pudo obtener el número de alquiler.", true);
                    }
                }
            }
            catch (Exception exc)
            {
                MostrarMensaje("No se pudo crear el alquiler: " + exc.Message, true);
            }
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            // Regresa a la pantalla correspondiente según el rol del usuario.
            RegresarSegunRol();
        }

        private void RegresarSegunRol()
        {
            /*
                El administrador vuelve a la gestión general.
                El jugador normal vuelve a la lista de sus propios alquileres.
            */

            bool esAdministrador = Convert.ToBoolean(Session["EsAdministrador"]);

            if (esAdministrador)
                Response.Redirect("~/Pages/GestionarAlquileres.aspx", true);
            else
                Response.Redirect("~/Pages/MisAlquileres.aspx", true);
        }

        private void MostrarMensaje(string mensaje, bool esError)
        {
            // Muestra mensajes de error o confirmación en la página.
            lblMensaje.Text = mensaje;
            lblMensaje.Visible = true;
            lblMensaje.CssClass = esError ? "mensaje-error" : "mensaje-correcto";
        }
    }
}