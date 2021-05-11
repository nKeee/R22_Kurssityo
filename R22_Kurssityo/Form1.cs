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
            //string connectionString = @"Dsn = village newbies; uid = root";
            //con = new MySqlConnection(connectionString);
            //con.Open();
            this.toimintaalueTableAdapter.Fill(this.dataSet1.toimintaalue);
            this.asiakasTableAdapter.Fill(this.dataSet1.asiakas);
            this.postiTableAdapter.Fill(this.dataSet1.posti);
            this.palveluTableAdapter.Fill(this.dataSet1.palvelu);
            this.varauksen_palvelutTableAdapter.Fill(this.dataSet1.varauksen_palvelut);
            this.varausTableAdapter.Fill(this.dataSet1.varaus);
            this.mokkiTableAdapter.Fill(this.dataSet1.mokki);
            this.laskuTableAdapter1.Fill(this.dataSet1.lasku);
        }

        private void btn_tallenna_varaus_Click(object sender, EventArgs e)
        {
            Validate();
            asiakasBindingSource.EndEdit();
            postiBindingSource.EndEdit();
            postiTableAdapter.Update(this.dataSet1);

            bool loytyi = false;
            foreach (DataRow dr in dataSet1.posti.Rows) //Tarkistetaan, löytyykö kirjotettu postinumero jo SQL:stä
            {
                string postinumero = dr["postinro"].ToString();
                if (postinumero == tbPostinro.Text)
                    loytyi = true;
            }
            if(loytyi) //Jos löytyi, niin ei luoda uutta postinumeroa posti tableen
            {
                asiakasTableAdapter.Update(this.dataSet1);
                asiakasTableAdapter.Insert(tbPostinro.Text, tbEnimi.Text, tbSnimi.Text, tbOsoite.Text, tbSposti.Text, tbPuhnro.Text);
                this.asiakasTableAdapter.Fill(this.dataSet1.asiakas);
                varausBindingSource.EndEdit();
                varausTableAdapter.Update(this.dataSet1);
                DateTime tyhja = new DateTime(1111, 11, 11);
                long? asiakasnumero = asiakasTableAdapter.ScalarQuery();
                varausTableAdapter.Insert((long)asiakasnumero, (long)cbMokki_varaus.SelectedValue, DateTime.Now.Date, tyhja, dateTimePicker1.Value, dateTimePicker2.Value);
            }

            else //Jos ei löytynyt, niin luodaan uusi postinro
            {
                postiTableAdapter.Insert(tbPostinro.Text, tbPostitoimip.Text);
                //Lähetetään eka postitableen tiedot, koska asiakastablessa käytetään foreign keynä postinro.
                //Ilman tätä järjestystä tulee erroria.
                asiakasTableAdapter.Update(this.dataSet1);
                asiakasTableAdapter.Insert(tbPostinro.Text, tbEnimi.Text, tbSnimi.Text, tbOsoite.Text, tbSposti.Text, tbPuhnro.Text);
                this.asiakasTableAdapter.Fill(this.dataSet1.asiakas);
                //Ja nyt varaus sisään
                varausBindingSource.EndEdit();
                varausTableAdapter.Update(this.dataSet1);
                DateTime tyhja = new DateTime(1111, 11, 11);
                long? asiakasnumero = asiakasTableAdapter.ScalarQuery();
                varausTableAdapter.Insert((long)asiakasnumero, (long)cbMokki_varaus.SelectedValue, DateTime.Now.Date, tyhja, dateTimePicker1.Value, dateTimePicker2.Value);
            }
            tbEnimi.Text = "";
            tbSnimi.Text = "";
            tbOsoite.Text = "";
            tbSposti.Text = "";
            tbPuhnro.Text = "";
            tbPostinro.Text = "";
            tbPostitoimip.Text = "";
        }


        private void btn_uusimokki_Click(object sender, EventArgs e)
        {
            Validate();
            mokkiBindingSource.EndEdit();
            postiBindingSource.EndEdit();
            postiTableAdapter.Update(this.dataSet1);

            bool loytyi = false;
            foreach (DataRow dr in dataSet1.posti.Rows) //Tarkistetaan, löytyykö kirjotettu postinumero jo SQL:stä
            {
                string postinumero = dr["postinro"].ToString();
                if (postinumero == tbMokkipostinro.Text)
                    loytyi = true;
            }
            if(loytyi)
            {
                mokkiTableAdapter.Update(this.dataSet1);
                mokkiTableAdapter.Insert((long)cbToimalue.SelectedValue, tbMokkipostinro.Text, tbMokkinimi.Text, tbMokkiosoite.Text, tbMokkikuvaus.Text, Convert.ToInt32(tbMokkihenkimaara.Text), tbMokkivarustelu.Text);
                this.mokkiTableAdapter.Fill(this.dataSet1.mokki);
            }
            else
            {
                postiTableAdapter.Insert(tbMokkipostinro.Text, tbMokkipostitoimipaik.Text);
                //Lähetetään eka postitableen tiedot, koska mökkitablessa käytetään foreign keynä postinro.
                //Ilman tätä järjestystä tulee erroria.
                mokkiTableAdapter.Update(this.dataSet1);
                mokkiTableAdapter.Insert((long)cbToimalue.SelectedValue, tbMokkipostinro.Text, tbMokkinimi.Text, tbMokkiosoite.Text, tbMokkikuvaus.Text, Convert.ToInt32(tbMokkihenkimaara.Text), tbMokkivarustelu.Text);
                this.mokkiTableAdapter.Fill(this.dataSet1.mokki);
            }
            tbMokkihenkimaara.Text = "";
            tbMokkikuvaus.Text = "";
            tbMokkinimi.Text = "";
            tbMokkiosoite.Text = "";
            tbMokkipostinro.Text = "";
            tbMokkipostitoimipaik.Text = "";
            tbMokkivarustelu.Text = "";
        }

        private void btnUusitoimalue_Click(object sender, EventArgs e)
        {
            Validate();
            toimintaalueBindingSource.EndEdit();
            toimintaalueTableAdapter.Update(this.dataSet1);
            toimintaalueTableAdapter.Insert(tbToimintaalue.Text);
            this.toimintaalueTableAdapter.Fill(this.dataSet1.toimintaalue);
            tbToimintaalue.Text = "";
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
            Validate();
            varausBindingSource.EndEdit();
            varausTableAdapter.Update(this.dataSet1);
            dgVaraukset.CurrentRow.Cells[4].Value = DateTime.Now.ToString("dd/M/yyyy");
            this.varausTableAdapter.Update(this.dataSet1.varaus);

        }

        private void btnPoistaVaraus_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow item in this.dgVaraukset.SelectedRows)
            {
                dgVaraukset.Rows.RemoveAt(item.Index);
            }
            this.varausTableAdapter.Update(this.dataSet1.varaus);
        }

        private void tabControl1_Click(object sender, EventArgs e)
        {
            Validate();
            laskuBindingSource.EndEdit();
            laskuTableAdapter1.Update(this.dataSet1.lasku);
            DateTime aloitus = dateTimePicker1.Value;
            DateTime lopetus = dateTimePicker2.Value;
            TimeSpan tp = aloitus - lopetus;
            int erotusPaivina = tp.Days;
            double verotonHinta = erotusPaivina * 100; //muutetaan tarvittaessa verolliseksi hinnaksi
            laskuTableAdapter1.Insert(cbLasku.SelectedIndex, cbVaraus.SelectedIndex, verotonHinta, 24);

        }

        private void btnPoistaLasku_Click(object sender, EventArgs e)
        {
            Validate();
            foreach (DataGridViewRow laskuRow in dgwLasku.SelectedCells)
            {
                if (laskuRow.Selected)
                    dgwLasku.Rows.RemoveAt(laskuRow.Index);
            }
            this.laskuTableAdapter1.Update(this.dataSet1.lasku);

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
    }
}
