using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FClient
{
    public partial class Form1 : Form
    {
        // ========================================================
        // BIẾN KẾT NỐI
        // ========================================================

        // Client kết nối đến server
        private TcpClient client;

        // Stream gửi và nhận dữ liệu
        private NetworkStream ns;

        // Đọc dữ liệu text
        private StreamReader reader;

        // Gửi dữ liệu text
        private StreamWriter writer;

        // Kiểm tra đã kết nối hay chưa
        private bool isConnected = false;

        // ========================================================
        // CÁC CONTROL GIAO DIỆN
        // ========================================================

        private TextBox txtIP;
        private TextBox txtPort;
        private TextBox txtUser;
        private TextBox txtPass;
        private TextBox txtRemoteFile;
        private TextBox txtFolderName;
        private TextBox txtLog;

        private Button btnConnect;
        private Button btnDisconnect;
        private Button btnLogin;
        private Button btnUpload;
        private Button btnDownload;
        private Button btnCreateFolder;
        private Button btnList;

        private Label lblStatus;

        // ========================================================
        // HÀM KHỞI TẠO
        // ========================================================

        public Form1()
        {
            InitializeComponent();
            CreateUI();
        }

        // ========================================================
        // TẠO GIAO DIỆN
        // ========================================================

        private void CreateUI()
        {
            this.Text = "FTP CLIENT";
            this.Width = 720;
            this.Height = 600;

            // =====================================================
            // ROW 1: IP & PORT
            // =====================================================
            Label lblIP = new Label();
            lblIP.Text = "IP Server:";
            lblIP.AutoSize = true; // SỬA LỖI Ở ĐÂY: Tự động co giãn Label
            lblIP.Left = 20;
            lblIP.Top = 23;
            this.Controls.Add(lblIP);

            txtIP = new TextBox();
            txtIP.Text = "127.0.0.1";
            txtIP.Left = 100;
            txtIP.Top = 20;
            txtIP.Width = 120;
            this.Controls.Add(txtIP);

            Label lblPort = new Label();
            lblPort.Text = "Port:";
            lblPort.AutoSize = true; // SỬA LỖI Ở ĐÂY
            lblPort.Left = 240;
            lblPort.Top = 23;
            this.Controls.Add(lblPort);

            txtPort = new TextBox();
            txtPort.Text = "2121";
            txtPort.Left = 320;
            txtPort.Top = 20;
            txtPort.Width = 80;
            this.Controls.Add(txtPort);

            btnConnect = new Button();
            btnConnect.Text = "CONNECT";
            btnConnect.Left = 420;
            btnConnect.Top = 18;
            btnConnect.Width = 100;
            btnConnect.Click += BtnConnect_Click;
            this.Controls.Add(btnConnect);

            btnDisconnect = new Button();
            btnDisconnect.Text = "DISCONNECT";
            btnDisconnect.Left = 530;
            btnDisconnect.Top = 18;
            btnDisconnect.Width = 120;
            btnDisconnect.Enabled = false;
            btnDisconnect.Click += BtnDisconnect_Click;
            this.Controls.Add(btnDisconnect);

            // =====================================================
            // ROW 2: LOGIN
            // =====================================================
            Label lblUser = new Label();
            lblUser.Text = "Username:";
            lblUser.AutoSize = true; // SỬA LỖI Ở ĐÂY
            lblUser.Left = 20;
            lblUser.Top = 73;
            this.Controls.Add(lblUser);

            txtUser = new TextBox();
            txtUser.Text = "user1";
            txtUser.Left = 100;
            txtUser.Top = 70;
            txtUser.Width = 120;
            this.Controls.Add(txtUser);

            Label lblPass = new Label();
            lblPass.Text = "Password:";
            lblPass.AutoSize = true; // SỬA LỖI Ở ĐÂY
            lblPass.Left = 240;
            lblPass.Top = 73;
            this.Controls.Add(lblPass);

            txtPass = new TextBox();
            txtPass.Text = "123456";
            txtPass.Left = 320;
            txtPass.Top = 70;
            txtPass.Width = 120;
            txtPass.UseSystemPasswordChar = true;
            this.Controls.Add(txtPass);

            btnLogin = new Button();
            btnLogin.Text = "LOGIN";
            btnLogin.Left = 460;
            btnLogin.Top = 68;
            btnLogin.Width = 100;
            btnLogin.Enabled = false;
            btnLogin.Click += BtnLogin_Click;
            this.Controls.Add(btnLogin);

            // =====================================================
            // ROW 3: UPLOAD / DOWNLOAD
            // =====================================================
            btnUpload = new Button();
            btnUpload.Text = "UPLOAD";
            btnUpload.Left = 20;
            btnUpload.Top = 130;
            btnUpload.Width = 100;
            btnUpload.Enabled = false;
            btnUpload.Click += BtnUpload_Click;
            this.Controls.Add(btnUpload);

            Label lblFile = new Label();
            lblFile.Text = "Tên file:";
            lblFile.AutoSize = true; // SỬA LỖI Ở ĐÂY
            lblFile.Left = 140;
            lblFile.Top = 135;
            this.Controls.Add(lblFile);

            txtRemoteFile = new TextBox();
            txtRemoteFile.Left = 200;
            txtRemoteFile.Top = 132;
            txtRemoteFile.Width = 220;
            this.Controls.Add(txtRemoteFile);

            btnDownload = new Button();
            btnDownload.Text = "DOWNLOAD";
            btnDownload.Left = 440;
            btnDownload.Top = 130;
            btnDownload.Width = 120;
            btnDownload.Enabled = false;
            btnDownload.Click += BtnDownload_Click;
            this.Controls.Add(btnDownload);

            // =====================================================
            // ROW 4: CREATE FOLDER / LIST FILE
            // =====================================================
            Label lblFolder = new Label();
            lblFolder.Text = "Folder:";
            lblFolder.AutoSize = true; // SỬA LỖI Ở ĐÂY
            lblFolder.Left = 20;
            lblFolder.Top = 183;
            this.Controls.Add(lblFolder);

            txtFolderName = new TextBox();
            txtFolderName.Text = "NewFolder";
            txtFolderName.Left = 100;
            txtFolderName.Top = 180;
            txtFolderName.Width = 160;
            this.Controls.Add(txtFolderName);

            btnCreateFolder = new Button();
            btnCreateFolder.Text = "CREATE";
            btnCreateFolder.Left = 280;
            btnCreateFolder.Top = 178;
            btnCreateFolder.Width = 100;
            btnCreateFolder.Enabled = false;
            btnCreateFolder.Click += BtnCreateFolder_Click;
            this.Controls.Add(btnCreateFolder);

            btnList = new Button();
            btnList.Text = "LIST FILE";
            btnList.Left = 400;
            btnList.Top = 178;
            btnList.Width = 120;
            btnList.Enabled = false;
            btnList.Click += BtnList_Click;
            this.Controls.Add(btnList);

            // =====================================================
            // STATUS & LOG (Giữ nguyên)
            // =====================================================
            lblStatus = new Label();
            lblStatus.Text = "Chưa kết nối";
            lblStatus.AutoSize = true; // Thêm luôn cho Status
            lblStatus.Left = 20;
            lblStatus.Top = 230;
            this.Controls.Add(lblStatus);

            txtLog = new TextBox();
            txtLog.Multiline = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Left = 20;
            txtLog.Top = 260;
            txtLog.Width = 650;
            txtLog.Height = 250;
            txtLog.ReadOnly = true;
            this.Controls.Add(txtLog);

            Button btnClear = new Button();
            btnClear.Text = "CLEAR LOG";
            btnClear.Left = 550;
            btnClear.Top = 520;
            btnClear.Click += (s, e) => txtLog.Clear();
            this.Controls.Add(btnClear);
        }

        // ========================================================
        // CONNECT SERVER
        // ========================================================

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            string ip = txtIP.Text.Trim();

            int port;

            bool checkPort =
                int.TryParse(txtPort.Text.Trim(), out port);

            if (checkPort == false)
            {
                MessageBox.Show("Port không hợp lệ");
                return;
            }

            try
            {
                // Tạo client
                client = new TcpClient();

                // Kết nối đến server
                client.Connect(ip, port);

                // Lấy stream
                ns = client.GetStream();

                // Tạo reader và writer
                reader = new StreamReader(ns);

                writer = new StreamWriter(ns);
                writer.AutoFlush = true;

                // Đọc thông báo chào
                string banner = reader.ReadLine();

                Log("SERVER: " + banner);

                isConnected = true;

                lblStatus.Text =
                    "Đã kết nối đến " + ip + ":" + port;

                btnConnect.Enabled = false;
                btnDisconnect.Enabled = true;
                btnLogin.Enabled = true;

                Log("Kết nối thành công");
            }
            catch (Exception ex)
            {
                Log("Lỗi kết nối: " + ex.Message);

                MessageBox.Show(ex.Message);
            }
        }

        // ========================================================
        // DISCONNECT
        // ========================================================

        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            if (isConnected)
            {
                try
                {
                    // Gửi lệnh QUIT
                    writer.WriteLine("QUIT");

                    string response = reader.ReadLine();

                    Log("SERVER: " + response);
                }
                catch
                {
                }

                CloseConnection();
            }
        }

        // ========================================================
        // LOGIN
        // ========================================================

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUser.Text.Trim();

            string password = txtPass.Text.Trim();

            // Gửi USER
            SendCommand("USER " + username);

            // Gửi PASS
            SendCommand("PASS " + password);

            // Mở các nút chức năng
            btnUpload.Enabled = true;
            btnDownload.Enabled = true;
            btnCreateFolder.Enabled = true;
            btnList.Enabled = true;

            btnLogin.Enabled = false;
        }

        // ========================================================
        // UPLOAD FILE
        // ========================================================

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            string filePath = dialog.FileName;

            string fileName =
                Path.GetFileName(filePath);

            try
            {
                // Đọc file thành mảng byte
                byte[] data =
                    File.ReadAllBytes(filePath);

                long fileSize = data.Length;

                // Gửi lệnh STOR
                writer.WriteLine("STOR " + fileName);

                // Đọc phản hồi
                string response = reader.ReadLine();

                Log("SERVER: " + response);

                // Nếu server chưa sẵn sàng thì dừng
                if (!response.StartsWith("150"))
                {
                    return;
                }

                // Gửi dữ liệu file
                BinaryWriter bw =
                    new BinaryWriter(ns);

                bw.Write(fileSize);

                bw.Write(data);

                bw.Flush();

                // Đọc phản hồi hoàn tất
                response = reader.ReadLine();

                Log("SERVER: " + response);

                Log("Upload thành công: " + fileName);
            }
            catch (Exception ex)
            {
                Log("Lỗi upload: " + ex.Message);
            }
        }

        // ========================================================
        // DOWNLOAD FILE
        // ========================================================

        private void BtnDownload_Click(object sender, EventArgs e)
        {
            string fileName =
                txtRemoteFile.Text.Trim();

            if (fileName == "")
            {
                MessageBox.Show("Nhập tên file");
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog();

            dialog.FileName = fileName;

            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            try
            {
                // Gửi lệnh RETR
                writer.WriteLine("RETR " + fileName);

                // Đọc phản hồi
                string response = reader.ReadLine();

                Log("SERVER: " + response);

                // Nếu file không tồn tại
                if (!response.StartsWith("150"))
                {
                    return;
                }

                // Nhận dữ liệu file
                BinaryReader br =
                    new BinaryReader(ns);

                long fileSize = br.ReadInt64();

                byte[] data =
                    br.ReadBytes((int)fileSize);

                // Lưu file
                File.WriteAllBytes(dialog.FileName, data);

                Log("Download thành công");

                MessageBox.Show("Tải file thành công");
            }
            catch (Exception ex)
            {
                Log("Lỗi download: " + ex.Message);
            }
        }

        // ========================================================
        // CREATE FOLDER
        // ========================================================

        private void BtnCreateFolder_Click(object sender, EventArgs e)
        {
            string folderName =
                txtFolderName.Text.Trim();

            if (folderName == "")
            {
                return;
            }

            SendCommand("MKD " + folderName);
        }

        // ========================================================
        // LIST FILE
        // ========================================================

        private void BtnList_Click(object sender, EventArgs e)
        {
            try
            {
                // Gửi lệnh LIST
                writer.WriteLine("LIST");

                while (true)
                {
                    string response =
                        reader.ReadLine();

                    if (response == null)
                    {
                        break;
                    }

                    Log("SERVER: " + response);

                    // Kết thúc danh sách
                    if (response.StartsWith("226"))
                    {
                        break;
                    }

                    // Lỗi
                    if (response.StartsWith("550"))
                    {
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Lỗi LIST: " + ex.Message);
            }
        }

        // ========================================================
        // GỬI LỆNH
        // ========================================================

        private void SendCommand(string command)
        {
            try
            {
                // Hiển thị lệnh client gửi
                Log("CLIENT: " + command);

                // Gửi lệnh
                writer.WriteLine(command);

                // Đọc phản hồi server
                string response =
                    reader.ReadLine();

                Log("SERVER: " + response);
            }
            catch (Exception ex)
            {
                Log("Lỗi: " + ex.Message);
            }
        }

        // ========================================================
        // ĐÓNG KẾT NỐI
        // ========================================================

        private void CloseConnection()
        {
            isConnected = false;

            if (reader != null)
            {
                reader.Close();
            }

            if (writer != null)
            {
                writer.Close();
            }

            if (client != null)
            {
                client.Close();
            }

            lblStatus.Text = "Đã ngắt kết nối";

            btnConnect.Enabled = true;
            btnDisconnect.Enabled = false;
            btnLogin.Enabled = false;

            btnUpload.Enabled = false;
            btnDownload.Enabled = false;
            btnCreateFolder.Enabled = false;
            btnList.Enabled = false;

            Log("Đã đóng kết nối");
        }

        // ========================================================
        // GHI LOG
        // ========================================================

        private void Log(string message)
        {
            string text =
                "[" + DateTime.Now.ToString("HH:mm:ss") + "] "
                + message;

            txtLog.AppendText(text + Environment.NewLine);
        }
    }
}
