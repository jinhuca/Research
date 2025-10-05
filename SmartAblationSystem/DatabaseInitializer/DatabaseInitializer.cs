using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace DatabaseFiller
{
    /// <summary>
    /// This Form contains procedures that handles database initialization.
    /// IEC 62304 Class A.
    /// </summary>
    public partial class FrmDatabaseInitializer : Form
    {
        /// <summary>
        /// Default Constructor
        /// IEC 62304 Class A.
        /// </summary>
        public FrmDatabaseInitializer()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This procedure handles graphic element content at the form loading.
        /// IEC 62304 Class A.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmDatabaseInitializer_Load(object sender, EventArgs e)
        {
            LblConnectionStatus.Text = string.Empty;
            TxtConnectionString.Text = Properties.Settings.Default.ConnectionString;
        }

        /// <summary>
        /// This procedure opens a database connection when the button is clicked.
        /// IEC 62304 Class A.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnTestConnection_Click(object sender, EventArgs e)
        {
            SqlConnection sqlConnection = null;

            try
            {
                sqlConnection = new SqlConnection(TxtConnectionString.Text);
                sqlConnection.Open();
                LblConnectionStatus.Text = "Success!";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                LblConnectionStatus.Text = "Fail";
            }
            finally
            {
                sqlConnection?.Close();
            }
        }

        /// <summary>
        /// This procedure execute a script that fills database fields.
        /// IEC 62304 Class A.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnFillValues_Click(object sender, EventArgs e)
        {
            SqlConnection sqlConnection = null;
            string script = File.ReadAllText(@".\InitializationScript.sql");

            try
            {
                sqlConnection = new SqlConnection(TxtConnectionString.Text);
                SqlCommand cmd = new SqlCommand();

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = script;

                sqlConnection.Open();
                cmd.Connection = sqlConnection;

                cmd.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                TxtStatus.Text = exception.ToString();
            }
            finally
            {
                sqlConnection?.Close();
            }

            TxtStatus.Text = "Insertion completed!";
        }
    }
}