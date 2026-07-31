<%@ Page Title="Modificar alquiler" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ModificarAlquiler.aspx.cs" Inherits="RentaVideojuegos.Pages.ModificarAlquiler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Modificar alquiler</h1>

    <asp:Label ID="lblMensaje" runat="server" Visible="false"></asp:Label>

    <table class="tabla-detalle">
        <tr>
            <td># Alquiler:</td>
            <td><asp:Label ID="lblIdAlquiler" runat="server" /></td>
        </tr>

        <tr>
            <td>Jugador:</td>
            <td><asp:Label ID="lblJugador" runat="server" /></td>
        </tr>

        <tr>
            <td>Sucursal:</td>
            <td><asp:Label ID="lblSucursal" runat="server" /></td>
        </tr>

        <tr>
            <td>Videojuego:</td>
            <td><asp:Label ID="lblVideojuego" runat="server" /></td>
        </tr>
    </table>

    <table class="tabla-formulario">
        <tr>
            <td>Fecha de inicio:</td>
            <td>
                <asp:TextBox ID="txtFechaInicio" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvFechaInicio" runat="server"
                    ControlToValidate="txtFechaInicio"
                    ErrorMessage="La fecha de inicio es requerida."
                    ForeColor="Red">
                </asp:RequiredFieldValidator>
            </td>
        </tr>

        <tr>
            <td>Fecha de devolución:</td>
            <td>
                <asp:TextBox ID="txtFechaDevolucion" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvFechaDevolucion" runat="server"
                    ControlToValidate="txtFechaDevolucion"
                    ErrorMessage="La fecha de devolución es requerida."
                    ForeColor="Red">
                </asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>

    <p>
        <asp:Button ID="btnGuardar" runat="server"
            Text="Guardar cambios"
            CssClass="btn btn-primary"
            OnClick="btnGuardar_Click" />

        <asp:Button ID="btnRegresar" runat="server"
            Text="Regresar"
            CssClass="btn btn-secondary"
            CausesValidation="false"
            OnClick="btnRegresar_Click" />
    </p>

</asp:Content>