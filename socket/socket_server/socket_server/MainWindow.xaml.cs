using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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

namespace socket_server
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
         TcpListener tcp;
        bool isClosing;
        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(btnConnect.Content.ToString()=="连接")
                {
                    IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(txtsetvice.Text), 8888);
                    tcp = new TcpListener(ipe);

                    tcp.Start();
                    tcp.BeginAcceptSocket(new AsyncCallback(AccptSocketCB), tcp);
                    isClosing = false;
                    ShowMsg("开始监听:" + ipe.Address.ToString() + ":" + ipe.Port.ToString());
                    btnConnect.Content = "断开";
                }
            
                else
                {
                    isClosing = true;
                    tcp.Stop();
                    ShowMsg("关闭监听成功");
                    btnConnect.Content = "连接";
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "错误", MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
        Socket Soc = null;
        private void AccptSocketCB(IAsyncResult ar)
        {
           if(!isClosing)
            {
                TcpListener tcp = (TcpListener)ar.AsyncState;
                 Soc =(Socket)tcp.EndAcceptSocket(ar);
                ShowMsg("用户连接");
                Soc.BeginReceive(buff,0,buff.Length,SocketFlags.None,new AsyncCallback(Receive),
                    tcp.BeginAcceptSocket(new AsyncCallback(AccptSocketCB), tcp));
               

            }
        }

        private void ShowMsg(string v)
        {
            lsMsg.Items.Add(v);
        }

        static byte[] buff = new byte[60000];
        private void Receive(IAsyncResult ar)
        {
            if(!isClosing)
            {
                Socket soc = (Socket)ar.AsyncState;
                int i = 0;
                try
                {
                    i = soc.EndReceive(ar);
                    ShowMsg("用户退出");
                }
                catch
                {
                    if(i==0)
                    {
                        ShowMsg("用户退出");
                    }
                    else
                    {
                        string readData_str = Encoding.Default.GetString(buff);
                        ShowMsg("发言人：客户端" + "时间：" + DateTime.Now.ToString() + "IP" + soc.RemoteEndPoint.ToString() + "内容" + readData_str.Replace("\0", ""));
                        buff = new byte[60000];
                        soc.BeginReceive(buff, 0, buff.Length, SocketFlags.None, new
                            AsyncCallback(Receive), soc);
                    }
                }
            }
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            if(Soc ==null)
            {
                MessageBox.Show("当前没有客户端连接");
                return;
            }
            byte[] buff_ = Encoding.Default.GetBytes(txtMsg.Text);
            Soc.Send(buff_);
        }
    }
}
