using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AguaPotable
{
    public partial class Usuario : Form
    {

        public Usuario()
        {
            InitializeComponent();
        }

        private void Usuario_Load(object sender, EventArgs e)
        {
            DataTable dtClientes = DBManager.ObtenerClientes();
            dataGridViewClientes.DataSource = dtClientes;



        }

        private void textBoxBuscar_TextChanged(object sender, EventArgs e)
        {
            DataTable dtClientes = DBManager.ObtenerClientes();
            DataView dvClientes = dtClientes.DefaultView;
            dvClientes.RowFilter = string.Format("Nombre LIKE '%{0}%' OR Direccion LIKE '%{0}%' OR Canton LIKE '%{0}%'", textBoxBuscar.Text);
            dataGridViewClientes.DataSource = dvClientes;
        }




        //private void buttonEditar_Click(object sender, EventArgs e)
        //{
        //    // Obtener el ClienteID del cliente seleccionado en el DataGridView
        //    int clienteID = Convert.ToInt32(dataGridViewClientes.SelectedRows[0].Cells["ClienteID"].Value);

        //    // Abrir un formulario o panel para editar los datos del cliente
        //    FormEditarCliente formEditarCliente = new FormEditarCliente(clienteID);
        //    if (formEditarCliente.ShowDialog() == DialogResult.OK)
        //    {
        //        // Actualizar el cliente en la base de datos
        //        DBManager.ActualizarCliente(clienteID, formEditarCliente.Nombre, formEditarCliente.Direccion, formEditarCliente.Canton, formEditarCliente.Telefono, formEditarCliente.Cuota);

        //        // Actualizar el DataGridView
        //        DataTable dtClientes = DBManager.ObtenerClientes();
        //        dataGridViewClientes.DataSource = dtClientes;
        //    }
        //}


        private void button1_Click(object sender, EventArgs e)
        {
            // Obtener los valores ingresados por el usuario
            string nombre = txtNombre.Text;
            string direccion = txtDireccion.Text;
            string canton = txtCanton.Text;
            string telefono = txtTelefono.Text;
            string cuotaTexto = txtCuota.Text;

            // Verificar que todos los campos estén llenos
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(direccion) ||
                string.IsNullOrWhiteSpace(canton) || string.IsNullOrWhiteSpace(telefono) ||
                string.IsNullOrWhiteSpace(cuotaTexto))
            {
                MessageBox.Show("Por favor, llene todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Verificar que el campo Cuota contenga un valor numérico
            if (!decimal.TryParse(cuotaTexto, out decimal cuota))
            {
                MessageBox.Show("El campo Cuota debe contener un valor numérico.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Insertar el nuevo cliente en la base de datos
            DBManager.InsertarCliente(nombre, direccion, canton, telefono, cuota);

            // Actualizar el DataGridView
            DataTable dtClientes = DBManager.ObtenerClientes();
            dataGridViewClientes.DataSource = dtClientes;

            // Limpiar los controles después de agregar el nuevo cliente
            txtNombre.Clear();
            txtDireccion.Clear();
            txtCanton.Clear();
            txtTelefono.Clear();
            txtCuota.Clear();

        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (dataGridViewClientes.SelectedRows.Count > 0)
            {
                // Obtener el ClienteID del cliente seleccionado en el DataGridView
                int clienteID = Convert.ToInt32(dataGridViewClientes.SelectedRows[0].Cells["ClienteID"].Value);

                // Confirmar la eliminación del cliente
                if (MessageBox.Show($"¿Está seguro de eliminar el cliente con ID {clienteID}?", "Confirmar eliminación", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    // Eliminar el cliente de la base de datos
                    DBManager.EliminarCliente(clienteID);

                    // Actualizar el DataGridView
                    DataTable dtClientes = DBManager.ObtenerClientes();
                    dataGridViewClientes.DataSource = dtClientes;
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un cliente para eliminar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        

        private void button3_Click(object sender, EventArgs e)
        {
             // Verificar si hay alguna fila seleccionada en el DataGridView
    if (dataGridViewClientes.SelectedRows.Count > 0)
    {
        // Obtener el ClienteID del cliente seleccionado en el DataGridView
        int clienteID = Convert.ToInt32(dataGridViewClientes.SelectedRows[0].Cells["ClienteID"].Value);

        // Obtener los valores ingresados por el usuario
        string nombre = txtNombre.Text;
        string direccion = txtDireccion.Text;
        string canton = txtCanton.Text;
        string telefono = txtTelefono.Text;
        string cuotaTexto = txtCuota.Text;

        // Verificar que todos los campos estén llenos
        if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(direccion) ||
            string.IsNullOrWhiteSpace(canton) || string.IsNullOrWhiteSpace(telefono) ||
            string.IsNullOrWhiteSpace(cuotaTexto))
        {
            MessageBox.Show("Por favor, llene todos los campos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Verificar que el campo Cuota contenga un valor numérico
        if (!decimal.TryParse(cuotaTexto, out decimal cuota))
        {
            MessageBox.Show("El campo Cuota debe contener un valor numérico.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        // Editar el cliente en la base de datos
        DBManager.EditarCliente(clienteID, nombre, direccion, canton, telefono, cuota);

        // Actualizar el DataGridView
        DataTable dtClientes = DBManager.ObtenerClientes();
        dataGridViewClientes.DataSource = dtClientes;

        // Limpiar los controles después de editar el cliente
        txtNombre.Clear();
        txtDireccion.Clear();
        txtCanton.Clear();
        txtTelefono.Clear();
        txtCuota.Clear();
    }
    else
    {
        MessageBox.Show("Por favor, seleccione un cliente para editar.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
        }

        private void dataGridViewClientes_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewClientes.SelectedRows.Count > 0)
            {
                // Obtener el ClienteID del cliente seleccionado
                int clienteID = Convert.ToInt32(dataGridViewClientes.SelectedRows[0].Cells["ClienteID"].Value);

                // Obtener los datos del cliente
                DataRow cliente = DBManager.ObtenerClientePorID(clienteID);



                // Llenar los campos del formulario con los datos del cliente
                txtNombre.Text = cliente["Nombre"].ToString();
                txtDireccion.Text = cliente["Direccion"].ToString();
                txtCanton.Text = cliente["Canton"].ToString();
                txtTelefono.Text = cliente["Telefono"].ToString();
                txtCuota.Text = cliente["Cuota"].ToString();
            }
        }
    }
}
