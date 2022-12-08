using System.IO.Ports;
namespace SocialSkill2
{
    public partial class Form1 : Form
    {
        //Var declaration
        static int NumProd = 0, nowid = 0;
        string[] Prod = new string[100];
        string[] NameProd = new string[100];
        string[] ProdCode = new string[100];
        int[] ProdVal = new int[100];
        bool[] ProdCon = new bool[10];
        string[] ProdTime = new string[100];

        SerialPort serial = new SerialPort();

        public Form1()
        {
            InitializeComponent();
            InitializeValue();
            label7.Text = "";
            serial.Open();
        }

        private void UpdateVal(int d, string s)
        {
            switch (d)
            {
                case 0: nowid = Convert.ToInt32(s) - 1; return;
                case 1: ProdCode[nowid] = s; return;
                case 2: NameProd[nowid] = s; return;
                case 3: ProdVal[nowid] = Convert.ToInt32(s); return;
                default: ProdTime[nowid] = s; return;
            }
        }

        private void InitializeValue()
        {
            numericUpDown2.Maximum = 10;
            numericUpDown2.Minimum = 1;

            if (!File.Exists("NumOfProduct.txt"))
                File.WriteAllText("NumOfProduct.txt", "0");

            string s = File.ReadAllText("NumOfProduct.txt");
            NumProd = Convert.ToInt32(s);

            if (File.Exists("Product.txt"))
                Prod = File.ReadAllLines("Product.txt");

            for (int i = 0; i < 10; i++) ProdCon[i] = true;
            for (int i = 0; i < NumProd; i++)
            {
                s = "";
                int d = 0;
                for (int j = 0; j < Prod[i].Length; j++)
                {
                    if (Prod[i][j] != '_') s += Prod[i][j];
                    else
                    {
                        UpdateVal(d, s);
                        if (d == 0) ProdCon[nowid] = false;
                        d++;
                        s = "";
                    }
                }
                UpdateVal(d, s);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox3.Text == "")
            {
                MessageBox.Show("Hãy nhập tên sản phẩm hoặc mã sản phẩm!!!", "Notification");
                return;
            }

            for (int i = 0; i < 10; i++) 
                if (textBox3.Text == ProdCode[i])
                {
                    MessageBox.Show("Mã này đã có trước đó!!!", "Notification");
                    return;
                }

            nowid = Convert.ToInt32(numericUpDown2.Value) - 1;
            if (ProdCon[nowid] == false)
            {
                MessageBox.Show("Tủ này đã đầy!!!");
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có chắc muốn lưu sản phẩm này?", "Notification", MessageBoxButtons.YesNo);
            if (result == DialogResult.No) return;

            NumProd++;

            ProdCon[nowid] = false;
            ProdCode[nowid] = textBox3.Text;
            NameProd[nowid] = textBox1.Text;
            ProdVal[nowid] = Convert.ToInt32(numericUpDown1.Value);
            ProdTime[nowid] = dateTimePicker1.Value.ToString();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            serial.Close();
            NumProd = 0;
            for (int i = 0; i < 10; i++)
            {
                if (ProdVal[i] > 0)
                {
                    Prod[NumProd] = (i + 1).ToString() + "_" + ProdCode[i] + "_" + NameProd[i] +
                                "_" + ProdVal[i] + "_" + ProdTime[i];
                    NumProd++;
                }
            }
            File.WriteAllText("NumOfProduct.txt", NumProd.ToString());
            File.WriteAllLines("Product.txt", Prod);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label7.Text = "";
            string s = textBox2.Text;
            bool isNotFound = true;
            for (int i = 0; i < 10; i++)
            {
                if (s == ProdCode[i] && ProdVal[i] > 0)
                {
                    label7.Text = (i + 1).ToString() + " " + NameProd[i] + " " + ProdVal[i] + " " + ProdTime[i];
                    isNotFound = false;
                    serial.Write(i.ToString());
                    ProdVal[i]--;
                    break;
                }
            }
            if (isNotFound) label7.Text = "Đã hết thuốc hoặc không tìm thấy thuốc có mã trên";
        }
    }
}