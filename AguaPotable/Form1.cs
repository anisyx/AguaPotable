using System;
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

        private void button1_Click(object sender, EventArgs e)
        {
            string searchTerm = textBox1.Text.Trim();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                BuscarCliente(searchTerm);
            }
            else
            {
                MessageBox.Show("Por favor, ingrese un término de búsqueda.");
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = textBox1.Text.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                MostrarTodosLosClientes();
            }
            else
            {
                BuscarCliente(searchTerm);
            }
        }

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
                        decimal monto = 50; // Obtener el monto del pago
                        decimal mora = 0; // Obtener la mora por mes (debes implementar este método)
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
    }
}
