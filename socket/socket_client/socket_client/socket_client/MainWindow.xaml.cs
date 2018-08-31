using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace socket_client
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        Socket soc = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        byte[] buff = new byte[60000];

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (btnConnect.Content.ToString() == "连接")
                {
                    soc.Connect(IPAddress.Parse(txtServer.Text), 8081);
                    soc.BeginReceive(buff, 0, buff.Length, SocketFlags.None, new AsyncCallback(ReceiveCB), soc);
                    ShowMsg("连接成功");
                    btnConnect.Content = "断开";
                }
                else
                {
                    soc.Disconnect(true);
                    ShowMsg("断开成功");
                    btnConnect.Content = "连接";
                }
            }
            catch (Exception ex)
            {
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
            private void ShowMsg(string v)
            {
            lsMsg.Items.Add(v);
            }

            private void ReceiveCB(IAsyncResult ar)
            {
                Socket soc = (Socket)ar.AsyncState;
                Thread.Sleep(100);
                Dispatcher.Invoke(new Action(() =>
                    {
                        string readData_str = Encoding.Default.GetString(buff);
                        ShowMsg("发言人：客户端" + "时间：" + DateTime.Now.ToString() + "IP" + soc.RemoteEndPoint.ToString() + "内容：" + readData_str.Replace("\0", ""));
                    }));
                buff = new byte[60000];
                try
                {
                    soc.BeginReceive(buff, 0, buff.Length, SocketFlags.None, new AsyncCallback(ReceiveCB), soc);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                    soc.Disconnect(true);
                    ShowMsg("断开成功");
                    Dispatcher.Invoke(new Action(() =>
                    {
                        btnConnect.Content = "连接";
                    }));
                }
            }

            private void btnSend_Click(object sender, RoutedEventArgs e)
            {
                string sendData_str = txtMsg.Text;
                ShowMsg("发送数据：" + sendData_str);
                byte[] sendByte = Encoding.Default.GetBytes(sendData_str);
                soc.Send(sendByte);
            }
        }
    }
