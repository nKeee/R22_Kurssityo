using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;


namespace R22_Kurssityo
{
    public partial class Form1 : Form
    {
       
        //MySqlConnection con;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'dataSet1.toimintaalue' table. You can move, or remove it, as needed.
            this.toimintaalueTableAdapter.Fill(this.dataSet1.toimintaalue);
            //string connectionString = @"Dsn = village newbies; uid = root";
            //con = new MySqlConnection(connectionString);
            //con.Open();

            this.asiakasTableAdapter.Fill(this.dataSet1.asiakas);
            this.postiTableAdapter.Fill(this.dataSet1.posti);
            this.palveluTableAdapter.Fill(this.dataSet1.palvelu);
            this.varauksen_palvelutTableAdapter.Fill(this.dataSet1.varauksen_palvelut);
            this.varausTableAdapter.Fill(this.dataSet1.varaus);
            this.mokkiTableAdapter.Fill(this.dataSet1.mokki);
        }

        private void btn_tallenna_varaus_Click(object sender, EventArgs e)
        {
            Validate();
            asiakasBindingSource.EndEdit();
            postiBindingSource.EndEdit();
            postiTableAdapter.Update(this.dataSet1);
            postiTableAdapter.Insert(tbPostinro.Text, tbPostitoimip.Text); 
            //Lähetetään eka postitableen tiedot, koska asiakastablessa käytetään foreign keynä postinro.
            //Ilman tätä järjestystä tulee erroria.
            asiakasTableAdapter.Update(this.dataSet1);
            asiakasTableAdapter.Insert(tbPostinro.Text, tbEnimi.Text, tbSnimi.Text, tbOsoite.Text, tbSposti.Text, tbPuhnro.Text);
            this.asiakasTableAdapter.Fill(this.dataSet1.asiakas);
            //Ja nyt varaus sisään
            varausBindingSource.EndEdit();
            varausTableAdapter.Update(this.dataSet1);
            //varausTableAdapter.Insert(dgvUusivaraus_asiakas.)
            // label33.Text = DateTime.Now.ToString("dd/M/yyyy");
        }

        private void btnHaeVaraus_Click(object sender, EventArgs e)
        {
            //Laskuun asiakkaan tietojen kirjoitus
            string haku = textVarausNumero.Text;
            string query = "SELECT * FROM varaus WHERE varaus_id="+haku+ " INNER JOIN asiakas ON varaus.asiakas_id=asiakas.asiakas_id";
            SaveFileDialog saveLasku = new SaveFileDialog();
            saveLasku.ShowDialog();
            if (saveLasku.FileName != "") 
            {
                StreamWriter sw = new StreamWriter(saveLasku.FileName);
                sw.WriteLine(query);
                sw.Flush();
                sw.Close();
            }
            //var cmd = new MySqlCommand(query, con);
            //MessageBox.Show(cmd.ExecuteReader().Read().ToString());
        }

        private void btn_uusimokki_Click(object sender, EventArgs e)
        {
            Validate();
            mokkiBindingSource.EndEdit();
            postiBindingSource.EndEdit();
            postiTableAdapter.Update(this.dataSet1);
            postiTableAdapter.Insert(tbMokkipostinro.Text, tbMokkipostitoimipaik.Text);
            //Lähetetään eka postitableen tiedot, koska mökkitablessa käytetään foreign keynä postinro.
            //Ilman tätä järjestystä tulee erroria.
            mokkiTableAdapter.Update(this.dataSet1);
            mokkiTableAdapter.Insert((long)cbToimalue.SelectedValue, tbMokkipostinro.Text, tbMokkinimi.Text, tbMokkiosoite.Text, tbMokkikuvaus.Text, Convert.ToInt32(tbMokkihenkimaara.Text), tbMokkivarustelu.Text);
            this.mokkiTableAdapter.Fill(this.dataSet1.mokki);
        }

        private void btnUusitoimalue_Click(object sender, EventArgs e)
        {
            Validate();
            toimintaalueBindingSource.EndEdit();
            toimintaalueTableAdapter.Update(this.dataSet1);
            toimintaalueTableAdapter.Insert(tbToimintaalue.Text);
            this.toimintaalueTableAdapter.Fill(this.dataSet1.toimintaalue);
        }

        private void btnPoistaTA_Click(object sender, EventArgs e)
        {
            dataGridView4.Rows.RemoveAt(dataGridView4.CurrentRow.Index);
            this.toimintaalueTableAdapter.Update(this.dataSet1.toimintaalue);
        }

        private void btnLisaapalvelu_Click(object sender, EventArgs e)
        {
            Validate();
            palveluBindingSource.EndEdit();
            palveluTableAdapter.Update(this.dataSet1);
            //palveluTableAdapter.Insert();
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void tbMokkihenkimaara_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnVahvistaVaraus_Click(object sender, EventArgs e)
        {
            
           
            dgVaraukset.Rows[dgVaraukset.CurrentRow].Cells[4].Value = DateTime.Now;

        }

        private void btnPoistaVaraus_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in this.dgVaraukset.SelectedRows)
            {
                dgVaraukset.Rows.RemoveAt(item.Index);
            }
            this.varausTableAdapter.Update(this.dataSet1.varaus);
        }
    }
}
