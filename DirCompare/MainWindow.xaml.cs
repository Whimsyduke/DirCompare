using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace DirCompare
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.CheckBox_ShowFirstDir.IsEnabled = false;
            this.CheckBox_ShowSecondDir.IsEnabled = false;
            this.CheckBox_ShowSameDir.IsEnabled = false;
        }

        #region 变量
        private DirListClass mainDirList;
        #endregion

        #region 控件事件
        /// <summary>
        /// 设置第一路径按钮按下事件。
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">路由事件</param>
        private void Button_FirstPath_Click(object sender, RoutedEventArgs e)
        {
            string projectPath = TextBox_FirstPath.Text;
            if (!Directory.Exists(projectPath))
            {
                projectPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            } 
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.Description = "设置第一路径";
            folderDialog.SelectedPath = projectPath;
            folderDialog.ShowDialog();
            if (folderDialog.SelectedPath != String.Empty)
                TextBox_FirstPath.Text = folderDialog.SelectedPath;
        }
        /// <summary>
        /// 设置第二要路径按钮按下事件。
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">路由事件</param>
        private void Button_SecondPath_Click(object sender, RoutedEventArgs e)
        {
            string projectPath = TextBox_SecondPath.Text;
            if (!Directory.Exists(projectPath))
            {
                projectPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.Description = "设置第二路径";
            folderDialog.SelectedPath = projectPath;
            folderDialog.ShowDialog();
            if (folderDialog.SelectedPath != String.Empty)
                TextBox_SecondPath.Text = folderDialog.SelectedPath;
        }
        /// <summary>
        /// 第一路径文本内容改变事件.
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">路由事件</param>
        private void TextBox_FirstPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Directory.Exists(this.TextBox_FirstPath.Text))
            {
                this.CheckBox_ShowFirstDir.IsEnabled = true;
                if (CheckBox_ShowSecondDir.IsEnabled == true)
                {
                    this.CheckBox_ShowSameDir.IsEnabled = true;
                }
            }
            else
            {
                this.CheckBox_ShowFirstDir.IsEnabled = false;
                this.CheckBox_ShowSameDir.IsEnabled = false;
                this.TreeView_FileTreeView.Items.Clear();
                mainDirList = null;
            }
        }
        /// <summary>
        /// 第二路径文本内容改变事件.
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">路由事件</param>
        private void TextBox_SecondPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Directory.Exists(this.TextBox_SecondPath.Text))
            {
                this.CheckBox_ShowSecondDir.IsEnabled = true;
                if (CheckBox_ShowFirstDir.IsEnabled == true)
                {
                    this.CheckBox_ShowSameDir.IsEnabled = true;
                }
            }
            else
            {
                this.CheckBox_ShowSecondDir.IsEnabled = false;
                this.CheckBox_ShowSameDir.IsEnabled = false;
                this.TreeView_FileTreeView.Items.Clear();
                mainDirList = null;
            }
        }
        /// <summary>
        /// 显示第一路径勾选。
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">路由事件</param>
        private void CheckBox_ShowFirstDir_Checked(object sender, RoutedEventArgs e)
        {
            if (mainDirList != null)
            {
                mainDirList.SetTreeViewItemsVisible(this.CheckBox_ShowFirstDir.IsChecked.Value, this.CheckBox_ShowSecondDir.IsChecked.Value, this.CheckBox_ShowSameDir.IsChecked.Value);
                this.TreeView_FileTreeView.Items.Refresh();
            }
        }
        /// <summary>
        /// 显示第二路径勾选。
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">路由事件</param>
        private void CheckBox_ShowSecondDir_Checked(object sender, RoutedEventArgs e)
        {
            if (mainDirList != null)
            {
                mainDirList.SetTreeViewItemsVisible(this.CheckBox_ShowFirstDir.IsChecked.Value, this.CheckBox_ShowSecondDir.IsChecked.Value, this.CheckBox_ShowSameDir.IsChecked.Value);
                this.TreeView_FileTreeView.Items.Refresh();
            }
        }
        /// <summary>
        /// 显示相同路径勾选。
        /// </summary>
        /// <param name="sender">响应控件</param>
        /// <param name="e">路由事件</param>
        private void CheckBox_ShowSameDir_Checked(object sender, RoutedEventArgs e)
        {
            if (mainDirList != null)
            {
                mainDirList.SetTreeViewItemsVisible(this.CheckBox_ShowFirstDir.IsChecked.Value, this.CheckBox_ShowSecondDir.IsChecked.Value, this.CheckBox_ShowSameDir.IsChecked.Value);
                this.TreeView_FileTreeView.Items.Refresh();
            }
        }
        /// <summary>
        /// 显示目录按钮按下事件。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ShowTreeView_Click(object sender, RoutedEventArgs e)
        {
            FileListInit();
        }
        #endregion

        #region 通用类、结构体、枚举
        /// <summary>
        /// 类类型枚举。
        /// </summary>
        private enum FileOrDirEnum
        {
            File,
            Dir
        }
        /// <summary>
        /// 文件存在类型枚举。
        /// </summary>
        private enum ExistTypeEnum
        {
            First,
            Second,
            Same
        }
        /// <summary>
        /// 文件列表类。
        /// </summary>
        private class FileListClass
        {
            public object first, second;
            private string path;
            private ExistTypeEnum existType;
            private FileOrDirEnum fileOrDir;
            public TreeViewItem treeViewControl;
            /// <summary>
            /// 封装字段
            /// </summary>
            public object First
            {
                get { return first; }
                set { first = value; }
            }
            /// <summary>
            /// 封装字段
            /// </summary>
            public object Second
            {
                get { return second; }
                set { second = value; }
            }
            /// <summary>
            /// 封装字段
            /// </summary>
            public string Path
            {
                get { return path; }
                set { path = value; }
            }
            /// <summary>
            /// 封装字段
            /// </summary>
            public ExistTypeEnum ExistType
            {
                get { return existType; }
                set { existType = value; }
            }
            /// <summary>
            /// 封装字段
            /// </summary>
            public FileOrDirEnum FileOrDir
            {
                get { return fileOrDir; }
                set { fileOrDir = value; }
            }
            /// <summary>
            /// 封装字段
            /// </summary>
            public TreeViewItem TreeViewControl
            {
                get { return treeViewControl; }
                set { treeViewControl = value; }
            }
            /// <summary>
            /// 构造函数。派生目录。
            /// </summary>
            public FileListClass()
            {
            }
            /// <summary>
            /// 构造函数。生成文件。
            /// </summary>
            /// <param name="pathobject">文件</param>
            /// <param name="allList">全部文件列表</param>
            /// <param name="ExistType">文件存在类型</param>
            /// <param name="mainPath">主路径</param>
            public FileListClass(FileInfo pathobject, ExistTypeEnum existTypeEnum, string mainPath, bool firstOrSecond)
            {
                if (firstOrSecond)
                {
                    First = pathobject;
                    Path = (First as FileInfo).FullName.Replace(mainPath, "");
                }
                else
                {
                    Second = pathobject;
                    Path = (Second as FileInfo).FullName.Replace(mainPath, "");
                }
                ExistType = existTypeEnum;
                FileOrDir = FileOrDirEnum.File;
            }
            /// <summary>
            /// 生成目录列表控件
            /// </summary>
            /// <returns>TreeViewListItem控件</returns>
            public TreeViewItem GetTreeViewListItems()
            {
                //容器控件
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;

                //名称文本
                TextBlock existTypeText = new TextBlock();
                switch (this.ExistType)
                {
                    case ExistTypeEnum.First:
                        existTypeText.Text = "①:";
                        break;
                    case ExistTypeEnum.Second:
                        existTypeText.Text = "②:";
                        break;
                    case ExistTypeEnum.Same:
                        existTypeText.Text = "@:";
                        break;
                }
                existTypeText.Foreground = new SolidColorBrush(Colors.Red);
                existTypeText.Margin = new Thickness(5, 0, 5, 0);
                stackPanel.Children.Add(existTypeText);

                //名称文本
                TextBlock textBlock = new TextBlock();
                textBlock.Text = Path.Substring(Path.LastIndexOf("\\") + 1);
                textBlock.Margin = new Thickness(5, 0, 0, 0);
                stackPanel.Children.Add(textBlock);

                //TreeViewItem控件
                TreeViewItem treeViewItem = new TreeViewItem();
                treeViewItem.Header = stackPanel;

                this.TreeViewControl = treeViewItem;
                return treeViewItem;
            }
        }

        /// <summary>
        /// 目录列表类。
        /// </summary>
        private class DirListClass:FileListClass
        {
            private List<object> childList = new List<object>();
            /// <summary>
            /// 封装字段
            /// </summary>
            public List<object> ChildList
            {
                get { return childList; }
                set { childList = value; }
            }
            /// <summary>
            /// 构造函数。生成目录。
            /// </summary>
            /// <param name="pathobject">目录</param>
            /// <param name="allList">全部文件列表</param>
            /// <param name="mainPath">主路径</param>
            public DirListClass(DirectoryInfo pathobject, ExistTypeEnum existTypeEnum, string mainPath, bool firstOrSecond)
                : base()
            {
                if (firstOrSecond)
                {
                    First = pathobject;
                    Path = (First as DirectoryInfo).FullName.Replace(mainPath, "");
                }
                else
                {
                    Second = pathobject;
                    Path = (Second as DirectoryInfo).FullName.Replace(mainPath, "");
                }
                ExistType = existTypeEnum;
                FileOrDir = FileOrDirEnum.Dir;
            }

            /// <summary>
            /// 构造函数。初始化。
            /// </summary>
            /// <param name="firstPath">第一路径</param>
            /// <param name="secondPath">第二路径</param>
            public DirListClass(DirectoryInfo firstPath, DirectoryInfo secondPath)
            {
                this.First = firstPath;
                this.Second = secondPath;
                this.Path = "\\";
                this.FileOrDir = FileOrDirEnum.Dir;
                this.ExistType = ExistTypeEnum.Same;
                this.AddChildList(firstPath, firstPath.FullName, true);
                this.AddChildList(secondPath, secondPath.FullName, false);
            }
            /// <summary>
            /// 添加子目录子文件
            /// </summary>
            /// <param name="pathobject">需要添加内容的父目录</param>
            /// <param name="mainPath">公共路径</param>
            /// <param name="firstOrSecond">第一路径或第二路径</param>
            public void AddChildList(DirectoryInfo pathobject, string mainPath, bool firstOrSecond)
            {

                if (firstOrSecond)
                {
                    //第一路径

                    //子目录
                    foreach (DirectoryInfo select in pathobject.GetDirectories())
                    {
                        DirListClass tempClass;
                        object tempobject = this.SelectExistClass(select.FullName.Replace(mainPath, ""));
                        if (tempobject == null)
                        {
                            tempClass = new DirListClass(select, ExistTypeEnum.First, mainPath, firstOrSecond);
                            this.ChildList.Add(tempClass);
                        }
                        else
                        {
                            tempClass = tempobject as DirListClass;
                            tempClass.First = select;
                            tempClass.ExistType = ExistTypeEnum.Same;
                        }
                        tempClass.AddChildList(select, mainPath, firstOrSecond);
                    }
                    //子文件
                    foreach (FileInfo select in pathobject.GetFiles())
                    {
                        FileListClass tempClass;
                        object tempobject = this.SelectExistClass(select.FullName.Replace(mainPath, ""));
                        if (tempobject == null)
                        {
                            tempClass = new FileListClass(select, ExistTypeEnum.First, mainPath, firstOrSecond);
                            this.ChildList.Add(tempClass);
                        }
                        else
                        {
                            tempClass = tempobject as FileListClass;
                            tempClass.First = select;
                            tempClass.ExistType = ExistTypeEnum.Same;
                        }
                    }
                }
                else
                {
                    //第二路径

                    //子目录
                    foreach (DirectoryInfo select in pathobject.GetDirectories())
                    {
                        DirListClass tempClass;
                        object tempobject = this.SelectExistClass(select.FullName.Replace(mainPath, ""));
                        if (tempobject == null)
                        {
                            tempClass = new DirListClass(select, ExistTypeEnum.Second, mainPath, firstOrSecond);
                            this.ChildList.Add(tempClass);
                        }
                        else
                        {
                            tempClass = tempobject as DirListClass;
                            tempClass.Second = select;
                            tempClass.ExistType = ExistTypeEnum.Same;
                        }
                        tempClass.AddChildList(select, mainPath, firstOrSecond);
                    }
                    //子文件
                    foreach (FileInfo select in pathobject.GetFiles())
                    {
                        FileListClass tempClass;
                        object tempobject = this.SelectExistClass(select.FullName.Replace(mainPath, ""));
                        if (tempobject == null)
                        {
                            tempClass = new FileListClass(select, ExistTypeEnum.Second, mainPath, firstOrSecond);
                            this.ChildList.Add(tempClass);
                        }
                        else
                        {
                            tempClass = tempobject as FileListClass;
                            tempClass.Second = select;
                            tempClass.ExistType = ExistTypeEnum.Same;
                        }
                    }
                }
            }
            /// <summary>
            /// 筛选符合条件的目录或文件
            /// </summary>
            /// <param name="checkPath">筛选参照路径</param>
            /// <returns>筛选结果</returns>
            private object SelectExistClass(string checkPath)
            {
                var tempClass = this.ChildList.Where(c => (c as FileListClass).Path == checkPath);
                if (tempClass.Count() != 0)
                    return tempClass.Select(c => c).First();
                else
                    return null;
            }
            /// <summary>
            /// 生成目录列表控件
            /// </summary>
            /// <returns>TreeViewListItem控件</returns>
            public new TreeViewItem GetTreeViewListItems()
            {
                //容器控件
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;

                //名称文本
                TextBlock existTypeText = new TextBlock();
                switch(this.ExistType)
                {
                    case ExistTypeEnum.First:
                        existTypeText.Text = "①:";
                        break;
                    case ExistTypeEnum.Second:
                        existTypeText.Text = "②:";
                        break;
                    case ExistTypeEnum.Same:
                        existTypeText.Text = "@:";
                        break;
                }
                existTypeText.Foreground = new SolidColorBrush(Colors.Red);
                existTypeText.Margin = new Thickness(5, 0, 5, 0);
                stackPanel.Children.Add(existTypeText);
                
                //名称文本
                TextBlock textBlock = new TextBlock();
                textBlock.Text = Path.Substring(Path.LastIndexOf("\\") + 1) + "\\";
                textBlock.Margin = new Thickness(5, 0, 0, 0);
                stackPanel.Children.Add(textBlock);

                //TreeViewItem控件
                TreeViewItem treeViewItem = new TreeViewItem();
                treeViewItem.Header = stackPanel;

                //添加下级控件
                if (this.ChildList.Count != 0)
                {
                    foreach (object select in this.ChildList)
                    {
                        if ((select as FileListClass).FileOrDir == FileOrDirEnum.Dir)
                            treeViewItem.Items.Add((select as DirListClass).GetTreeViewListItems());
                        else
                            treeViewItem.Items.Add((select as FileListClass).GetTreeViewListItems());
                    }
                }
                this.TreeViewControl = treeViewItem;
                return treeViewItem;
            }
            /// <summary>
            /// 设置目录列表可见性
            /// </summary>
            /// <param name="first">第一路径</param>
            /// <param name="second">第二路径</param>
            /// <param name="same">相同路径</param>
            public bool SetTreeViewItemsVisible(bool firstPath, bool secondPath, bool samePath)
            {
                bool tempBool = (firstPath && this.ExistType == ExistTypeEnum.First) || (secondPath && this.ExistType == ExistTypeEnum.Second) || (samePath && this.ExistType == ExistTypeEnum.Same);
                foreach (object select in this.ChildList)
                {
                    FileListClass tempClass = select as FileListClass;
                    if (tempClass.FileOrDir == FileOrDirEnum.Dir)
                    {
                        if ((select as DirListClass).SetTreeViewItemsVisible(firstPath, secondPath, samePath))
                        {
                            tempBool = true;
                        }
                    }
                    else
                    {
                        if ((firstPath && tempClass.ExistType == ExistTypeEnum.First) || (secondPath && tempClass.ExistType == ExistTypeEnum.Second) || (samePath && tempClass.ExistType == ExistTypeEnum.Same))
                        {
                            tempClass.TreeViewControl.Visibility = Visibility.Visible;
                            tempBool = true;
                        }
                        else
                        {
                            tempClass.TreeViewControl.Visibility = Visibility.Collapsed;
                        }
                    }
                }
                if (tempBool)
                {
                    this.TreeViewControl.Visibility = Visibility.Visible;
                }
                else
                {
                    this.TreeViewControl.Visibility = Visibility.Collapsed;
                }
                return tempBool;
            }
        }
        #endregion

        #region 通用方法
        /// <summary>
        /// 刷新文件列表。
        /// </summary>
        private void FileListInit()
        {
            this.TreeView_FileTreeView.Items.Clear();
            mainDirList = new DirListClass(new DirectoryInfo(this.TextBox_FirstPath.Text), new DirectoryInfo(this.TextBox_SecondPath.Text));
            this.TreeView_FileTreeView.Items.Add(mainDirList.GetTreeViewListItems());
        }
        #endregion

    }
}
