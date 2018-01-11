using AL.AL_Service;
using AL.Implement;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
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

namespace AL
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public Main m;

        public MainWindow()
        {
            InitializeComponent();

            headimg.Source = new BitmapImage(new Uri("./Images/h2.bmp", UriKind.Relative));


            //给该窗口添加键盘事件
            this.KeyDown += MainWindow_KeyDown;
            username.Focus();
        }


        /// <summary>
        /// 当前窗口的键盘事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            //判断是否按了回车
            if (e.Key == Key.Enter)
            {
                //验证非空
                if (string.IsNullOrEmpty(username.Text))
                    username.Focus();
                if (string.IsNullOrEmpty(password.Password))
                    password.Focus();
                if (!string.IsNullOrEmpty(username.Text) && !string.IsNullOrEmpty(password.Password))
                {
                    LoginClick(sender, e);
                }
            }

            //throw new NotImplementedException();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginClick(object sender, RoutedEventArgs e)
        {
            Login();
        }

        /// <summary>
        /// 登录
        /// </summary>
        private async void Login()
        {
            Loading.Visibility = Visibility.Visible;
            DTO_User du = new DTO_User();

            //获取帐号
            du._Number = username.Text;

            //获取登录密码
            du._Pass = password.Password;

            if (!string.IsNullOrEmpty(du._Number) && !string.IsNullOrEmpty(du._Pass))
            {
                //获取本机ip
                du._IP = Dns.GetHostAddresses(Dns.GetHostName()).GetValue(1).ToString();

                //尝试登陆
                try
                {
                    //传递客户端实现服务器接口消息
                    InstanceContext ic = new InstanceContext(new ISC_Implement(this));

                    //建立通讯
                    AL_ServiceClient alsc = new AL_ServiceClient(ic);

                    //请求服务器，验证登录信息
                    DTO_Login dl = await alsc.LoginAsync(du);

                    //判断是否登录成功
                    if (dl.IsLogin)
                    {
                        //创建登录成功窗体对象
                        m = new Main(this, dl, alsc);
                        //显示
                        m.Show();
                    }
                    else
                    {
                        //new MyMessageBox("系统提示", dl.LoginMess).Show();
                        MessageBox.Show(dl.LoginMess);
                    }
                }
                catch (Exception)
                {


                }
            }
            else
            {
                //new MyMessageBox("系统提示", "用户名和密码不能为空").Show();
                MessageBox.Show("用户名和密码不能为空");
            }
            Loading.Visibility = System.Windows.Visibility.Collapsed;
        }





    }
}
