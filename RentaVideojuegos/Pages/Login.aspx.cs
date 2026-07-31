using DataModels;
using System;
using System.Linq;

namespace RentaVideojuegos.Pages
{
    public partial class Login : System.Web.UI.Page
    {
        /*
            Esta página se encarga del inicio de sesión del sistema.
            Valida el correo y la contraseña contra la base de datos.
            Si las credenciales son correctas, guarda los datos principales
            del usuario en Session y lo redirige según su rol.
        */

        protected void Page_Load(object sender, EventArgs e)
        {
            /*
                Si el usuario ya inició sesión y vuelve al login,
                se le redirige automáticamente a la pantalla que le corresponde.
            */
            if (!IsPostBack && Session["IdJugador"] != null)
            {
                RedirigirSegunRol();
            }
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            /*
                Este evento se ejecuta cuando el usuario presiona el botón Ingresar.
                Primero se validan los campos requeridos y luego se consulta la base de datos.
            */

            if (!Page.IsValid)
                return;

            string email = txtEmail.Text.Trim();
            string clave = txtClave.Text.Trim();

            try
            {
                using (RentaVideojuegosDB db = new RentaVideojuegosDB("DbRentaVideojuegos"))
                {
                    /*
                        Se llama al procedimiento almacenado que valida las credenciales.
                        Este procedimiento revisa que el jugador exista, que la clave coincida
                        y que el estado del jugador sea activo.
                    */
                    var resultado = db.SpValidarCredenciales(email, clave).FirstOrDefault();

                    if (resultado == null)
                    {
                        MostrarError("Las credenciales no son correctas.");
                        return;
                    }

                    /*
                        Se guardan en Session los datos necesarios para mantener
                        identificado al usuario durante el uso del sistema.
                    */
                    Session["IdJugador"] = resultado.IdJugador;
                    Session["NombreCompleto"] = resultado.NombreCompleto;
                    Session["EsAdministrador"] = resultado.EsAdministrador;
                }

                RedirigirSegunRol();
            }
            catch (Exception exc)
            {
                MostrarError("Ocurrió un error al iniciar sesión: " + exc.Message);
            }
        }

        private void RedirigirSegunRol()
        {
            /*
                Después del login, el administrador entra a la gestión general
                y el jugador normal entra directamente a sus propios alquileres.
            */

            bool esAdministrador = Convert.ToBoolean(Session["EsAdministrador"]);

            if (esAdministrador)
                Response.Redirect("~/Pages/GestionarAlquileres.aspx", true);
            else
                Response.Redirect("~/Pages/MisAlquileres.aspx", true);
        }

        private void MostrarError(string mensaje)
        {
            // Muestra en pantalla el mensaje cuando el inicio de sesión falla.
            lblMensajeError.Text = mensaje;
            lblMensajeError.Visible = true;
        }
    }
}