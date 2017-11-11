namespace SQLFork
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Microsoft.SqlServer.Management.BatchParser;
    using System.Data.SqlClient;

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // BatchParser documentation: https://technet.microsoft.com/en-us/library/microsoft.sqlserver.management.batchparser.batchparser.parse.aspx

            var startTime = DateTime.Now;
            var batches = BatchParser.Parse(textBox1.Text, new ParseOptions("GO"));

            Parallel.ForEach(batches, batch =>
            {
                ProcessBatch(batch.Content);
            });

            var stopTime = DateTime.Now;

            MessageBox.Show(
                string.Format("Started at {0}; ended at {1}; runtime of {2} seconds",
                startTime.ToString("yyyy-MM-dd HH:mm:ss"), stopTime.ToString("yyyy-MM-dd HH:mm:ss"),
                stopTime.Subtract(startTime).TotalSeconds
                ));
        }

        private void ProcessBatch(string content)
        {
            using (var conn = new SqlConnection(textBox2.Text))
            {
                conn.Open();

                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = content;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.Text;

                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }
        }
    }
}
