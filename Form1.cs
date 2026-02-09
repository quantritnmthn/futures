using System;

using System.ComponentModel;

using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Net;

using System.Runtime.InteropServices;
using System.Diagnostics;


using Newtonsoft.Json.Linq;


namespace binance
{



    // 52.192.96.6
    public partial class Form1 : Form
    {
        DataGridViewCellStyle stylered = new DataGridViewCellStyle();
        DataGridViewCellStyle stylegreen = new DataGridViewCellStyle();
        DataGridViewCellStyle style = new DataGridViewCellStyle();
        string content;

        long T1;
        long T2;

        string endpoint = "";
        string gourl = "";
        double tongchenh = 0;



        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);
        private const int WM_SETREDRAW = 11;

        Form fv = new Formve();
        Int32 screenWidth = 0;
        Int32 screenHeight = 0;

        public Form1()
        {
            InitializeComponent();


        }



        private void Form1_Load(object sender, EventArgs e)
        {

            stylered.ForeColor = Color.Red; // the color change
            stylegreen.ForeColor = Color.Green; // the color change
            LoaiSoSanh.SelectedIndex = 0;
            market.SelectedIndex = 0;

            if (!System.Windows.Forms.SystemInformation.TerminalServerSession)
            {
                Type dgvType = dataGridView1.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(gridmoi, true, null);
            }


            // Vẽ form dưới đáy màn hình

            fv.FormBorderStyle = FormBorderStyle.None;
            fv.TopMost = true;
            fv.BackColor = Color.Purple;
            fv.StartPosition = FormStartPosition.Manual;
            Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
            screenWidth = screenBounds.Width;
            screenHeight = screenBounds.Height;
            fv.Top = screenHeight - 3;
            fv.Width = screenWidth;
            fv.ShowInTaskbar = false;
            fv.Show();
            ////////////

            loadfirst();
            loadfund();


        }







        protected void loadfund()
        {
            try
            {


                content = file_get_contents("https://www.binance.com/fapi/v1/premiumIndex");
                dynamic stuff = JsonConvert.DeserializeObject(content);
                string cap = "";


                double fund = 0;
                double laisuat = 0;




                gridcu.Rows.Clear();

                var pdata = stuff;
                foreach (var detail in pdata)
                {

                    cap = detail["symbol"];
                    fund = Convert.ToDouble(detail["lastFundingRate"]);

                    laisuat = System.Math.Round(Convert.ToDouble(detail["lastFundingRate"]) * 100, 3);




                    string[] row1 = new string[] { cap, System.Math.Round(Convert.ToDouble(detail["indexPrice"]), 3).ToString(), laisuat.ToString() + " %" };



                    gridfund.Rows.Add(row1);



                }


            }
            catch { }


        }


        protected void loadfirst()
        {
            try
            {
                long T1 = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

                if (market.SelectedIndex == 0) endpoint = "https://fapi.binance.com/fapi/v1/ticker/24hr"; else endpoint = "https://www.binance.com/api/v3/ticker/24hr";



                content = file_get_contents(endpoint);




                dynamic stuff = JsonConvert.DeserializeObject(content);
                string cap = "";



                string thaydoi = "";

                double vonhoa = 0;
                double khoiluong = 0;

                double giamo = 0;
                double giahientai = 0;
                double ptthaydoi = 0;
                double biendong = 0;


                gridcu.Rows.Clear();
                changerange.Rows.Clear();

                var pdata = stuff;
                
                foreach (var detail in pdata)
                {
                    ComboboxItem item = new ComboboxItem();
                    cap = detail["symbol"];
                    giamo = Convert.ToDouble(detail["openPrice"]);
                    giahientai = Convert.ToDouble(detail["lastPrice"]);
                    ptthaydoi = Convert.ToDouble(detail["priceChangePercent"]);
                
                    khoiluong = Convert.ToDouble(detail["quoteVolume"]);
                    vonhoa = Convert.ToDouble(detail["volume"]); ;
                    biendong = (Convert.ToDouble(detail["highPrice"]) - Convert.ToDouble(detail["lowPrice"])) / Convert.ToDouble(detail["lastPrice"]) * 100;
                    biendong = System.Math.Round(biendong, 3);
                    string[] row1 = new string[] { cap, giahientai.ToString(), ptthaydoi.ToString(), Convert.ToDouble(detail["highPrice"]).ToString(), Convert.ToDouble(detail["lowPrice"]).ToString(), vonhoa.ToString() + " M", khoiluong.ToString("N0"), "0", DateTime.Now.ToString(), "0", "0", "0", "0" };
                                                //  0 ,            1         ,          2           ,                      3                         ,                       4                        ,               5         ,             6           ,  7 ,          8             ,  9  , 10, 11 , 12                                                                                                             
                    string[] row2 = new string[] { cap, biendong.ToString() + " %" };



                    if (cap.Contains("USDT") && (khoiluong > (Convert.ToDouble(TongTien.Text) * 1000000)))
                    {
                        gridcu.Rows.Add(row1);
                        changerange.Rows.Add(row2);

                    }


                }


            }
            catch { this.Text = "Error since: " + DateTime.Now.ToString(); }


        }

        protected string file_get_contents(string fileName)
        {
            WebClient wc = new WebClient();
            string somestring = "";
            wc.Proxy = GlobalProxySelection.GetEmptyWebProxy();
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.2; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36");
            try
            {
                somestring = wc.DownloadString(fileName);
            }
            catch { somestring = ""; }

            return somestring;

        }

        protected void downloadasync(string fileName)
        {
            T1 = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;


            try
            {
                WebClient wc = new WebClient();
                wc.Proxy = GlobalProxySelection.GetEmptyWebProxy();
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.2; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36");
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadStringCompleted);
                wc.DownloadStringAsync(new Uri(fileName));
            }
            catch { }
        }


        protected void downloadvol(string fileName)
        {
            T1 = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;


            try
            {
                WebClient wc = new WebClient();
                wc.Proxy = GlobalProxySelection.GetEmptyWebProxy();
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
                wc.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.2; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36");
                wc.DownloadStringCompleted += new DownloadStringCompletedEventHandler(DownloadVolCompleted);
                wc.DownloadStringAsync(new Uri(fileName));
                wc.BaseAddress = fileName;
            }
            catch { }
        }


        protected void DownloadVolCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            dynamic stufft = JsonConvert.DeserializeObject(e.Result);
            double maker = 0;
            double taker = 0;


            try
            {

                foreach (var detail in stufft)
                {
                    if (detail["isBuyerMaker"] == true)
                        maker += Convert.ToDouble(detail["qty"]);
                    else
                        taker += Convert.ToDouble(detail["qty"]);
                }



                string[] row1 = new string[] { sender.GetType().GetProperty("BaseAddress").GetValue(sender, null).ToString().Replace("https://www.binance.com/fapi/v1/trades?limit=1000&symbol=", "").Replace("https://www.binance.com/api/v3/trades?limit=1000&symbol=", ""), (maker / taker).ToString("N3") };
                gridvol.Rows.Add(row1);
            }
            catch { }
        }




        public int dongcu(string capcu)
        {
            int dong = -1;

            for (int i = 0; i < gridcu.RowCount; i++)
            {
                if (gridcu.Rows[i].Cells[0].Value.ToString().Trim() == capcu)
                    dong = i;
            }

            return dong;
        }


        public decimal CalculateColumnSum(DataGridView dataGridView, int columnIndex)
        {
            try
            {
                // Kiểm tra nếu DataGridView rỗng hoặc columnIndex không hợp lệ
                if (dataGridView == null || dataGridView.Rows.Count == 0 ||
                    columnIndex < 0 || columnIndex >= dataGridView.Columns.Count)
                {
                    return 0;
                }

                decimal sum = 0;

                // Duyệt qua tất cả các dòng
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    // Bỏ qua dòng mới (nếu có)
                    if (!row.IsNewRow)
                    {
                        // Lấy giá trị ô trong cột được chỉ định
                        if (row.Cells[columnIndex].Value != null)
                        {
                            // Chuyển đổi giá trị sang decimal và cộng vào tổng
                            if (decimal.TryParse(row.Cells[columnIndex].Value.ToString(), out decimal value))
                            {
                                sum += value;
                            }
                        }
                    }
                }

                return sum;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        protected void DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {

                dynamic stuff = JsonConvert.DeserializeObject(e.Result);


                string cap = "";

                string thaydoi = "";

                double vonhoa = 0;
                double khoiluong = 0;

                int bull = 0;
                int iddong = -1;
                double giamo = 0;
                double giahientai = 0;
                double ptthaydoi = 0;
                double ptchenhlech = 0;
                double ptchenhlechthap = 0;
                double ptchenhlechgia = 0;
                double ptchenhlechkl = 0;
                var pdata = stuff;
                int i = 0;
                int idx = 0;




                SendMessage(gridmoi.Handle, WM_SETREDRAW, false, 0);
                gridmoi.Rows.Clear();


                tongchenh = 0;
                foreach (var detail in pdata)
                {
                    if (i < gridcu.Rows.Count)
                    {

                        cap = detail["symbol"];
                        iddong = dongcu(cap.ToUpper().Trim());
                        
                        if (iddong > -1)
                        {
                            idx++;
                            giahientai = Convert.ToDouble(detail["lastPrice"]);
                            ptthaydoi = Convert.ToDouble(detail["priceChangePercent"]);

                            khoiluong = Convert.ToDouble(detail["quoteVolume"]);
                            vonhoa = Convert.ToDouble(detail["volume"]); ;



                            ptchenhlechkl = (khoiluong / Convert.ToDouble(gridcu.Rows[iddong].Cells[6].Value) * 100) - 100;
                            ptchenhlechkl = System.Math.Round(System.Math.Abs(ptchenhlechkl), 3);

                            ptchenhlech = (giahientai / Convert.ToDouble(detail["highPrice"].ToString()) * 100) - 100;
                            ptchenhlech = System.Math.Round(System.Math.Abs(ptchenhlech), 3);

                            ptchenhlechthap = (giahientai / Convert.ToDouble(detail["lowPrice"].ToString()) * 100) - 100;
                            ptchenhlechthap = System.Math.Round(System.Math.Abs(ptchenhlechthap), 3);

                            ptchenhlechgia = (giahientai / Convert.ToDouble(gridcu.Rows[iddong].Cells[1].Value) * 100) - 100;
                            ptchenhlechgia = System.Math.Round(ptchenhlechgia, 3);


                            tongchenh += ptchenhlechgia;

                            bull = Convert.ToInt32(gridcu.Rows[iddong].Cells[11].Value);
                            if (giahientai > Convert.ToDouble(gridcu.Rows[iddong].Cells[1].Value)) bull++;
                            if (giahientai < Convert.ToDouble(gridcu.Rows[iddong].Cells[1].Value)) bull--;


                            string[] row1 = new string[] { cap, giahientai.ToString(), ptthaydoi.ToString(), Convert.ToDouble(detail["highPrice"]).ToString(), Convert.ToDouble(detail["lowPrice"]).ToString(), vonhoa.ToString("N0"), khoiluong.ToString("N0"), ptchenhlech.ToString(), DateTime.Now.ToString(), ptchenhlechgia.ToString(), ptchenhlechthap.ToString(), bull.ToString(), ptchenhlechkl.ToString() };

                            gridmoi.Rows.Add(row1);




                            if (gridmoi.Rows[i].Cells[2].Value.ToString() != "" && gridcu.Rows[iddong].Cells[2].Value.ToString() != "")
                            {
                                // màu chữ
                                if (Convert.ToDouble(gridmoi.Rows[i].Cells[7].Value) > Convert.ToDouble(gridcu.Rows[iddong].Cells[7].Value))
                                {
                                    gridmoi.Rows[i].Cells[7].Style = stylegreen;
                                }
                                if (Convert.ToDouble(gridmoi.Rows[i].Cells[7].Value) < Convert.ToDouble(gridcu.Rows[iddong].Cells[7].Value))
                                {
                                    gridmoi.Rows[i].Cells[7].Style = stylered;
                                }

                                if (Convert.ToDouble(gridmoi.Rows[i].Cells[1].Value) > Convert.ToDouble(gridcu.Rows[iddong].Cells[1].Value))
                                {
                                    gridmoi.Rows[i].Cells[1].Style = stylegreen;
                                }
                                if (Convert.ToDouble(gridmoi.Rows[i].Cells[1].Value) < Convert.ToDouble(gridcu.Rows[iddong].Cells[1].Value))
                                {
                                    gridmoi.Rows[i].Cells[1].Style = stylered;
                                }


                                if (Convert.ToDouble(gridmoi.Rows[i].Cells[2].Value) > 0)
                                {
                                    gridmoi.Rows[i].Cells[2].Style = stylegreen;
                                }
                                if (Convert.ToDouble(gridmoi.Rows[i].Cells[2].Value) < 0)
                                {
                                    gridmoi.Rows[i].Cells[2].Style = stylered;
                                }


                                if (Convert.ToDouble(gridmoi.Rows[i].Cells[10].Value) < Convert.ToDouble(gridcu.Rows[iddong].Cells[10].Value))
                                {
                                    gridmoi.Rows[i].Cells[10].Style = stylegreen;
                                }
                                if (Convert.ToDouble(gridmoi.Rows[i].Cells[10].Value) > Convert.ToDouble(gridcu.Rows[iddong].Cells[10].Value))
                                {
                                    gridmoi.Rows[i].Cells[10].Style = stylered;
                                }

                                if (Convert.ToDouble(gridmoi.Rows[i].Cells[12].Value) > 1)
                                {
                                    gridmoi.Rows[i].Cells[12].Style.ForeColor = Color.MediumPurple;
                                }
                                if (Convert.ToDouble(gridmoi.Rows[i].Cells[12].Value) > 3)
                                {
                                    gridmoi.Rows[i].Cells[12].Style.ForeColor = Color.DarkViolet;
                                }
                                if (Convert.ToDouble(gridmoi.Rows[i].Cells[12].Value) > 5)
                                {
                                    gridmoi.Rows[i].Cells[12].Style.ForeColor = Color.Indigo;
                                }
                                if (Convert.ToDouble(gridmoi.Rows[i].Cells[12].Value) > 10)
                                {
                                    gridmoi.Rows[i].Cells[12].Style.ForeColor = Color.Purple;
                                }

                                if (Convert.ToDouble(gridmoi.Rows[i].Cells[1].Value) > Convert.ToDouble(gridcu.Rows[iddong].Cells[1].Value))
                                {
                                    gridmoi.Rows[i].Cells[9].Style = stylegreen;
                                }
                                if (Convert.ToDouble(gridmoi.Rows[i].Cells[1].Value) < Convert.ToDouble(gridcu.Rows[iddong].Cells[1].Value))
                                {
                                    gridmoi.Rows[i].Cells[9].Style = stylered;
                                }

                                if (LoaiSoSanh.SelectedIndex == 1) // so sánh với giá cao nhất trong ngày
                                {
                                    if ((giahientai) > Convert.ToDouble(gridcu.Rows[iddong].Cells[3].Value))
                                    {
                                        idx = dataGridView1.Rows.Add(row1);
                                        dataGridView1.Rows[idx].Cells[7].Style = stylegreen;
                                        playsound();
                                        this.Text = cap + " : " + detail["symbol"].ToString();
                                        dataGridView1.Sort(dataGridView1.Columns[8], ListSortDirection.Descending);
                                    }
                                }

                                if (LoaiSoSanh.SelectedIndex == 2) // so sánh với giá thấp nhất trong ngày
                                {
                                    if ((giahientai) < Convert.ToDouble(gridcu.Rows[iddong].Cells[4].Value))
                                    {
                                        idx = dataGridView1.Rows.Add(row1);
                                        dataGridView1.Rows[idx].Cells[10].Style = stylered;
                                        playsound();
                                        this.Text = cap + " : " + detail["symbol"].ToString();
                                        dataGridView1.Sort(dataGridView1.Columns[8], ListSortDirection.Descending);
                                    }
                                }

                                if (LoaiSoSanh.SelectedIndex == 0) // so sánh giá phiên trước pump
                                {
                                    if (clear.Checked == true && ptchenhlechgia > Convert.ToDouble(phantram.Text) && (giahientai) > Convert.ToDouble(gridcu.Rows[iddong].Cells[1].Value))
                                    {
                                        idx = dataGridView1.Rows.Add(row1);
                                        dataGridView1.Rows[idx].Cells[9].Style = stylegreen;
                                        playsound();
                                        this.Text = cap + " : " + detail["symbol"].ToString();
                                        dataGridView1.Sort(dataGridView1.Columns[8], ListSortDirection.Descending);
                                    }
                                }

                                if (LoaiSoSanh.SelectedIndex == 3) // so sánh giá phiên trước dump
                                {
                                    if (clear.Checked == true && System.Math.Abs(ptchenhlechgia) > Convert.ToDouble(phantram.Text) && (giahientai) < Convert.ToDouble(gridcu.Rows[iddong].Cells[1].Value))
                                    {
                                        idx = dataGridView1.Rows.Add(row1);
                                        dataGridView1.Rows[idx].Cells[9].Style = stylered;
                                        playsound();
                                        this.Text = cap + " : " + detail["symbol"].ToString();
                                        dataGridView1.Sort(dataGridView1.Columns[8], ListSortDirection.Descending);
                                    }
                                }

                                if (LoaiSoSanh.SelectedIndex == 6) // so sánh khối lượng
                                {
                                    if (clear.Checked == true && ptchenhlechkl > Convert.ToDouble(phantram.Text) && (giahientai) > Convert.ToDouble(gridcu.Rows[iddong].Cells[1].Value))
                                    {
                                        idx = dataGridView1.Rows.Add(row1);
                                        dataGridView1.Rows[idx].Cells[9].Style = stylegreen;
                                        playsound();
                                        this.Text = cap + " : " + detail["symbol"].ToString();
                                        dataGridView1.Sort(dataGridView1.Columns[8], ListSortDirection.Descending);
                                    }
                                }


                            }



                            i++;
                        }



                    }


                } // foreach



                if (clear.Checked == true)
                {
                    copygrid();
                }

                //this.Text = idx.ToString();



                if (LoaiSoSanh.SelectedIndex == 0) // so sánh giá phiên trước (pumb)   
                    gridmoi.Sort(gridmoi.Columns[9], ListSortDirection.Descending);


                if (LoaiSoSanh.SelectedIndex == 3) // so sánh giá phiên trước (dump)   
                    gridmoi.Sort(gridmoi.Columns[9], ListSortDirection.Ascending);

                if (LoaiSoSanh.SelectedIndex == 1) // so sánh giá cao nhất trong ngày
                    gridmoi.Sort(gridmoi.Columns[7], ListSortDirection.Ascending);

                if (LoaiSoSanh.SelectedIndex == 2) // so sánh giá thấp nhất trong ngày
                    gridmoi.Sort(gridmoi.Columns[10], ListSortDirection.Ascending);

                if (LoaiSoSanh.SelectedIndex == 4) // Bull run
                {
                    copygrid();
                    gridmoi.Sort(gridmoi.Columns[11], ListSortDirection.Descending);
                }
                if (LoaiSoSanh.SelectedIndex == 5) // Bear run
                {
                    copygrid();
                    gridmoi.Sort(gridmoi.Columns[11], ListSortDirection.Ascending);
                }


                if (LoaiSoSanh.SelectedIndex == 6) // so sánh chênh khối lượng
                    gridmoi.Sort(gridmoi.Columns[12], ListSortDirection.Descending);

                SendMessage(gridmoi.Handle, WM_SETREDRAW, true, 0);
                gridmoi.Refresh();


                
                T2 = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                label4.Text = "Time: " + (T2 - T1).ToString() + " ms." + "Total change: "+ tongchenh.ToString();


                // vẽ đường line đáy màn hình 
                fv.TopMost = true;

                using (Graphics g = fv.CreateGraphics())
                {

                    g.FillRectangle(Brushes.Black, new Rectangle(0, 0, screenWidth, 3));
                    g.FillRectangle(Brushes.White, new Rectangle(screenWidth / 2, 0, 1, 3));
                    g.FillRectangle(Brushes.PaleTurquoise, new Rectangle((screenWidth / 2) + 69, 0, 1, 3));
                    g.FillRectangle(Brushes.Red, new Rectangle((screenWidth / 2) - 69, 0, 1, 3));
                    g.FillRectangle(Brushes.PaleTurquoise, new Rectangle((screenWidth / 2) + 36, 0, 1, 3));
                    g.FillRectangle(Brushes.Red, new Rectangle((screenWidth / 2) - 36, 0, 1, 3));
                    g.FillRectangle(Brushes.PaleTurquoise, new Rectangle((screenWidth / 2) + 108, 0, 1, 3));
                    g.FillRectangle(Brushes.Red, new Rectangle((screenWidth / 2) - 108, 0, 1, 3));
                    if (tongchenh > 0)
                    {

                        g.FillRectangle(Brushes.LimeGreen, new Rectangle(screenWidth / 2 + 1, 1, Convert.ToInt32(Math.Abs(Math.Round(tongchenh, 0))), 2));
                    }
                    else
                    {

                        g.FillRectangle(Brushes.Red, new Rectangle(screenWidth / 2 - Convert.ToInt32(Math.Abs(Math.Round(tongchenh, 0))), 1, Convert.ToInt32(Math.Abs(Math.Round(tongchenh, 0))), 2));

                    }

                }



            }
            catch { loadfirst(); }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {

                if (market.SelectedIndex == 0) endpoint = "https://fapi.binance.com/fapi/v1/ticker/24hr"; else endpoint = "https://www.binance.com/api/v3/ticker/24hr";
                downloadasync(endpoint);

            }


            catch { this.Text = "Error since: " + DateTime.Now.ToString(); }
        }

        public string Between(string STR, string FirstString, string LastString)
        {
            string FinalString;
            int Pos1 = STR.IndexOf(FirstString) + FirstString.Length;
            int Pos2 = STR.IndexOf(LastString);
            FinalString = STR.Substring(Pos1, Pos2 - Pos1);
            return FinalString;
        }

        protected void playsound()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            player.Stream = Properties.Resources.alert;
            player.Play();

        }


        public DataGridView CloneDataGrid(DataGridView mainDataGridView)
        {
            DataGridView cloneDataGridView = new DataGridView();

            if (cloneDataGridView.Columns.Count == 0)
            {
                foreach (DataGridViewColumn datagrid in mainDataGridView.Columns)
                {
                    cloneDataGridView.Columns.Add(datagrid.Clone() as DataGridViewColumn);
                }
            }

            DataGridViewRow dataRow = new DataGridViewRow();

            for (int i = 0; i < mainDataGridView.Rows.Count; i++)
            {
                dataRow = (DataGridViewRow)mainDataGridView.Rows[i].Clone();
                int Index = 0;
                foreach (DataGridViewCell cell in mainDataGridView.Rows[i].Cells)
                {
                    dataRow.Cells[Index].Value = cell.Value;
                    Index++;
                }
                cloneDataGridView.Rows.Add(dataRow);
            }
            cloneDataGridView.AllowUserToAddRows = false;
            cloneDataGridView.Refresh();


            return cloneDataGridView;
        }

        protected void copygrid()
        {
            gridcu = CloneDataGrid(gridmoi);

        }

        public class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            loadfirst();
            dataGridView1.Rows.Clear();
            timer1.Interval = Convert.ToInt32(thoigian.Text);
            timer1.Enabled = true;
            thoigian.ReadOnly = true;
            phantram.ReadOnly = true;
            LoaiSoSanh.Enabled = false;
            market.Enabled = false;
            clear.Enabled = false;
            TongTien.Enabled = false;
            label5.Text = "Since: " + DateTime.Now.ToString();

        }

        private void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if ((e.Column.Index == 1) || (e.Column.Index == 2) || (e.Column.Index == 3) || (e.Column.Index == 4) || (e.Column.Index == 5) || (e.Column.Index == 6) || (e.Column.Index == 7) || (e.Column.Index == 9) || (e.Column.Index == 10) || (e.Column.Index == 11) || (e.Column.Index == 12))
            {
                e.SortResult = double.Parse(e.CellValue1.ToString()).CompareTo(double.Parse(e.CellValue2.ToString()));
                e.Handled = true;//pass by the default sorting
            }
        }

        private void gridcu_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if ((e.Column.Index == 1) || (e.Column.Index == 2) || (e.Column.Index == 3) || (e.Column.Index == 4) || (e.Column.Index == 5) || (e.Column.Index == 6) || (e.Column.Index == 7) || (e.Column.Index == 9) || (e.Column.Index == 10) || (e.Column.Index == 11) || (e.Column.Index == 12))
            {
                e.SortResult = double.Parse(e.CellValue1.ToString().Replace(" M", "")).CompareTo(double.Parse(e.CellValue2.ToString().Replace(" M", "")));
                e.Handled = true;//pass by the default sorting
            }
        }

        private void gridmoi_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if ((e.Column.Index == 1) || (e.Column.Index == 2) || (e.Column.Index == 3) || (e.Column.Index == 4) || (e.Column.Index == 5) || (e.Column.Index == 6) || (e.Column.Index == 7) || (e.Column.Index == 9) || (e.Column.Index == 10) || (e.Column.Index == 11) || (e.Column.Index == 12))
            {
                e.SortResult = double.Parse(e.CellValue1.ToString().Replace(" M", "")).CompareTo(double.Parse(e.CellValue2.ToString().Replace(" M", "")));
                e.Handled = true;//pass by the default sorting
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

                if (market.SelectedIndex == 0) gourl = "https://www.binance.com/vn/futures/" + dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Replace(" / ", "_"); else gourl = "https://www.binance.com/vn/trade/" + dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString().Replace(" / ", "_") + "?layout=pro";

                Clipboard.SetText(gourl);
                Process.Start("chrome", gourl);


            }
            catch { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            timer1.Interval = Convert.ToInt32(thoigian.Text);
            timer1.Enabled = false;
            thoigian.ReadOnly = false;
            phantram.ReadOnly = false;
            LoaiSoSanh.Enabled = true;
            market.Enabled = true;
            clear.Enabled = true;
            TongTien.Enabled = true;
            label5.Text = "Since: ";
            label4.Text = "Time: ";
        }


        public long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }

        private void gridmoi_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (market.SelectedIndex == 0) gourl = "https://www.binance.com/vn/futures/" + gridmoi.Rows[e.RowIndex].Cells[0].Value.ToString().Replace(" / ", "_"); else gourl = "https://www.binance.com/vn/trade/" + gridmoi.Rows[e.RowIndex].Cells[0].Value.ToString().Replace(" / ", "_") + "?layout=pro";

                Clipboard.SetText(gourl);
                Process.Start("chrome", gourl);
            }
            catch { }
        }

        private void gridmoi_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                loadfirst();
                dataGridView1.Rows.Clear();
                label5.Text = "Since: " + DateTime.Now.ToString();
                this.Text = DateTime.Now.ToString();
            }
        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                dataGridView1.Rows.Clear();
                label5.Text = "Since: " + DateTime.Now.ToString();
                this.Text = DateTime.Now.ToString();
            }
        }

        private void gridfund_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 1 || e.Column.Index == 2)
            {
                e.SortResult = double.Parse(e.CellValue1.ToString().Replace(" %", "")).CompareTo(double.Parse(e.CellValue2.ToString().Replace(" %", "")));
                e.Handled = true;//pass by the default sorting
            }
        }

        private void gridfund_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                gridfund.Rows.Clear();
                loadfund();

            }
        }

        private void gridfund_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                Clipboard.SetText("https://www.binance.com/vn/futures/" + gridfund.Rows[e.RowIndex].Cells[0].Value.ToString().Replace(" / ", "_"));
                Process.Start("chrome", "https://www.binance.com/vn/futures/" + gridfund.Rows[e.RowIndex].Cells[0].Value.ToString().Replace(" / ", "_"));
            }
            catch { }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {


                tabControl1.SelectedTab = tabControl1.TabPages[4];

                if (market.SelectedIndex == 0) endpoint = "https://fapi.binance.com/fapi/v1/ticker/24hr"; else endpoint = "https://www.binance.com/api/v3/ticker/24hr";



                content = file_get_contents(endpoint);
                dynamic stuff = JsonConvert.DeserializeObject(content);
                string cap = "";


                gridvol.Rows.Clear();

                var pdata = stuff;
                foreach (var detail in pdata)
                {

                    cap = detail["symbol"];





                    if (cap.IndexOf("_") == -1 && cap.Contains("USDT") && !cap.Contains("UPUSDT") && !cap.Contains("DOWNUSDT"))
                    {

                        if (market.SelectedIndex == 0)
                            downloadvol("https://www.binance.com/fapi/v1/trades?limit=1000&symbol=" + cap);
                        else
                            downloadvol("https://www.binance.com/api/v3/trades?limit=1000&symbol=" + cap);


                    }






                }


            }
            catch { this.Text = "Error since: " + DateTime.Now.ToString(); }
        }

        private void gridvol_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (market.SelectedIndex == 0) gourl = "https://www.binance.com/vn/futures/" + gridvol.Rows[e.RowIndex].Cells[0].Value.ToString().Replace(" / ", "_"); else gourl = "https://www.binance.com/vn/trade/" + gridvol.Rows[e.RowIndex].Cells[0].Value.ToString().Replace(" / ", "_") + "?layout=pro";

                Clipboard.SetText(gourl);
                Process.Start("chrome", gourl);
            }
            catch { }

        }

        private void changerange_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 1)
            {
                e.SortResult = double.Parse(e.CellValue1.ToString().Replace(" %", "")).CompareTo(double.Parse(e.CellValue2.ToString().Replace(" %", "")));
                e.Handled = true;//pass by the default sorting
            }
        }

        private void changerange_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (market.SelectedIndex == 0) gourl = "https://www.binance.com/vn/futures/" + changerange.Rows[e.RowIndex].Cells[0].Value.ToString().Replace(" / ", "_"); else gourl = "https://www.binance.com/vn/trade/" + changerange.Rows[e.RowIndex].Cells[0].Value.ToString().Replace(" / ", "_") + "?layout=pro";

                Clipboard.SetText(gourl);
                Process.Start("chrome", gourl);
            }
            catch { }
        }

        private void gridmoi_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                gridmoi.Rows[e.RowIndex].Cells[0].Style.BackColor = Color.Tomato;
                if (e.ColumnIndex >= 0)
                {
                    gridmoi.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = gridmoi.Rows[e.RowIndex].Cells[0].Value.ToString();
                }
            }
        }

        private void gridmoi_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                gridmoi.Rows[e.RowIndex].Cells[0].Style.BackColor = Color.White;
        }
    }
}


