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
        ImageList largeGallary, smallGallary = null;
        ToolBar toolbar = null;
        OpenFileDialog openfile = null;

        public Form1()
        {
            InitializeComponent();

            treeView1.BeforeExpand += TreeView1_BeforeSelect;
            FillDriveNodes();

            treeView1.ItemDrag += TreeView1_ItemDrag;
            treeView1.NodeMouseDoubleClick += TreeView1_NodeMouseDoubleClick;

            InitialListView();

            InitialContextMenu();

            InitialToolBar();

            richTextBox1.AllowDrop = true;
            richTextBox1.DragEnter += RichTextBox1_DragEnter;
            richTextBox1.DragDrop += RichTextBox1_DragDrop;

        }

        private void TreeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            richTextBox1.ReadOnly = false;
            TreeNode tree = (TreeNode)e.Item;
            openfile = new OpenFileDialog();
            DoDragDrop(tree.FullPath, DragDropEffects.Copy | DragDropEffects.Move);

            try
            {
                if (openfile.FileName.EndsWith("rtf"))
                {
                    richTextBox1.LoadFile(openfile.FileName, RichTextBoxStreamType.RichText);
                }
                else
                {
                    using (StreamReader reader = new StreamReader(openfile.FileName, Encoding.Default))
                    {
                        richTextBox1.Text = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception)
            {
            }
        }


        private void RichTextBox1_DragDrop(object sender, DragEventArgs e)
        {

            openfile = new OpenFileDialog();
            string name = e.Data.GetData(DataFormats.Text).ToString();
            richTextBox1.Clear();

            try
            {
                openfile = new OpenFileDialog();
                openfile.FileName = name;
            }
            catch (Exception)
            {

               
            }
        }

        private void RichTextBox1_DragEnter(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void RichTextBox1_MouseDown(object sender, MouseEventArgs e)
        {
           richTextBox1.AllowDrop = true;
           richTextBox1.DoDragDrop(richTextBox1.Text, DragDropEffects.Copy);
        }

        private void InitialContextMenu()
        {
            contextMenuStrip1.Items.Add("Detail");
            contextMenuStrip1.Items.Add("Tile");
            contextMenuStrip1.Items.Add("LargeIcon");
            contextMenuStrip1.Items.Add("SmallIcon");
            contextMenuStrip1.Items.Add("List");

            foreach (var item in contextMenuStrip1.Items)
            {
                (item as ToolStripMenuItem).Click += Form1_Click;
            }

            listView1.ContextMenuStrip = contextMenuStrip1;

        }

        private void InitialToolBar ()
        {
            ImageList gallery = new ImageList();
            gallery.ImageSize = new Size(30, 30);
            gallery.Images.Add(new Bitmap("selectall.bmp"));
            gallery.Images.Add(new Bitmap("removeSelection.bmp"));


            toolbar = new ToolBar();
            toolbar.Appearance = ToolBarAppearance.Flat;
            toolbar.BorderStyle = BorderStyle.Fixed3D;
            toolbar.ImageList = gallery;
            toolbar.Location = new Point(this.Location.X, this.Location.Y);
            ToolBarButton toolBarButton1 = new ToolBarButton();
            toolBarButton1.Tag = "selectall";
            toolBarButton1.Text = "Select all";
            ToolBarButton toolBarButton2 = new ToolBarButton();
            toolBarButton2.Tag = "removeSelected";
            toolBarButton2.Text = "Remove selected";

            toolBarButton1.ImageIndex = 0;
            toolBarButton2.ImageIndex = 1;


            toolbar.Buttons.Add(toolBarButton1);
            toolbar.Buttons.Add(toolBarButton2);

            this.Controls.Add(toolbar);

            toolbar.ButtonClick += Toolbar_ButtonClick;
        }

        private void InitialListView ()
        {

            listView1.FullRowSelect = true;
            listView1.GridLines = true;

            largeGallary = new ImageList();
            smallGallary = new ImageList();


            largeGallary.ImageSize = new Size(60, 60);
            smallGallary.ImageSize = new Size(30, 30);
            listView1.LargeImageList = largeGallary;
            listView1.SmallImageList = smallGallary;
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void Toolbar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            switch (e.Button.Tag)
            {
                case "selectall":
                    {
                        for (int i = 0; i < listView1.Items.Count; i++)
                        {
                            listView1.Items[i].Selected = true;
                            listView1.Select();
                        }
                        break;
                    }

                case "removeSelected":
                    {
                        for (int i = 0; i < listView1.Items.Count; i++)
                        {
                            listView1.Items[i].Selected = false;
                            listView1.Select();
                        }
                        break;
                    }

                default:
                    break;
            }
        }

        private void Form1_Click(object sender, EventArgs e)
        {
            switch ((sender as ToolStripMenuItem).Text)
            {
                case "Detail":
                    {
                        listView1.View = View.Details;
                        break;
                    }
                case "SmallIcon":
                    {
                        listView1.View = View.SmallIcon;
                        break;
                    }
                case "LargeIcon":
                    {
                        listView1.View = View.LargeIcon;
                        break;
                    }
                case "List":
                    {
                        listView1.View = View.List;
                        break;
                    }
                case "Tile":
                    {
                        listView1.View = View.Tile;
                        break;
                    }
                default:
                    break;
            }
        }

        private void TreeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
           
            listView1.Clear();
            if (Directory.Exists(e.Node.FullPath))
            {
                try
                {
                    string[] dirs = Directory.GetDirectories(e.Node.FullPath);

                    if (dirs.Length != 0)
                    {
                        listView1.Columns.Add("FileName");
                        listView1.Columns[0].Width = 300;
                        listView1.Columns.Add("FilePath");
                        listView1.Columns[0].Width = 300;

                        textBoxAdress.Text = (new FileInfo(Path.GetFullPath(dirs[0]))).ToString();

                        for (int i = 0; i < dirs.Length; i++)
                        {
                         
                            listView1.Items.Add(Path.GetFileName(dirs[i]), 0);
                            listView1.Items[i].SubItems.Add((new FileInfo(Path.GetFullPath(dirs[i]))).ToString());

                            Bitmap image = new Bitmap("image1.bmp");
                            largeGallary.Images.Add(image);
                            smallGallary.Images.Add(image);
                        }

                    }
                }
                catch (Exception)
                { }

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
            catch (Exception ex )
            {
                MessageBox.Show($"{ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TreeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            e.Node.Nodes.Clear();
            try
            {
                if (Directory.Exists(e.Node.FullPath))
                {
                    string[] dir = Directory.GetDirectories(e.Node.FullPath);
                    for (int i = 0; i < dir.Length; i++)
                    {
                        TreeNode dirNode = new TreeNode(new DirectoryInfo(dir[i]).Name);
                        FillTreeNode(dirNode, dir[i]);
                        e.Node.Nodes.Add(dirNode);
                    }


                    string[] file = Directory.GetFiles(e.Node.FullPath);
                    for (int i = 0; i < file.Length; i++)
                    {
                        TreeNode fileNode = new TreeNode(new DirectoryInfo(file[i]).Name);
                        e.Node.Nodes.Add(fileNode);
                    }
                }

            }
            catch (Exception)
            {
            }

        }

        private void smallIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.SmallIcon;
        }

        private void detailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.Details;
        }

        private void largeIconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.LargeIcon;
        }

        private void listToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.List;
        }

        private void tileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.View = View.Tile;
        }

        private void openInNewWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 f = new Form1();
            f.Show();
        }


        private void FillTreeNode(TreeNode dirNode, string path)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(path);

                foreach (string dir in dirs)
                {
                    TreeNode treeN = new TreeNode(dir.Remove(0, dir.LastIndexOf("\\") + 1));
                    dirNode.Nodes.Add(treeN);
                }

                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    TreeNode fileNode = new TreeNode(new DirectoryInfo(file).Name);
                    dirNode.Nodes.Add(fileNode);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
