using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace ProgramManager
{
    public partial class Form1 : Form
    {
        public static string pathh;
        public Form1()
        {
            InitializeComponent();
            UmplereCopac();
            this.treeView1.NodeMouseClick += new TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
        }
        public static TreeNode nod;
        private void UmplereCopac()
        {
            string[] drives = Environment.GetLogicalDrives();
            foreach (string folder in drives)
            {
                nod = new TreeNode(folder);
                nod.Tag = folder;
                nod.ImageIndex = 2;
                nod.Tag = folder;
                treeView1.Nodes.Add(nod);
                nod.Nodes.Add(new TreeNode("?"));
            }
            treeView1.BeforeExpand += new TreeViewCancelEventHandler(treeView1_BeforeExpand);

        }

        void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if ((e.Node.Nodes.Count == 1) && (e.Node.Nodes[0].Text == "?"))
            {
                Obtinere(e.Node);
            }
        }

        public static string path;
        private TreeNode Obtinere(TreeNode nod)
        {
            string[] foldere;
            path = (string)nod.Tag;
            nod.Nodes.Clear();
            foldere = System.IO.Directory.GetDirectories(path);
            for (int i = 0; i < foldere.Length; i++)
            {
                TreeNode x = new TreeNode(foldere[i].Substring(foldere[i].LastIndexOf('\\') + 1));
                x.ImageIndex = 0;
                x.Tag = foldere[i];
                nod.Nodes.Add(x);
                x.Nodes.Add(new TreeNode("?"));
            }

            string[] fisiere = System.IO.Directory.GetFiles(path);
            for (int i = 0; i < fisiere.Length; i++)
            {
                TreeNode y = new TreeNode(System.IO.Path.GetFileName(fisiere[i]));
                y.Tag = fisiere[i];
                y.ImageIndex = 1;
                nod.Nodes.Add(y);
            }

            return nod;
        }
        public static TreeNode newSelected;
        void treeView1_NodeMouseClick(object sender,
    TreeNodeMouseClickEventArgs e)
        {
            newSelected = e.Node;
        }

        private void NewFolder_Click(object sender, EventArgs e)
        {
            int j = 0;
            while (j < 2)
            {
                string[] foldere;
                string path = (string)treeView1.SelectedNode.Tag;
                foldere = System.IO.Directory.GetDirectories(path);
                Directory.CreateDirectory(Path.Combine(newSelected.Tag as string, "New Folder"));
                for (int i = 0; i < foldere.Length; i++)
                {
                    TreeNode x = new TreeNode(foldere[i].Substring(foldere[i].LastIndexOf('\\') + 1));
                    x.ImageIndex = 0;
                    x.Tag = foldere[i];
                    treeView1.SelectedNode.Nodes.Add(x);
                }
                j++;
            }

        }
        public void Delete_Click(object sender, EventArgs e)
        {
            path = (string)treeView1.SelectedNode.Tag;
            if(!System.IO.Directory.Exists(treeView1.SelectedNode.FullPath))
            {
                File.Delete(Path.Combine(newSelected.Tag as string));
                if (treeView1.SelectedNode != null)
                {
                    if (treeView1.SelectedNode.Parent == null)
                    {
                        treeView1.Nodes.Remove(treeView1.SelectedNode);
                    }
                    else
                    {
                        treeView1.SelectedNode.Parent.Nodes.Remove(treeView1.SelectedNode);
                    }
                }
            }
            else
            {
                Directory.Delete(Path.Combine(newSelected.Tag as string),true);
                if (treeView1.SelectedNode != null)
                {
                    if (treeView1.SelectedNode.Parent == null)
                    {
                        treeView1.Nodes.Remove(treeView1.SelectedNode);
                    }
                    else
                    {
                        treeView1.SelectedNode.Parent.Nodes.Remove(treeView1.SelectedNode);
                    }
                }
            }
        }
        
        private void Rename_Click(object sender, EventArgs e)
        {
            string newName;
            newName = textBox1.Text.ToString();
            if (!System.IO.Directory.Exists(treeView1.SelectedNode.FullPath))
            {

                path = (string)treeView1.SelectedNode.Tag;
               
                try
                {
                    treeView1.SelectedNode.Text = newName;
                    path = (string)treeView1.SelectedNode.Tag;
                    FileInfo fi = new FileInfo(path);
                    fi.MoveTo(fi.Directory + "\\" + newName);
                }
                finally
                {

                }
            }
            else
            {
                 newName = textBox1.Text.ToString(); ;
                try
                {
                    treeView1.SelectedNode.Text = newName;
                    path = (string)treeView1.SelectedNode.Tag;
                    DirectoryInfo di = new DirectoryInfo(path);
                    di.MoveTo(di.Parent + "\\" + newName);
                }
                finally
                {
                   
                }
            }
             

    }
        public static TreeNode cpnode;
        private void Copy_Click(object sender, EventArgs e)
        {
            cpnode = treeView1.SelectedNode;
        }
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException();
       
            }
            DirectoryInfo[] dirs = dir.GetDirectories(); 
            Directory.CreateDirectory(destDirName);
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        private void Paste_Click(object sender, EventArgs e)
        {
            if (!System.IO.Directory.Exists(treeView1.SelectedNode.FullPath))
            {
                File.Copy(Path.Combine((string)cpnode.Parent.Tag, (string)cpnode.Tag), Path.Combine((string)treeView1.SelectedNode.Tag, (string)cpnode.Tag), true);
            }
            else
            {
                DirectoryCopy((string)cpnode.Parent.Tag, (string)treeView1.SelectedNode.Tag, true);
            }
            TreeNode NodeClone = (TreeNode)cpnode.Clone();
            treeView1.SelectedNode.Nodes.Add(NodeClone);

        }

        private void OpenTxt_Click(object sender, EventArgs e)
        {
            var fileToOpen = (string)treeView1.SelectedNode.Tag;
            var process = new Process();
            process.StartInfo = new ProcessStartInfo()
            {
                UseShellExecute = true,
                FileName = fileToOpen
            };

            process.Start();
            
        }
    }
}


