using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckFileSize
{
    public partial class Form1 : Form
    {
        //打开文件夹组件
        FolderBrowserDialog fbd;
        //大于50M
        float _FMinSizeValue = 50.0f;
        //大于100M
        float _FMaxSizeValue = 100.0f;
        //文件夹路径
        string FolderPath = "";
        
        public Form1()
        {
            InitializeComponent();
            //初始dataGridView
            InitDataViewColumn();
            //初始化打开文件夹
            fbd = new FolderBrowserDialog();
            
            CheckForIllegalCrossThreadCalls = false;
        }
       


        #region 初始化dataview的列
        private void InitDataViewColumn()
        {
            //去掉左边那一列
            dataGridView1.RowHeadersVisible = false;
            //去掉下面哪一行
            dataGridView1.AllowUserToAddRows = false;
            
            int columnsIndex = dataGridView1.Columns.Add("ID", "ID");
            dataGridView1.Columns[columnsIndex].Width = (int)(0.073f * dataGridView1.Width);//30
            //不可点击
            dataGridView1.Columns[columnsIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
            columnsIndex = dataGridView1.Columns.Add("name", "文件名称");
            dataGridView1.Columns[columnsIndex].Width = (int)(0.244f * dataGridView1.Width);//100;
            dataGridView1.Columns[columnsIndex].SortMode= DataGridViewColumnSortMode.NotSortable;
            columnsIndex = dataGridView1.Columns.Add("path", "文件路径");
            dataGridView1.Columns[columnsIndex].Width = (int)(0.439f * dataGridView1.Width);// 180;
            dataGridView1.Columns[columnsIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
            columnsIndex = dataGridView1.Columns.Add("size", "文件大小");
            dataGridView1.Columns[columnsIndex].Width = (int)(0.244f * dataGridView1.Width);//100;
            dataGridView1.Columns[columnsIndex].SortMode = DataGridViewColumnSortMode.NotSortable;

            //设置每列自动铺满
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        #endregion

        #region 打开文件夹路径
        private void OnOpenFolderClick(object sender, EventArgs e)
        {
           
            DialogResult result= fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = fbd.SelectedPath;
                FolderPath = fbd.SelectedPath;

                //  Thread t = new Thread(CreateThreadCalcFileSize);
                //  t.Start();
                ////
                
                CreateThreadCalcFileSize();
            }
        }
        #endregion
        
        #region 通过文件夹路径获取所有文件
        private void GetAllFilesFromFolderPath(string path)
        {
            string _TempCurrent = "";

            DirectoryInfo RootFolder = new DirectoryInfo(path);
            FileSystemInfo[] Fsi = RootFolder.GetFileSystemInfos();
            for (int i = 0; i < Fsi.Length; i++)
            {
                _TempCurrent = Fsi[i].Name;
                // label_State.Text = _TempCurrent;
                this.Invoke(new Action(() => label_State.Text = _TempCurrent));

                if (Fsi[i] is DirectoryInfo)//是文件夹
                {
                    string _TempPath = (Fsi[i] as DirectoryInfo).FullName;
                    GetAllFilesFromFolderPath(_TempPath);
                }
                if (Fsi[i] is FileInfo)//是文件
                {
                    FileInfo _TempFileInfo = Fsi[i] as FileInfo;
                 //   Console.WriteLine(_TempFileInfo.Name);
                    float _TempFileSize = (float)_TempFileInfo.Length / (1024.0f * 1024.0f);
                    if (_TempFileSize > _FMinSizeValue)
                    {
                        this.Invoke(new Action(() => AddFileInforItemToDataViewRows(_TempFileInfo)));
                       // AddFileInforItemToDataViewRows(_TempFileInfo);
                    }
                }
            }
            
            this.Invoke(new Action(() => label_State.Text = "已无数据"));
        }
        #endregion

        #region 通过文件信息添加数据列表
        void AddFileInforItemToDataViewRows(FileInfo _TempFile)
        {
            string _TempFileName = _TempFile.Name;
            string _TempFilePath = _TempFile.FullName;
            //1024字节=1kb  1024kb=1MB
            float _TempFileSize=(float)_TempFile.Length/(1024.0f*1024.0f);

            int index = dataGridView1.Rows.Add();
            if (_TempFileSize >= _FMaxSizeValue)
                dataGridView1.Rows[index].DefaultCellStyle.ForeColor = Color.Red;
            else
                dataGridView1.Rows[index].DefaultCellStyle.ForeColor = Color.Green;
            dataGridView1.Rows[index].ReadOnly = true;
            dataGridView1.Rows[index].Cells["ID"].Value = index;
            dataGridView1.Rows[index].Cells["name"].Value = _TempFileName;
            dataGridView1.Rows[index].Cells["path"].Value = _TempFilePath;
            dataGridView1.Rows[index].Cells["size"].Value = _TempFileSize + "MB";
        }
        #endregion

        #region 输入整数或小数
        private void OnWaringTextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Trim() == "")
            {
                textBox2.Text = _FMinSizeValue.ToString();
                return;
            }

            Regex _TempRegexFloat = new Regex("^([0-9]{1,}[.][0-9]*)$");
            Regex _TempRegexInt = new Regex("^([0-9]{1,})$");

            if (_TempRegexFloat.IsMatch(textBox2.Text.Trim())
                || _TempRegexInt.IsMatch(textBox2.Text.Trim()))
            {
                _FMinSizeValue = float.Parse(textBox2.Text.Trim());
            }
            else
            {
                textBox2.Text = _FMinSizeValue.ToString();
                MessageBox.Show("输入文件限制的大小");
            }
        }
        

        private void OnFailedTextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text.Trim() == "")
            {
                textBox3.Text = _FMaxSizeValue.ToString();
                return;
            }

            Regex _TempRegexFloat = new Regex("^([0-9]{1,}[.][0-9]*)$");
            Regex _TempRegexInt = new Regex("^([0-9]{1,})$");

            if (_TempRegexFloat.IsMatch(textBox3.Text.Trim())
                || _TempRegexInt.IsMatch(textBox3.Text.Trim()))
            {
                _FMaxSizeValue = float.Parse(textBox3.Text.Trim());

            }
            else
            {
                textBox3.Text = _FMaxSizeValue.ToString();
                MessageBox.Show("输入文件限制的大小");
            }
        }

        #endregion

        #region 点击查询按钮开始查询
        private void OnQueryClick(object sender, EventArgs e)
        {
            if (FolderPath == "")
                return;

            CreateThreadCalcFileSize();
        }
        #endregion

        private CancellationTokenSource _cts;
        private void CreateThreadCalcFileSize()
        {
            if (_cts != null)
                _cts.Cancel();

            //清除每行的数据
            dataGridView1.Rows.Clear();

            _cts = new CancellationTokenSource();
            ThreadPool.QueueUserWorkItem(state => GetAllFilesFromFolderPath(FolderPath));
            
            //GetAllFilesFromFolderPath(FolderPath);
        }

    }
}
