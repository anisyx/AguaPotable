using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace AguaPotable
{
    public partial class Form1 : Form
    {
        public static int clienteId;
        public Form1()
        {
            InitializeComponent();
            MostrarTodosLosClientes();
            button3.Enabled = false;
            textBox2.Enabled = false;
            label1.Enabled = false;
        }

        private void MostrarTodosLosClientes()
        {
            try
            {
                DataTable dataTable = DBManager.ObtenerTodosLosClientes();
                dataGridViewClientes.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar todos los clientes: " + ex.Message);
            }
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    string searchTerm = textBox1.Text.Trim();
        //    if (!string.IsNullOrEmpty(searchTerm))
        //    {
        //        BuscarCliente(searchTerm);
        //    }
        //    else
        //    {
        //        MessageBox.Show("Por favor, ingrese un término de búsqueda.");
        //    }
        //}

     
        private void BuscarCliente(string searchTerm)
        {
            try
            {
                DataTable dataTable = DBManager.BuscarCliente(searchTerm);
                dataGridViewClientes.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar el cliente: " + ex.Message);
            }
        }

        private void checkBox_Click(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            int mes = ObtenerMesDesdeCheckBox(checkBox);

            if (checkBox.Checked)
            {
                int anio = 2024; // Año correspondiente a los CheckBox
                int clienteId = ObtenerClienteIdSeleccionado();

                if (clienteId != -1)
                {
                    try
                    {
                        decimal monto = DBManager.ObtenerCuotaCliente(clienteId); // Obtener el monto del pago
                        decimal mora = Int64.Parse(textBox2.Text); // Obtener la mora por mes (debes implementar este método)
                        DBManager.GuardarPago(clienteId, mes, anio, monto, mora, DateTime.Today);
                        MessageBox.Show("Pago registrado correctamente.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al guardar el pago: " + ex.Message);
                        checkBox.Checked = false; // Desmarcar el CheckBox en caso de error
                    }
                }
                else
                {
                    MessageBox.Show("Por favor, seleccione un cliente antes de marcar un pago.");
                    checkBox.Checked = false; // Desmarcar el CheckBox si no se seleccionó un cliente
                }
            }
            else
            {
                EliminarRegistroPago(checkBox);
            }
        }

        private void EliminarRegistroPago(CheckBox checkBox)
        {
            int mes = ObtenerMesDesdeCheckBox(checkBox); // Obtener el mes desde el CheckBox
            int anio = 2024; // Año correspondiente a los CheckBox
            clienteId = ObtenerClienteIdSeleccionado(); // Obtener el ClienteID seleccionado en el DataGridView

            try
            {
                DBManager.EliminarPago(clienteId, mes, anio);
                MessageBox.Show("Registro de pago eliminado correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el registro de pago: " + ex.Message);
                checkBox.Checked = true; // Volver a marcar el CheckBox en caso de error
            }
        }

        private int ObtenerClienteIdSeleccionado()
        {
            if (dataGridViewClientes.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridViewClientes.SelectedRows[0];
                return Convert.ToInt32(selectedRow.Cells["ClienteID"].Value);
            }
            else
            {
                return -1; // Retorna -1 si no hay ninguna fila seleccionada
            }
        }

        private int ObtenerMesDesdeCheckBox(CheckBox checkBox)
        {
            string nombreCheckBox = checkBox.Name;
            string numeroMesString = nombreCheckBox.Substring("checkBox".Length);
            return int.Parse(numeroMesString);
        }

        private void dataGridViewClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridViewClientes.Rows[e.RowIndex];
                clienteId = Convert.ToInt32(row.Cells["ClienteID"].Value);

                try
                {
                    DataTable pagosCliente = DBManager.ObtenerPagosCliente(clienteId);

                    DesmarcarCheckBoxes();

                    foreach (DataRow pago in pagosCliente.Rows)
                    {
                        int mes = Convert.ToInt32(pago["Mes"]);
                        CheckBox checkBox = ObtenerCheckBoxPorMes(mes);
                        if (checkBox != null)
                        {
                            checkBox.Checked = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al obtener los pagos del cliente: " + ex.Message);
                }
            }
        }

        private void DesmarcarCheckBoxes()
        {
            foreach (Control control in Controls)
            {
                if (control is CheckBox checkBox)
                {
                    checkBox.Checked = false;
                }
            }
        }

        private CheckBox ObtenerCheckBoxPorMes(int mes)
        {
            string nombreCheckBox = "checkBox" + mes.ToString();
            return Controls[nombreCheckBox] as CheckBox;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridViewClientes.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridViewClientes.SelectedRows[0];
                clienteId = Convert.ToInt32(selectedRow.Cells["ClienteID"].Value);

                using (Boleta boleta = new Boleta())
                {
                    boleta.ShowDialog();
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un cliente para generar la boleta.");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            int anioActual = DateTime.Now.Year;
            lblanio.Text = anioActual.ToString();
        }

       

        private void button3_Click(object sender, EventArgs e)
        {
            int anio = 2024; // Año correspondiente a los CheckBox
            int clienteId = ObtenerClienteIdSeleccionado();

            if (clienteId != -1)
            {
                try
                {
                    decimal monto = DBManager.ObtenerCuotaCliente(clienteId); // Obtener el monto del pago
                    decimal mora = Int64.Parse(textBox2.Text); // Obtener la mora por mes (debes implementar este método)
                    DBManager.GuardarPago(clienteId, 0, anio, 0, mora, DateTime.Today);
                    MessageBox.Show("Mora registrada correctamente.");
                    button3.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al guardar la mora: " + ex.Message);
                    button3.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un cliente antes de marcar la mora.");
                button3.Enabled = true;
            }
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = (CheckBox)sender;
            if (checkBox13.Checked == true)
            {

                button3.Enabled = true;
                textBox2.Enabled = true; 
                label1.Enabled = true;
            }
            else
            {
                button3.Enabled = false;
                textBox2.Enabled = false;
                label1.Enabled = false;

            }
        }

    

        private void button4_Click_1(object sender, EventArgs e)
        {
            Usuario formularioUsuario = new Usuario();
            formularioUsuario.ShowDialog();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            DataTable dtClientes = DBManager.ObtenerClientes();
            DataView dvClientes = dtClientes.DefaultView;
            dvClientes.RowFilter = string.Format("Nombre LIKE '%{0}%' OR Direccion LIKE '%{0}%' OR Canton LIKE '%{0}%'", textBox3.Text);
            dataGridViewClientes.DataSource = dvClientes;

            string searchTerm = textBox3.Text.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                MostrarTodosLosClientes();
            }
            else
            {
                BuscarCliente(searchTerm);
            }
        }
    }
}

