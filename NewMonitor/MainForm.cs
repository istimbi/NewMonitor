using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NewMonitor
{
    public partial class MainForm : Form
    {
        double bottomline = 0.56;
        double topline = 0.7;
        double speed = 0.0;
        String[] m_PortNames = SerialPort.GetPortNames();                   // Имена портов

        public MainForm()
        {
            InitializeComponent();
            
            try
            {
                textBox1.Text = Convert.ToString(Properties.Settings.Default.bottom_line);
                textBox2.Text = Convert.ToString(Properties.Settings.Default.top_line);
                textBox3.Text = "170";
            }
            catch (Exception)
            {
                textBox1.Text = Convert.ToString(bottomline);
                textBox2.Text = Convert.ToString(topline);
                textBox3.Text = "170";
            }
           

            for (int i = 0; i <  m_PortNames.Length; i++)
            {
                comboBox1.Items.Add(m_PortNames[i]);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.bottom_line = Convert.ToDouble(textBox1.Text);
            Properties.Settings.Default.top_line = Convert.ToDouble(textBox2.Text);
            Properties.Settings.Default.Save();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {            
            
            try
            {
                Device.Open(comboBox1.SelectedItem.ToString());
                label3.Text = "Подключенно к COM порту";
                label3.ForeColor = Color.Green;
            }
            catch (Exception)
            {
                label3.Text = "Не удалось подключиться к порту";
                label3.ForeColor = Color.Red;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (checkBox.Checked)
            {
                Device.chek = true;
                Device.time = Convert.ToInt32(numeric.Value);
            }   
                    
            try
            {
                if (Device.Port.IsOpen)
                {
                    Device.L = Convert.ToDouble(textBox3.Text);
                    this.DialogResult = DialogResult.OK;
                    this.Hide();
                }
            }

            catch (Exception)
            {
                MessageBox.Show("Не выбран COM порт");
            }

        }
        
        public string Data
        {          
        get
            {                
                return Device.Port.PortName;
                               
            }
        }

    }
}
