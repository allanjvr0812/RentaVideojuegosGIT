<%@ Page Title="Crear alquiler" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CrearAlquiler.aspx.cs" Inherits="RentaVideojuegos.Pages.CrearAlquiler" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Crear alquiler</h1>

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
            <td>Jugador:</td>
            <td>
                <asp:DropDownList ID="ddlJugador" runat="server" CssClass="form-control"></asp:DropDownList>

                <asp:RequiredFieldValidator ID="rfvJugador" runat="server"
                    ControlToValidate="ddlJugador"
                    InitialValue=""
                    ErrorMessage="Debe seleccionar un jugador."
                    ForeColor="Red">
                </asp:RequiredFieldValidator>
            </td>
        </tr>

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
            Text="Guardar alquiler"
            CssClass="btn btn-primary"
            OnClick="btnGuardar_Click" />

        <asp:Button ID="btnRegresar" runat="server"
            Text="Regresar"
            CssClass="btn btn-secondary"
            CausesValidation="false"
            OnClick="btnRegresar_Click" />
    </p>

</asp:Content>