<%@ Page Title="Iniciar sesión" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="RentaVideojuegos.Pages.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Iniciar sesión</h1>

    <asp:Label ID="lblMensajeError" runat="server" CssClass="mensaje-error" ForeColor="Red" Visible="false" />

    <div class="mb-3">
        <label for="txtEmail" class="form-label">Correo electrónico</label>
        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email"></asp:TextBox>
        <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
            ErrorMessage="El correo es requerido"
            ForeColor="Red"
            ControlToValidate="txtEmail">
        </asp:RequiredFieldValidator>
    </div>

    <div class="mb-3">
        <label for="txtClave" class="form-label">Contraseña</label>
        <asp:TextBox ID="txtClave" runat="server" CssClass="form-control" TextMode="Password"></asp:TextBox>
        <asp:RequiredFieldValidator ID="rfvClave" runat="server"
            ErrorMessage="La contraseña es requerida"
            ForeColor="Red"
            ControlToValidate="txtClave">
        </asp:RequiredFieldValidator>
    </div>

    <div class="mb-3">
        <asp:Button ID="btnIngresar" runat="server"
            Text="Ingresar"
            CssClass="btn btn-primary"
            OnClick="btnIngresar_Click" />
    </div>

</asp:Content>