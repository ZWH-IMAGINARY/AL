using AL.AL_Service;
using AL.model;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AL
{
    /// <summary>
    /// ChatWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChatWindow : MetroWindow
    {
        //好友列表窗口
        private Window w;
        private List<DTO_SMess> messages = new List<DTO_SMess>();

        /// <summary>
        /// 给予所有聊天信息
        /// </summary>
        /// <returns></returns>
        public List<DTO_SMess> GetMessages()
        {
            return messages;
        }


        public ChatWindow()
        {
            InitializeComponent();
        }

        public ChatWindow(DTO_Friend _df, UserInfo _u, Window _w)
        {
            InitializeComponent();
            w = _w;


            //将自己的信息存入消息框tag
            MessageValue.Tag = _u;

            //获取本地聊天记录
            Dictionary<string, List<DTO_SMess>> dic = FilesHelper.FileHelper.ReadFMessages(_u.UI_Number);
            //提取好友信息
            if (dic.Count > 0)
            {
                List<string> li = dic.Keys.ToList();
                foreach (var item in (w as Main).GetFriends(li))
                {
                    AddOneUser(item);
                    messages.AddRange(dic[item._Number]);
                }

                List<DTO_SMess> lis = dic[(this.Tag as DTO_Friend)._Number];
                foreach (var item in lis)
                {
                    AddMess(item);
                }
            }
            //添加一个可聊天好友
            AddOneUser(_df);


        }



        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    DialogResult result = MessageBox.Show("确定要退出吗？", "信息");
        //    if (result == DialogResult.No)
        //    {
        //        e.Cancel = true; //取消关闭操作  
        //    }
        //}






        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MetroWindow_Closing_1(object sender, CancelEventArgs e)
        {
            //MessageBoxResult mbr = MessageBox.Show("是否关闭", "信息", MessageBoxButton.YesNo);
            //阻止关闭当前聊天窗口
            e.Cancel = true;

            //隐藏当前窗口
            this.Hide();
        }











        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendTo(object sender, RoutedEventArgs e)
        {
            string _mess = (new TextRange(MessageValue.Document.ContentStart, MessageValue.Document.ContentEnd)).Text.Replace("\r\n", "\n");
            if (!string.IsNullOrEmpty(_mess))
            {
                // AddMessage(null, MessageValue.Tag as UserInfo, HorizontalAlignment.Right, _mess);

                //创建聊天消息存放对象
                DTO_SMess _smessage = new DTO_SMess();
                //获取的信息
                _smessage.Despatcher = (MessageValue.Tag as UserInfo);
                _smessage.SendToNum = (this.Tag as DTO_Friend);
                _smessage.SendDate = DateTime.Now;
                _smessage.Message = _mess;
                _smessage.type = 0;
                _smessage.ReadType = 1;

                if ((this.Tag as DTO_Friend)._Number == _smessage.SendToNum._Number)
                    AddMess(_smessage);
                //添加消息记录
                //messages.Add(_smessage);
                //发送
                int i = (w as Main).SendMessage(_smessage);
                //清空消息框中的数据
                MessageValue.Document.Blocks.Clear();
            }
        }









        /// <summary>
        /// 清空聊天的消息
        /// </summary>
        /// <param name="_mess"></param>
        private void MessClear(object sender, RoutedEventArgs e)
        {
            MessageValue.Document.Blocks.Clear();
        }






        /// <summary>
        /// 接收聊天消息
        /// </summary>
        /// <param name="_mess"></param>
        public void AddMess(DTO_SMess _mess)
        {

            //添加消息记录
            messages.Add(_mess);


            if ((this.Tag as DTO_Friend)._Number == _mess.Despatcher.UI_Number)
            {
                AddMessage(_mess.Despatcher, HorizontalAlignment.Left, _mess.Message);
            }
            else
            {

                AddMessage(_mess.Despatcher, HorizontalAlignment.Right, _mess.Message);
            }




            //让聊天窗口消息内容始终保持底部
            mysv.ScrollToVerticalOffset(mysv.ActualHeight + mysv.ViewportHeight);
        }









        //发送消息
        private void AddMessage(UserInfo u, HorizontalAlignment _ha, string _mess)
        {
            //SendMessages
            //创建存放消息的容器
            StackPanel sp = new StackPanel();
            //创建样式
            sp.Orientation = Orientation.Horizontal;//横向排列
            sp.HorizontalAlignment = _ha;//内容水平靠右
            sp.Margin = new Thickness(0, 6, 0, 6);//边距
            SendMessages.Children.Add(sp);


            //创建聊天内容和昵称存放的容器
            StackPanel spchat = new StackPanel();
            sp.Children.Add(spchat);


            //自己的昵称
            TextBlock tbnick = new TextBlock();
            //昵称的样式
            tbnick.HorizontalAlignment = _ha;//昵称水平居右
            tbnick.Margin = new Thickness(0, 0, 0, 3);//边距
            //内容

            tbnick.Text = u.UI_Nike;

            tbnick.Foreground = new SolidColorBrush(Colors.BlueViolet);
            spchat.Children.Add(tbnick);


            //消息内容
            TextBlock tbmess = new TextBlock();
            //样式
            tbmess.TextWrapping = TextWrapping.Wrap;//自动换行
            tbmess.FontSize = 16;
            //内容
            tbmess.Text = _mess.Substring(0, _mess.LastIndexOf("\n"));
            spchat.Children.Add(tbmess);


            //头像
            Image img = new Image();
            //样式
            img.Width = 30;
            img.Height = 30;
            img.VerticalAlignment = System.Windows.VerticalAlignment.Top;//定位顶部
            img.Clip = new EllipseGeometry(new Point(15, 15), 15, 15);
            //图片源
            img.Source = new BitmapImage(new Uri("./HeadImgs/h2.bmp", UriKind.Relative));
            sp.Children.Add(img);

        }









        /// <summary>
        /// 添加一个聊天用户
        /// </summary>
        /// <param name="_df"></param>
        void AddOneUser(DTO_Friend _df)
        {
            //创建容器
            StackPanel sp = new StackPanel();
            //设置内容排列方式
            sp.Orientation = Orientation.Horizontal;
            sp.MouseLeftButtonDown += sp_MouseLeftButtonDown;
            //设置边距
            sp.Margin = new Thickness(3, 3, 3, 0);
            sp.Tag = _df;


            //头像
            Image img = new Image();
            img.Source = new BitmapImage(new Uri("./HeadImgs/h1.bmp", UriKind.Relative));
            img.Width = 30;
            img.Height = 30;
            img.Clip = new EllipseGeometry(new Point(15, 15), 15, 15);
            sp.Children.Add(img);


            //好友名称
            TextBlock tb = new TextBlock();
            //设置好友名称
            string nick = string.IsNullOrEmpty(_df._Remark) ? _df._Nike : _df._Remark;
            tb.Text = nick.Length > 6 ? nick.Substring(0, 3) + "..." : nick;
            tb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            tb.FontSize = 15;
            sp.Children.Add(tb);


            bool isadd = true;

            foreach (var item in messagefriendslist.Children)
            {
                if (((item as StackPanel).Tag as DTO_Friend)._Number == _df._Number)
                    isadd = false;
            }
            if (isadd)
            {
                messagefriendslist.Children.Add(sp);
            }


            this.Tag = _df;
            this.Title = string.IsNullOrEmpty(_df._Remark) ? _df._Nike : _df._Remark;
        }










        /// <summary>
        /// 选择和谁聊天
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void sp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //获取选择的好友
            StackPanel sp = sender as StackPanel;
            //得到好友信息
            DTO_Friend df = sp.Tag as DTO_Friend;



            if (!df._Number.Equals((this.Tag as DTO_Friend)._Number))
            {
                //清空消息展示框
                SendMessages.Children.Clear();

                mysv.ScrollToVerticalOffset(0);
                this.Title = string.IsNullOrEmpty(df._Remark) ? df._Nike : df._Remark;
                this.Tag = df;


                //添加聊天消息
                foreach (var item in messages.Where(a => a.Despatcher.UI_Number.Equals(df._Number) || a.SendToNum._Number.Equals(df._Number)).ToList())
                {
                    AddMess(item);
                }
            }
        }







        /// <summary>
        /// 添加一个用户聊天
        /// </summary>
        /// <param name="_df"></param>
        public void AddFriend(DTO_Friend _df)
        {
            AddOneUser(_df);
        }











        //给RichTextBox追加内容
        //RichTextBox rtb = new RichTextBox();
        //rtb.AppendText("sdfsd");
        //获取RichTExtBox的text内容
        //TextRange a = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
    }
}
