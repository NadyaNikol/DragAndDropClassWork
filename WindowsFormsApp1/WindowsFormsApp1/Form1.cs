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

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        ImageList image = null;
        TreeView tree = null;
        public Form1()
        {
            InitializeComponent();

            listView1.FullRowSelect = true;
            listView1.GridLines = true;

            treeView1.BeforeSelect += TreeView1_BeforeSelect;
            treeView1.BeforeExpand += TreeView1_BeforeExpand; // раскрытие
            FillDriveNodes();

            treeView1.NodeMouseDoubleClick += TreeView1_NodeMouseDoubleClick;

            image = new ImageList();
            tree = new TreeView();

            try
            {
 
                tree.ImageList = image;
                image.ImageSize = new Size(20, 20);

                Bitmap bmp = new Bitmap("image1.bmp");
                image.Images.Add(bmp);

                treeView1.ImageList = image;

                ImageList largeGallary = new ImageList();
                largeGallary.Images.Add(bmp);

                largeGallary.ImageSize = new Size(30, 30);
                

                listView1.LargeImageList = largeGallary;
                listView1.SmallImageList = largeGallary;
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);


            }
            catch (Exception)
            {
            }


            treeView1.MouseDown += TreeView1_MouseDown;
            listView1.DragEnter += ListView1_DragEnter;
            listView1.DragDrop += ListView1_DragDrop;

        }

        private void ListView1_DragDrop(object sender, DragEventArgs e)
        {
            listView1.Items.Add(e.Data.GetData(DataFormats.StringFormat).ToString());
        }

        private void ListView1_DragEnter(object sender, DragEventArgs e)
        {
           if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void TreeView1_MouseDown(object sender, MouseEventArgs e)
        {
            listView1.DoDragDrop(treeView1.Nodes, DragDropEffects.Copy);
        }

        //событие двойного клика мышью по дереву
        private void TreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            listView1.Clear();
            //буфер для хранения списка папок в папке по котой было инициировано событие 
            string[] dirs = null;

            //блок try служит для случаев, когда доступ к системной папки запрещен системой, если доступ будет запрещен сгинирируется исключение которое проброситься в пустой cath.
            try
            {
                //проверяем сщиствует ли папко по данному пути 
                if (Directory.Exists(e.Node.FullPath))
                {
                    //записываем список вложениых папок.
                    dirs = Directory.GetDirectories(e.Node.FullPath);

                    //если папка не пустая
                    if (dirs.Length != 0)
                    {
                        //в цыкле добавляем кждую папку из списка dir
                        foreach (var dir in dirs)
                        {
                            listView1.Items.Add(dir);
                           

                        }

                    }

                }

            }
            catch (Exception)
            {
            }

        }

        private void FillDriveNodes()
        {
            try
            {
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    TreeNode driveNode = new TreeNode(drive.Name);
                    FillTreeNode(driveNode, drive.Name);
                    treeView1.Nodes.Add(driveNode);
                }
            }
            catch (Exception)
            {
            }
        }

        private void TreeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            string[] dirs;
            try
            {
                if (Directory.Exists(e.Node.FullPath))
                {
                    dirs = Directory.GetDirectories(e.Node.FullPath);
                    if (dirs.Length != 0)
                    {
                        for (int i = 0; i < dirs.Length; i++)
                        {
                            TreeNode dirNode = new TreeNode(new DirectoryInfo(dirs[i]).Name);
                            FillTreeNode(dirNode, dirs[i]);
                            e.Node.Nodes.Add(dirNode);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void TreeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();

            string[] dirs;

            try
            {
                if (Directory.Exists(e.Node.FullPath))
                {
                    dirs = Directory.GetDirectories(e.Node.FullPath);

                    if (dirs.Length != 0)
                    {

                        for (int i = 0; i < dirs.Length; i++)
                        {
                            TreeNode dirNode = new TreeNode(new DirectoryInfo(dirs[i]).Name);
                            FillTreeNode(dirNode, dirs[i]);
                            e.Node.Nodes.Add(dirNode);
                        }

                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void FillTreeNode(TreeNode driveNode, string path)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(path);
                foreach (string dir in dirs)
                {
                    TreeNode dirNode = new TreeNode();
                    dirNode.Text = dir.Remove(0, dir.LastIndexOf("\\") + 1);

                    driveNode.Nodes.Add(dirNode);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
