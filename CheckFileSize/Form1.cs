using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CheckFileSize
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            InitDataViewColumn();


            //for (int i = 0; i < 100; i++)
            //{
            //    int index = dataGridView1.Rows.Add();
            //    dataGridView1.Rows[index].ReadOnly = true;
            //    dataGridView1.Rows[index].Cells["name"].Value = "名称";
            //    dataGridView1.Rows[index].Cells["path"].Value = "路径path**********************";
            //    dataGridView1.Rows[index].Cells["size"].Value = "135";
                
            //}
        }

        //大于100M
        float _FSizeValue = 100.0f;

        #region 初始化dataview的列
        private void InitDataViewColumn()
        {
            //去掉左边那一列
            dataGridView1.RowHeadersVisible = false;
            //去掉下面哪一行
            dataGridView1.AllowUserToAddRows = false;

            int columnsIndex = dataGridView1.Columns.Add("ID", "ID");
            dataGridView1.Columns[columnsIndex].Width = 50;
            //不可点击
            dataGridView1.Columns[columnsIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
            columnsIndex = dataGridView1.Columns.Add("name", "文件名称");
            dataGridView1.Columns[columnsIndex].Width = 100;
            dataGridView1.Columns[columnsIndex].SortMode= DataGridViewColumnSortMode.NotSortable;
            columnsIndex = dataGridView1.Columns.Add("path", "文件路径");
            dataGridView1.Columns[columnsIndex].Width = 180;
            dataGridView1.Columns[columnsIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
            columnsIndex = dataGridView1.Columns.Add("size", "文件大小");
            dataGridView1.Columns[columnsIndex].Width = 100;
            dataGridView1.Columns[columnsIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
        }
        #endregion

        #region 打开文件夹路径
        private void OnOpenFolderClick(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            DialogResult result= fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = fbd.SelectedPath;
                //Console.WriteLine("点击确定");
                GetAllFilesFromFolderPath(fbd.SelectedPath);
            }
        }
        #endregion

        #region 通过文件夹路径获取所有文件
        private void GetAllFilesFromFolderPath(string path)
        {
            DirectoryInfo RootFolder = new DirectoryInfo(path);

            FileSystemInfo[] Fsi = RootFolder.GetFileSystemInfos();
            for (int i = 0; i < Fsi.Length; i++)
            {
                if (Fsi[i] is DirectoryInfo)//是文件夹
                {
                    string _TempPath = (Fsi[i] as DirectoryInfo).FullName;
                    GetAllFilesFromFolderPath(_TempPath);
                }
                if (Fsi[i] is FileInfo)//是文件
                {
                    FileInfo _TempFileInfo = Fsi[i] as FileInfo;
                    Console.WriteLine(_TempFileInfo.Name);
                    float _TempFileSize = (float)_TempFileInfo.Length / (1024.0f * 1024.0f);
                    if (_TempFileSize>_FSizeValue)
                        AddFileInforItemToDataViewRows(_TempFileInfo);
                }
            }
            
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
            dataGridView1.Rows[index].ReadOnly = true;
            dataGridView1.Rows[index].Cells["ID"].Value = index;
            dataGridView1.Rows[index].Cells["name"].Value = _TempFileName;
            dataGridView1.Rows[index].Cells["path"].Value = _TempFilePath;
            dataGridView1.Rows[index].Cells["size"].Value = _TempFileSize + "MB";
        }
        #endregion

    }
}
