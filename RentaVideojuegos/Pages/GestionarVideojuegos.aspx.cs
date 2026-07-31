using DataModels;
using System;
using System.Linq;

namespace RentaVideojuegos.Pages
{
    public partial class GestionarVideojuegos : System.Web.UI.Page
    {
        /*
            Esta página permite al administrador consultar la lista de videojuegos.
            Desde aquí también se puede ir al formulario para crear o editar videojuegos.
        */

        protected void Page_Load(object sender, EventArgs e)
        {
            // Ninguna página interna debe abrir si el usuario no ha iniciado sesión.
            if (Session["IdJugador"] == null)
            {
                Response.Redirect("~/Pages/Login.aspx", true);
                return;
            }

            bool esAdministrador = Convert.ToBoolean(Session["EsAdministrador"]);

            /*
                La gestión de videojuegos es exclusiva del administrador.
                Si un jugador normal intenta entrar, se devuelve a sus alquileres.
            */
            if (!esAdministrador)
            {
                Response.Redirect("~/Pages/MisAlquileres.aspx", true);
                return;
            }

            // La lista se carga solo al abrir la página por primera vez.
            if (!IsPostBack)
            {
                CargarVideojuegos();
            }
        }

        private void CargarVideojuegos()
        {
            /*
                Consulta los videojuegos registrados en la base de datos.
                También se transforma el estado A/I en Activo/Inactivo para que se entienda mejor.
            */

            try
            {
                using (RentaVideojuegosDB db = new RentaVideojuegosDB("DbRentaVideojuegos"))
                {
                    var lista = db.SpObtenerVideojuegos()
                        .ToList()
                        .Select(v => new
                        {
                            v.IdVideojuego,
                            v.NombreSucursal,
                            v.Titulo,
                            v.IdCategoria,
                            EstadoTexto = v.Estado == 'A' ? "Activo" : "Inactivo"
                        })
                        .ToList();

                    gvVideojuegos.DataSource = lista;
                    gvVideojuegos.DataBind();
                }
            }
            catch (Exception exc)
            {
                MostrarMensaje("Error al cargar videojuegos: " + exc.Message, true);
            }
        }

        protected void btnCrear_Click(object sender, EventArgs e)
        {
            // Abre el formulario en modo creación, porque no se envía id por la URL.
            Response.Redirect("~/Pages/FormularioVideojuego.aspx", true);
        }

        private void MostrarMensaje(string mensaje, bool esError)
        {
            /*
                Muestra mensajes en pantalla.
                Se usa una clase CSS diferente dependiendo de si es error o mensaje correcto.
            */

            lblMensaje.Text = mensaje;
            lblMensaje.Visible = true;
            lblMensaje.CssClass = esError ? "mensaje-error" : "mensaje-correcto";
        }
    }
}