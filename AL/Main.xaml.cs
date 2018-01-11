using AL.AL_Service;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AL
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : MetroWindow
    {
        private Window loginwindow;//登陆窗口对象
        private Window chatwindow;//聊天窗口对象
        private AL_ServiceClient alsc;//通讯通道对象



        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="w">登陆窗口</param>
        /// <param name="_mess">登陆返回的个人信息及好友信息</param>
        /// <param name="_alsc">登陆用户的通讯通道</param>
        public Main(Window w, DTO_Login _mess, AL_ServiceClient _alsc)
        {
            //获取通讯通道
            alsc = _alsc;


            InitializeComponent();
            loginwindow = w;
            //隐藏登录窗口
            w.Hide();

            this.Closed += Main_Closed;

            //添加自己的信息
            LoadingMyMess(_mess.MyMess);



            //将所有信息存入窗体的tag中
            this.Tag = _mess;

            LoadingFriends(_mess.MyGroup.ToList(), _mess.MyFriends.ToList());


            //加载历史聊天记录
            List<string> lis = FilesHelper.FileHelper.GetMessageFileName(_mess.MyMess.UI_Number);

            if (lis.Count > 0)
            {
                List<DTO_Friend> flis = new List<DTO_Friend>();

                foreach (var item in lis)
                {
                    if (_mess.MyFriends.Where(a => a._Number.Equals(item)).ToList().Count > 0)
                        flis.AddRange(_mess.MyFriends.Where(a => a._Number.Equals(item)).ToList());
                }

               mview.Children.Add(LoadingFriendMess(flis));
            }
        }






        private void LoadingMyMess(UserInfo _u)
        {
            //获取自己的信息中的昵称，将自己的信息存入tag
            nick.Text = _u.UI_Nike;
            //头像
            headimg.Source = string.IsNullOrEmpty(_u.UI_HeadImg) ? headimg.Source : new BitmapImage(new Uri(_u.UI_HeadImg, UriKind.Relative));

            //性别
            sex.Text = _u.UI_Sex == 0 ? "女" : "男";

            //修改备注
            remark.Text = string.IsNullOrEmpty(_u.UI_Remark) ? remark.Text : _u.UI_Remark;

            this.Tag = _u;
            this.Title = _u.UI_Nike;
        }


        private void LoadingFriends(List<GroupInfo> glis, List<DTO_Friend> flis)
        {
            //遍历好友列表
            foreach (var item in glis)
            {
                //创建一个item标签容器
                StackPanel sp = new StackPanel();
                //存入列表信息到item标签容器
                sp.Tag = item;


                //创建一个下拉列表标题容器
                StackPanel spitemtitle = new StackPanel();
                //设置容器样式
                spitemtitle.Margin = new Thickness(5, 5, 0, 0);//左上右下边距
                spitemtitle.Orientation = Orientation.Horizontal;//容器内容的排序方式
                spitemtitle.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#eee"));
                //添加单击事件
                spitemtitle.MouseLeftButtonDown += OpenOrClose;
                //加载到item标签容器中
                sp.Children.Add(spitemtitle);


                //下拉列表项的图标
                Image itemimg = new Image();
                //宽度和高度
                itemimg.Width = 10;
                itemimg.Height = 10;
                //设置图片源
                itemimg.Source = new BitmapImage(new Uri("./Icon/xiala.png", UriKind.Relative));
                //加载到下拉列表标题容器中
                spitemtitle.Children.Add(itemimg);


                //下拉列表项标题
                TextBlock tbitemtitle = new TextBlock();
                //设置显示文字
                tbitemtitle.Text = item.GI_Name;
                //设置样式
                tbitemtitle.Margin = new Thickness(3, 0, 0, 0);
                //添加到下拉列表标题容器中
                spitemtitle.Children.Add(tbitemtitle);


                sp.Children.Add(LoadingFriendMess(flis.Where(a => a._Group == item.GI_ID).ToList()));


                fview.Children.Add(sp);
            }
        }



        void OpenOrClose(object sender, MouseButtonEventArgs e)
        {
            //获取点击的标题
            StackPanel spchi = sender as StackPanel;
            //返回上一级
            StackPanel sp = spchi.Parent as StackPanel;

            if (sp.Children.Count > 1)
            {
                //得到下拉列表标题图标
                Image img = spchi.Children[0] as Image;
                //显示子元素
                if (sp.Children[1].Visibility == System.Windows.Visibility.Collapsed)
                {
                    sp.Children[1].Visibility = System.Windows.Visibility.Visible;

                    img.RenderTransform = new RotateTransform(0);
                    img.Margin = new Thickness(0, 0, 0, 0);
                }
                //隐藏子元素
                else
                {
                    sp.Children[1].Visibility = System.Windows.Visibility.Collapsed;

                    img.RenderTransform = new RotateTransform(-90);
                    img.Margin = new Thickness(0, 13, 0, -7);
                }
            }
        }


        /// <summary>
        /// 解析好友信息
        /// </summary>
        /// <param name="_flis"></param>
        /// <returns></returns>
        private StackPanel LoadingFriendMess(List<DTO_Friend> _flis)
        {

            //创建一个好友信息容器
            StackPanel spfriends = new StackPanel();


            //遍历所有好友信息
            foreach (var item in _flis)
            {
                //创建信息存放容器
                StackPanel sp = new StackPanel();
                //设置宽度
                sp.Width = 240;
                //设置子元素的排序方式
                sp.Orientation = Orientation.Horizontal;//横向排序
                //添加到好友信息容器中
                spfriends.Children.Add(sp);
                sp.MouseLeftButtonDown += sp_MouseLeftButtonDown;
                sp.Tag = item;


                //头像
                Image img = new Image();
                //图片源
                img.Source = new BitmapImage(new Uri("./HeadImgs/h1.bmp", UriKind.Relative));
                //设置显示的头像的高度和宽度
                img.Width = 26;
                img.Height = 26;
                img.Margin = new Thickness(16, 3, 0, 3);
                //设置显示的图片为圆形
                img.Clip = new EllipseGeometry(new Point(13, 13), 13, 13);
                img.ForceCursor = true;
                //添加好友一个好友信息到好友信息容器
                sp.Children.Add(img);


                //创建好友信息展示容器
                StackPanel spmess = new StackPanel();
                sp.Children.Add(spmess);


                //创建一个存放昵称的容器
                StackPanel nick = new StackPanel();
                //设置显示样式
                nick.Orientation = Orientation.Horizontal;//让内容横向排序
                nick.Margin = new Thickness(10, 0, 0, 0);
                spmess.Children.Add(nick);


                //昵称
                TextBlock tbnick = new TextBlock();
                tbnick.Text = item._Nike;
                nick.Children.Add(tbnick);


                //是否是vip 
                TextBlock isvip = new TextBlock();
                isvip.Text = "vip";
                isvip.Foreground = new SolidColorBrush(Colors.OrangeRed);
                nick.Children.Add(isvip);


                //个性签名
                TextBlock gx = new TextBlock();
                gx.Text = string.IsNullOrEmpty(item._Remark) ? "这家伙很懒，什么都没有留下" : item._Remark;
                gx.Margin = new Thickness(10, 0, 0, 0);
                gx.FontSize = 10;
                gx.Foreground = new SolidColorBrush(Colors.Gray);
                spmess.Children.Add(gx);
            }


            return spfriends;
        }


        /// <summary>
        /// 打开聊天窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void sp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //判断是否双击好友
            if (e.ClickCount == 2)
            {
                //获取双击的控件，得到好友信息
                StackPanel sp = sender as StackPanel;
                //判断是否打开聊天窗口
                if (chatwindow == null)
                {
                    //创建聊天窗口
                    chatwindow = new ChatWindow(sp.Tag as DTO_Friend, (this.Tag as DTO_Login).MyMess, this);
                    chatwindow.Closed += chatwindow_Closed;
                    chatwindow.Show();
                }
                else
                {
                    ChatWindow ch = (chatwindow as ChatWindow);
                    ch.AddFriend(sp.Tag as DTO_Friend);
                    ch.Show();
                }
            }
        }

        void chatwindow_Closed(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="_sm"></param>
        /// <returns></returns>
        public int SendMessage(DTO_SMess _sm)
        {
            //通过通讯通道传输聊天信息
            int returnval = alsc.SendMessage(_sm);


            return returnval;
        }



        public void AddMessage(DTO_SMess _mess)
        {
            (chatwindow as ChatWindow).AddMess(_mess);
        }




        /// <summary>
        /// 关闭程序时，将整个进程结束
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Main_Closed(object sender, EventArgs e)
        {
            //关闭整个程序
            Application.Current.Shutdown();
            //throw new NotImplementedException();
        }








        /// <summary>
        /// 获取有本地聊天记录的好友信息
        /// </summary>
        /// <param name="_keys">好友账号</param>
        /// <returns></returns>
        public List<DTO_Friend> GetFriends(List<string> _keys)
        {
            List<DTO_Friend> dlis = new List<DTO_Friend>();


            //获取所有好友信息
            List<DTO_Friend> lis = (this.Tag as DTO_Login).MyFriends.ToList();
            foreach (var item in lis)
            {
                if (_keys.Contains(item._Number))
                    dlis.Add(item);
            }


            return dlis;
        }


        private void button2_Click(object sender, EventArgs e)
        {
            //调用系统默认的浏览器   
            //System.Diagnostics.Process.Start("explorer.exe", "http://blog.csdn.net/testcs_dn");
        }



        /// <summary>
        /// 关闭当前窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //提示用户是否关闭
            MessageBoxResult mbr = MessageBox.Show("是否退出？？？", "信息", MessageBoxButton.YesNo);


            //判断是否退出程序
            if (mbr == MessageBoxResult.No)
            {
                //阻止关闭窗口
                e.Cancel = true;
            }
            else
            {
                ////获取所有聊天消息
                //List<DTO_SMess> slis = (chatwindow as ChatWindow).GetMessages() ?? new List<DTO_SMess>();

                //if (slis.Count > 0)
                //{
                //    FilesHelper.FileHelper.AddFMessage(slis, (this.Tag as DTO_Login).MyMess);
                //}
            }
        }


        //显示好友列表
        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            Border br = sender as Border;
            br.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fdd"));

            ((br.Parent as StackPanel).Children[1] as Border).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fff"));

            //隐藏好友列表
            fscor.Visibility = Visibility.Visible;
            //显示历史消息列表
            msour.Visibility = System.Windows.Visibility.Collapsed;
        }

        /// <summary>
        /// 显示历史消息列表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            Border br = sender as Border;
            br.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fdd"));

            ((br.Parent as StackPanel).Children[0] as Border).Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#fff"));


            //隐藏好友列表
            fscor.Visibility = Visibility.Collapsed;
            //显示历史消息列表
            msour.Visibility = System.Windows.Visibility.Visible;
        }
    }
}
