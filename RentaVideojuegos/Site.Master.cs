using System;
using System.Web.UI;

namespace RentaVideojuegos
{
    public partial class SiteMaster : MasterPage
    {
        /*
            Esta clase controla la plantilla principal del sistema.
            Desde aquí se maneja el menú, el nombre del usuario
            y las opciones que se muestran según el rol.
        */

        protected void Page_Load(object sender, EventArgs e)
        {
            // Se configura el menú cada vez que carga una página.
            ConfigurarMenu();
        }

        private void ConfigurarMenu()
        {
            /*
                Cuando no hay sesión activa, se ocultan las opciones internas.
                Esto permite que el login use la misma Master Page sin mostrar el menú completo.
            */
            if (Session["IdJugador"] == null)
            {
                lnkMisAlquileres.Visible = false;
                lnkGestionarAlquileres.Visible = false;
                lnkGestionarVideojuegos.Visible = false;
                lnkCerrarSesion.Visible = false;
                lblNombreUsuario.Text = "";
                lblTituloSeccion.Text = "";
                return;
            }

            // Si el usuario ya inició sesión, se muestra su nombre en la barra superior.
            lblNombreUsuario.Text = "Bienvenido, " + Session["NombreCompleto"].ToString();

            lnkMisAlquileres.Visible = true;
            lnkCerrarSesion.Visible = true;

            bool esAdministrador = Convert.ToBoolean(Session["EsAdministrador"]);

            // Estas opciones solo se muestran cuando el usuario es administrador.
            lnkGestionarAlquileres.Visible = esAdministrador;
            lnkGestionarVideojuegos.Visible = esAdministrador;
        }

        public void EstablecerTituloSeccion(string titulo)
        {
            /*
                Permite cambiar el título superior desde las páginas internas.
                Se deja para reutilizarlo si se desea personalizar cada sección.
            */
            lblTituloSeccion.Text = titulo;
        }

        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            // Se limpia la sesión y se devuelve al usuario al login.
            Session.Clear();
            Session.Abandon();

            Response.Redirect("~/Pages/Login.aspx", true);
        }
    }
}