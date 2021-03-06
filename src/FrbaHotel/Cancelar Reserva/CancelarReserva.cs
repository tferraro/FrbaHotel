﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DOM;
using DOM.DAO;
using DOM.Dominio;
using DOM.Auxiliares;

namespace FrbaHotel.Cancelar_Reserva
{
    public partial class CancelarReserva : Form
    {
        public CancelarReserva()
        {
            InitializeComponent();
        }

        private void CancelarReserva_Load(object sender, EventArgs e)
        {
            limpiar();
        }

        private void botonLimpiar_Click(object sender, EventArgs e)
        {
            limpiar();
        }
        
        public void setearReserva(int nroReserva)
        {
            textNroReserva.Text = nroReserva.ToString();
        }

        private void limpiar()
        {
            textNroReserva.Text = "";
            textUsuario.Text = Globals.infoSesion.User.Usr;
            dateTimeCancelacion.Value = Globals.getFechaSistema();
            while(comboMotivos.Items.Count > 0)
                comboMotivos.Items.RemoveAt(0);
            comboMotivos.Items.AddRange(Globals.motivosBaja);
            if(Globals.infoSesion.Rol.Nombre == "GUEST")
            {
                comboMotivos.Items.RemoveAt(2);  
                comboMotivos.Items.RemoveAt(0);  
            }
            else
                comboMotivos.Items.RemoveAt(1);
            comboMotivos.SelectedIndex = 0;
        }

        private void botonCancelarReserva_Click(object sender, EventArgs e)
        {
            if (chequearDatos())
            {
                Reserva reserva = DAOReserva.obtenerReservaCancelable(Int32.Parse(textNroReserva.Text));
                if (reserva == null)
                {
                    showToolTip("Ingrese un número de reserva válido.", textNroReserva, textNroReserva.Location);
                    return;
                }
                CancelacionReserva cancelacion = new CancelacionReserva();
                //Actualizemos los datos
                cancelacion.Codigo_Reserva = reserva.CodigoReserva;
                cancelacion.Fecha_Cancelacion_struct = dateTimeCancelacion.Value;
                cancelacion.Motivo = comboMotivos.SelectedItem.ToString();
                cancelacion.Usr = Globals.infoSesion.User.Usr;
                reserva.Estado = estadoCancelacion(cancelacion.Motivo);
                cancelacion.Estado = reserva.Estado;

                //Actualizamos el estado de la reserva
                if (!DAOReserva.agregarCancelacion(cancelacion))
                {
                    MessageBox.Show("Error al cancelar la reserva. Intente nuevamente.", "Error", MessageBoxButtons.OK); 
                    return;
                }
                if (!DAOReserva.actualizar(reserva))
                {
                    MessageBox.Show("Error al cancelar la reserva. Intente nuevamente.", "Error", MessageBoxButtons.OK);
                    return;
                }
                //Creamos la cancelacion
                MessageBox.Show("Se cancelo la reserva " + " correctamente.", "", MessageBoxButtons.OK);
                this.Close();
            }
        }

        private int estadoCancelacion(string motivo)
        {
            List<EstadoReservas> estados = DAOReserva.obtenerEstadosReservas();
            for(int i = 0; i < estados.Count; i++)
                if (estados[i].descripcion.ToUpper() == motivo.ToUpper())
                    return i + 1;
            return -1;
        }

        private bool chequearDatos()
        {
            if (textNroReserva.Text == "")
            {
                showToolTip("Ingrese un número de reserva.", textNroReserva, textNroReserva.Location);
                return false;
            }
            if (comboMotivos.SelectedIndex == -1)
            {
                showToolTip("Ingrese un motivo de cancelacion.", comboMotivos, comboMotivos.Location);
                return false;
            }
            return true;
        }

        private void textNroReserva_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            showToolTip("Ingrese un solo números.", textNroReserva, textNroReserva.Location);
        }

        private void showToolTip(string msj, Control ventana, Point pos)
        {
            toolTip.Hide(ventana);
            toolTip.SetToolTip(ventana, "Entrada Invalida");
            toolTip.Show(msj, ventana, 50, 10, 5000);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            // Confirm user wants to close
            Globals.habilitarAnterior();
        }

    }
}
