using iText.Kernel.Pdf;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iText.Layout;
using iText.Kernel.Geom;
using iText.Layout.Properties;

namespace AguaPotable
{
    public partial class Form1 : Form
    {
        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataAdapter adapter;
        private DataTable dataTable;



        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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
            if (searchTerm != "")
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
            else
            {
                MessageBox.Show("Por favor, ingrese un término de búsqueda.");
            }

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string searchTerm = textBox1.Text.Trim();
            if (searchTerm != "")
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
            else
            {
                // Si el cuadro de búsqueda está vacío, muestra todos los clientes
                MostrarTodosLosClientes();
            }
        }

      private void  CheckBox_Click(object sender, EventArgs e)
        
            {
    CheckBox checkBox = (CheckBox)sender;
    
    // Verificar si el CheckBox se ha marcado
    if (checkBox.Checked == true)
    {
        int mes = ObtenerMesDesdeCheckBox(checkBox); // Obtener el mes desde el CheckBox
        int anio = 2024; // Año correspondiente a los CheckBox

        // Obtener el ClienteID seleccionado en el DataGridView
        int clienteId = ObtenerClienteIdSeleccionado();

        if (clienteId != -1) // Verificar si se seleccionó un cliente válido
        {
            decimal monto = 50; // Obtener el monto del pago
            decimal mora = 0; // Obtener la mora por mes (debes implementar este método)

            if (monto > 0) // Verificar si se pudo obtener un monto válido
            {
                try
                {
                    // Guardar el pago en la base de datos
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
                MessageBox.Show("No se pudo obtener el monto del pago para el cliente seleccionado.");
                checkBox.Checked = false; // Desmarcar el CheckBox si no se pudo obtener el monto
            }
        }
        else
        {
            MessageBox.Show("Por favor, seleccione un cliente antes de marcar un pago.");
            checkBox.Checked = false; // Desmarcar el CheckBox si no se seleccionó un cliente
        }
    }
    else // Si el CheckBox se desmarca
    {
        // Eliminar el registro de pago correspondiente
        EliminarRegistroPago(checkBox);
    }
}

        private void EliminarRegistroPago(CheckBox checkBox)
        {
            int mes = ObtenerMesDesdeCheckBox(checkBox); // Obtener el mes desde el CheckBox
            int anio = 2024; // Año correspondiente a los CheckBox
            int clienteId = ObtenerClienteIdSeleccionado(); // Obtener el ClienteID seleccionado en el DataGridView

            // Verificar si se seleccionó un cliente válido
            
            
                try
                {
                    // Eliminar el registro de pago en la base de datos
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


       private void dataGridViewClientes_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
            {
                if (e.RowIndex >= 0)
                {
                    DataGridViewRow row = dataGridViewClientes.Rows[e.RowIndex];
                    int clienteId = Convert.ToInt32(row.Cells["ClienteID"].Value);
                    
                    try
                    {
                        DataTable pagosCliente = DBManager.ObtenerPagosCliente(clienteId);

                        // Desmarcar todos los CheckBoxes primero
                        DesmarcarCheckBoxes();

                        // Iterar sobre los pagos y marcar los CheckBoxes correspondientes
                        foreach (DataRow pago in pagosCliente.Rows)
                        {
                            int mes = Convert.ToInt32(pago["Mes"]);
                            int anio = Convert.ToInt32(pago["Anio"]);
                            if (anio == 2024 && mes >= 1 && mes <= 12) // Asegurar que el mes esté dentro del rango válido
                            {
                                CheckBox checkBox = ObtenerCheckBoxPorMes(mes);
                                if (checkBox != null)
                                {
                                    checkBox.Checked = true;

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al obtener los pagos del cliente: " + ex.Message);
                    }
                }
            }

        }
    
        
        // Método para desmarcar todos los CheckBoxes
        private void DesmarcarCheckBoxes()
        {
            foreach (Control control in this.Controls)
            {
                if (control is CheckBox checkBox)
                {
                    checkBox.Checked = false;
                }
            }
        }

        // Método para obtener el CheckBox correspondiente a un mes dado
        private CheckBox ObtenerCheckBoxPorMes(int mes)
        {
            string nombreCheckBox = "checkBox" + mes.ToString();
            return this.Controls[nombreCheckBox] as CheckBox;
        }


        // Manejar el clic en un CheckBox
        // Manejar el clic en un CheckBox
    

        private int ObtenerMesDesdeCheckBox(CheckBox checkBox)
        {
            string nombreCheckBox = checkBox.Name;
            string numeroMesString = nombreCheckBox.Substring("checkBox".Length);
            int numeroMes;
            if (int.TryParse(numeroMesString, out numeroMes))
            {
                return numeroMes;
            }
            else
            {
                throw new ArgumentException("El nombre del CheckBox no sigue el formato esperado.");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {


            if (dataGridViewClientes.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridViewClientes.SelectedRows[0];
                int clienteId = Convert.ToInt32(selectedRow.Cells["ClienteID"].Value);

                try
                {
                    DataTable pagosCliente = DBManager.ObtenerPagosClienteImprimir(clienteId);
                    string fileName = $"Boleta_Cliente_{clienteId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";

                    using (PdfWriter writer = new PdfWriter(fileName))
                    {
                        using (PdfDocument pdf = new PdfDocument(writer))
                        {
                            // Establecer el tamaño de página para 58mm de ancho
                            pdf.SetDefaultPageSize(new PageSize(58, 200));

                            Document document = new Document(pdf);
                            document.SetMargins(5, 5, 5, 5); // Márgenes estrechos para maximizar el espacio

                            // Crear un estilo de texto centrado y con tamaño adecuado
                            Text title = new Text("Boleta de Pagos").SetTextAlignment(TextAlignment.CENTER).SetFontSize(7);
                            Paragraph titleParagraph = new Paragraph(title);
                            document.Add(titleParagraph);

                            // Crear tabla con encabezados
                            Table table = new Table(5).UseAllAvailableWidth(); // 5 columnas y ancho completo
                            table.SetWidth(UnitValue.CreatePercentValue(100)); // Ancho de 100% de la página

                            // Encabezados de la tabla con tamaño de fuente reducido
                            table.AddHeaderCell(new Cell().Add(new Paragraph("Mes").SetFontSize(8)));
                            table.AddHeaderCell(new Cell().Add(new Paragraph("Año").SetFontSize(8)));
                            table.AddHeaderCell(new Cell().Add(new Paragraph("Monto").SetFontSize(8)));
                            table.AddHeaderCell(new Cell().Add(new Paragraph("Mora").SetFontSize(8)));
                            table.AddHeaderCell(new Cell().Add(new Paragraph("Fecha de Pago").SetFontSize(8)));

                            decimal totalMonto = 0;
                            decimal totalMora = 0;

                            // Agregar detalles de pagos a la tabla y calcular totales
                            foreach (DataRow pago in pagosCliente.Rows)
                            {
                                int mes = Convert.ToInt32(pago["Mes"]);
                                int anio = Convert.ToInt32(pago["Anio"]);
                                decimal monto = Convert.ToDecimal(pago["Monto"]);
                                decimal mora = Convert.ToDecimal(pago["Mora"]);
                                DateTime fechaPago = Convert.ToDateTime(pago["FechaPago"]);

                                // Agregar fila a la tabla con tamaño de fuente reducido
                                table.AddCell(new Cell().Add(new Paragraph(mes.ToString()).SetFontSize(8)));
                                table.AddCell(new Cell().Add(new Paragraph(anio.ToString()).SetFontSize(8)));
                                table.AddCell(new Cell().Add(new Paragraph(monto.ToString()).SetFontSize(8)));
                                table.AddCell(new Cell().Add(new Paragraph(mora.ToString()).SetFontSize(8)));
                                table.AddCell(new Cell().Add(new Paragraph(fechaPago.ToString("dd/MM/yyyy")).SetFontSize(8)));

                                // Calcular totales
                                totalMonto += monto;
                                totalMora += mora;
                            }

                            // Calcular total a pagar
                            decimal totalPagar = totalMonto - totalMora;

                            // Agregar fila de total a pagar con tamaño de fuente reducido
                            table.AddCell(new Cell(1, 4).Add(new Paragraph("Total a pagar").SetFontSize(8)));
                            table.AddCell(new Cell().Add(new Paragraph(totalPagar.ToString()).SetFontSize(8)));

                            // Agregar la tabla al documento
                            document.Add(table);
                        }
                    }

                    MessageBox.Show("Boleta generada exitosamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al generar la boleta: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un cliente para generar la boleta.");
            }

            /*
            if (dataGridViewClientes.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridViewClientes.SelectedRows[0];
                int clienteId = Convert.ToInt32(selectedRow.Cells["ClienteID"].Value);

                try
                {
                    DataTable pagosCliente = DBManager.ObtenerPagosClienteImprimir(clienteId);

                    string fileName = $"Boleta_Cliente_{clienteId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                    using (PdfWriter writer = new PdfWriter(fileName))
                    {
                        using (PdfDocument pdf = new PdfDocument(writer))
                        {
                           
                            PageSize pageSize = new PageSize(58f, PageSize.A4.GetHeight());
                            pdf.SetDefaultPageSize(pageSize);
                            Document document = new Document(pdf);

                            // Crear tabla con encabezados
                            Table table = new Table(5); // 5 columnas
                            table.AddHeaderCell("Mes");
                            table.AddHeaderCell("Año");
                            table.AddHeaderCell("Monto");
                            table.AddHeaderCell("Mora");
                            table.AddHeaderCell("Fecha de Pago");

                            decimal totalMonto = 0;
                            decimal totalMora = 0;

                            // Agregar detalles de pagos a la tabla y calcular totales
                            foreach (DataRow pago in pagosCliente.Rows)
                            {
                                int mes = Convert.ToInt32(pago["Mes"]);
                                int anio = Convert.ToInt32(pago["Anio"]);
                                decimal monto = Convert.ToDecimal(pago["Monto"]);
                                decimal mora = Convert.ToDecimal(pago["Mora"]);
                                DateTime fechaPago = Convert.ToDateTime(pago["FechaPago"]);

                                // Agregar fila a la tabla
                                table.AddCell(mes.ToString());
                                table.AddCell(anio.ToString());
                                table.AddCell(monto.ToString());
                                table.AddCell(mora.ToString());
                                table.AddCell(fechaPago.ToString("dd/MM/yyyy"));

                                // Calcular totales
                                totalMonto += monto;
                                totalMora += mora;
                            }

                            // Calcular total a pagar
                            decimal totalPagar = totalMonto - totalMora;

                            // Agregar fila de total a pagar
                            table.AddCell(new Cell(1, 4).Add(new Paragraph("Total a pagar")));
                            table.AddCell(totalPagar.ToString());

                            // Agregar la tabla al documento
                            document.Add(table);
                        }
                    }

                    MessageBox.Show("Boleta generada exitosamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al generar la boleta: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un cliente para generar la boleta.");
            }





            /*
            // Verificar si se seleccionó una fila en el DataGridView
            if (dataGridViewClientes.SelectedRows.Count > 0)
            {
                // Obtener el cliente seleccionado
                DataGridViewRow selectedRow = dataGridViewClientes.SelectedRows[0];
                int clienteId = Convert.ToInt32(selectedRow.Cells["ClienteID"].Value);

                try
                {
                    // Obtener los pagos del cliente desde la base de datos
                    DataTable pagosCliente = DBManager.ObtenerPagosClienteImprimir(clienteId);

                    // Crear un nuevo documento PDF
                    string fileName = $"Boleta_Cliente_{clienteId}_{DateTime.Now:yyyyMMddHHmmss}.pdf";
                    using (PdfWriter writer = new PdfWriter(fileName))
                    {
                        using (PdfDocument pdf = new PdfDocument(writer))
                        {
                            Document document = new Document(pdf);

                            // Agregar título y fecha al documento
                            Paragraph title = new Paragraph("Boleta de Resumen de Pagos");
                            title.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                            document.Add(title);
                            document.Add(new Paragraph($"Fecha: {DateTime.Now:dd/MM/yyyy}"));

                            Table table = new Table(5); // 5 columnas
                            table.AddHeaderCell("Mes");
                            table.AddHeaderCell("Año");
                            table.AddHeaderCell("Monto");
                            table.AddHeaderCell("Mora");
                            table.AddHeaderCell("Fecha de Pago");

                            // Agregar información de pagos al documento
                            foreach (DataRow pago in pagosCliente.Rows)
                            {
                                int mes = Convert.ToInt32(pago["Mes"]);
                                int anio = Convert.ToInt32(pago["Anio"]);
                                decimal monto = Convert.ToDecimal(pago["Monto"]);
                                decimal mora = Convert.ToDecimal(pago["Mora"]);
                                DateTime fechaPago = Convert.ToDateTime(pago["FechaPago"]);

                                // Agregar detalles de pago al documento
                                document.Add(new Paragraph($"Mes: {mes}, Año: {anio}, Monto: {monto}, Mora: {mora}, Fecha de Pago: {fechaPago}"));
                            }
                        }
                    }

                    MessageBox.Show("Boleta generada exitosamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al generar la boleta: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un cliente para generar la boleta.");
            }
            */
        }

    }
    
}
    
