<%@ Page Title="Formulario videojuego" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FormularioVideojuego.aspx.cs" Inherits="RentaVideojuegos.Pages.FormularioVideojuego" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h1><asp:Label ID="lblTituloPagina" runat="server" Text="Formulario videojuego"></asp:Label></h1>

    <asp:Label ID="lblMensaje" runat="server" Visible="false"></asp:Label>

    <table class="tabla-formulario">

        <tr>
            <td>Sucursal:</td>
            <td>
                <asp:DropDownList ID="ddlSucursal" runat="server" CssClass="form-control"></asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvSucursal" runat="server"
                    ControlToValidate="ddlSucursal"
                    InitialValue=""
                    ErrorMessage="Debe seleccionar una sucursal."
                    ForeColor="Red">
                </asp:RequiredFieldValidator>
            </td>
        </tr>

        <tr>
            <td>Título:</td>
            <td>
                <asp:TextBox ID="txtTitulo" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvTitulo" runat="server"
                    ControlToValidate="txtTitulo"
                    ErrorMessage="El título es requerido."
                    ForeColor="Red">
                </asp:RequiredFieldValidator>
            </td>
        </tr>

        <tr>
            <td>Descripción:</td>
            <td>
                <asp:TextBox ID="txtDescripcion" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" MaxLength="500"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvDescripcion" runat="server"
                    ControlToValidate="txtDescripcion"
                    ErrorMessage="La descripción es requerida."
                    ForeColor="Red">
                </asp:RequiredFieldValidator>
            </td>
        </tr>

        <tr>
            <td>Categoría:</td>
            <td>
                <asp:TextBox ID="txtCategoria" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvCategoria" runat="server"
                    ControlToValidate="txtCategoria"
                    ErrorMessage="La categoría es requerida."
                    ForeColor="Red">
                </asp:RequiredFieldValidator>
            </td>
        </tr>

        <tr>
            <td>Fecha lanzamiento:</td>
            <td>
                <asp:TextBox ID="txtFechaLanzamiento" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvFechaLanzamiento" runat="server"
                    ControlToValidate="txtFechaLanzamiento"
                    ErrorMessage="La fecha de lanzamiento es requerida."
                    ForeColor="Red">
                </asp:RequiredFieldValidator>
            </td>
        </tr>

        <tr>
            <td>Desarrolladora:</td>
            <td>
                <asp:TextBox ID="txtDesarrolladora" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvDesarrolladora" runat="server"
                    ControlToValidate="txtDesarrolladora"
                    ErrorMessage="La desarrolladora es requerida."
                    ForeColor="Red">
                </asp:RequiredFieldValidator>
            </td>
        </tr>

        <tr>
            <td>Distribuidora:</td>
            <td>
                <asp:TextBox ID="txtDistribuidora" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvDistribuidora" runat="server"
                    ControlToValidate="txtDistribuidora"
                    ErrorMessage="La distribuidora es requerida."
                    ForeColor="Red">
                </asp:RequiredFieldValidator>
            </td>
        </tr>

        <tr>
            <td>Imagen:</td>
            <td>
                <asp:TextBox ID="txtImagen" runat="server" CssClass="form-control" MaxLength="255"></asp:TextBox>
            </td>
        </tr>

        <tr>
            <td>Tráiler:</td>
            <td>
                <asp:TextBox ID="txtTrailer" runat="server" CssClass="form-control" MaxLength="255"></asp:TextBox>
            </td>
        </tr>

    </table>

    <p>
        <asp:Button ID="btnGuardar" runat="server"
            Text="Guardar"
            CssClass="btn btn-primary"
            OnClick="btnGuardar_Click" />

        <asp:Button ID="btnInactivar" runat="server"
            Text="Inactivar"
            CssClass="btn btn-danger"
            Visible="false"
            CausesValidation="false"
            OnClientClick="return confirm('¿Desea inactivar este videojuego?');"
            OnClick="btnInactivar_Click" />

        <asp:Button ID="btnRegresar" runat="server"
            Text="Regresar"
            CssClass="btn btn-secondary"
            CausesValidation="false"
            OnClick="btnRegresar_Click" />
    </p>

</asp:Content>