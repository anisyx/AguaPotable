using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace AguaPotable
{
    public partial class Boleta : Form
    {
        public Boleta()
        {
            InitializeComponent();
        }

        ReporteBoleta repo = new ReporteBoleta();

        [Obsolete]
        private void  Boleta_Load(object sender, EventArgs e)
        {
            try
            {
                DataTable pagosCliente = DBManager.ObtenerPagosClienteImprimir(Form1.clienteId);
                repo = new ReporteBoleta();
                repo.DataSource = pagosCliente;
                reportViewer1.Report = repo;
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar la boleta: " + ex.Message);
            }
        }
    }
}
