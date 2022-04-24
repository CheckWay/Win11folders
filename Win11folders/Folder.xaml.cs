using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using WindowsShortcutFactory;

namespace Win11folders
{
    /// <summary>
    /// Interaction logic for Folder.xaml
    /// </summary>
    public partial class Folder : Window
    {
        private static int borderToTable = 10;
        private static int textHeight = 35;
        private static int iconSize = 64;
        private static int radiusBorder = 30;
        private static double folderOpacity = 0.5;
        private static int borderThickness = 3;
        private static double borderOpacity = 1;
        private static int tableToIcon = 15;
        public static int countRows = 3;
        public static int countColumn = 3;
        private static int shortcutBorderThickness = 3;
        private static int shortcutHeight = textHeight + iconSize + shortcutBorderThickness * 4;
        private static int shortcutWidth = iconSize + tableToIcon * 2 + shortcutBorderThickness * 4;
        private static int windowHeight = borderToTable * 2 + shortcutHeight * countRows;
        private static int windowWidth = borderToTable * 2 + shortcutWidth * countColumn;
        private static bool _canMove = false;
        private static int miniatureHeight = 80;
        private static int miniatureWidth = 80;
        private static int miniatureIconSize = 16;

        public Folder()
        {
            InitializeComponent();
            InitialisationFolder();
            System.Diagnostics.Trace.WriteLine($"Height = {textHeight + iconSize}");
            System.Diagnostics.Trace.WriteLine($"Width = {iconSize + tableToIcon * 2}");
        }

        private void MainWindow_OnMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            if (_canMove)
            {
                this.DragMove();
            }
        }

        private void InitialisationFolder()
        {
            InitialisationBorder(windowHeight, windowWidth, radiusBorder, folderOpacity, 
                borderThickness, borderOpacity, countColumn, countRows, borderToTable);
        }

        /// <summary>
        /// Initializes folder boundaries
        /// </summary>
        /// <param name="folderH">Folder height</param>
        /// <param name="folderW">Folder width</param>
        /// <param name="folderR">Folder radius</param>
        /// <param name="folderO">Folder opacity</param>
        /// <param name="borderT">Folder border thickness</param>
        /// <param name="borderO">Folder border opacity</param>
        private void InitialisationBorder(int folderH, int folderW, int folderR, double folderO,
            int borderT, double borderO, int _countColums, int _countRows, double _margin)
        {
            this.Height = folderH;
            this.Width = folderW;
            Background.RadiusX = folderR;
            Background.RadiusY = folderR;
            Background.Opacity = folderO;
            Background.Stroke = new SolidColorBrush(Colors.Black);
            Background.Stroke.Opacity = borderO;
            Background.StrokeThickness = borderT;
            InitializeTable( _countColums, _countRows, _margin);
        }

        private void InitializeTable(int _countColums, int _countRows, double _margin)
        {
            Table.Columns = _countColums;
            Table.Rows = _countRows;
            Table.Margin = new Thickness(_margin);
        }

        public void ResizeRight()
        {
            countColumn++;
            windowWidth = borderToTable * 2 + shortcutWidth * countColumn;
            this.Width = windowWidth;
        }

        public void ResizeDown()
        {
            countRows++;
            windowHeight = borderToTable * 2 + shortcutHeight * countRows;
            this.Height = windowHeight;
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                this.Close();
            else if (e.Key == Key.F1)
                _canMove = !_canMove;
        }

        private void Background_OnDragLeave(object sender, DragEventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Leave");
        }

        private void Background_OnDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
            foreach (var f in files)
            {
                string[] fullFilePath = f.Split((char) 0x5C);
                string fileName = fullFilePath[^1].Split('.')[0];
                string fileFormat = fullFilePath[^1].Split('.')[^1];
                switch (fileFormat)
                {
                    case "lnk":
                    {
                        WindowsShortcut shortcutToParse = WindowsShortcut.Load(f);
                        string iconLocation = shortcutToParse.IconLocation.Path;
                        ShortcutUI shortcut = new ShortcutUI();
                        shortcut.folder = this;
                        if (iconLocation != "" && iconLocation.Split((char)0x5C)[^1].Split('.')[^1] == "ico")
                        {
                            shortcut.InitProgramShortcut(iconSize, tableToIcon, textHeight, 
                                iconLocation, shortcutToParse.Path, fileName, shortcutBorderThickness);
                        }
                        else
                        {
                            shortcut.InitProgramShortcut(iconSize, tableToIcon, textHeight,
                                System.Drawing.Icon.ExtractAssociatedIcon(shortcutToParse.Path), shortcutToParse.Path,
                                fileName, shortcutBorderThickness);
                        }
                        Table.Children.Add(shortcut);
                        break;
                    }
                    case "url":
                    {
                        // Parse Url file
                        string Url = String.Empty, pathToIcon = String.Empty;
                        using (FileStream fileStream = new FileStream(f, FileMode.Open))
                        using (BinaryReader binaryReader = new BinaryReader(fileStream))
                        {
                            byte[] bin = binaryReader.ReadBytes(Convert.ToInt32(fileStream.Length));
                            string dataToParse = Encoding.Default.GetString(bin);
                            char[] separators = {(char) 0x0D, (char) 0x0A};
                            var data = dataToParse.Split(separators);
                            foreach (var d in data)
                            {
                                if (d.Contains("URL"))
                                    Url = d.Split('=')[1];
                                else if (d.Contains("IconFile"))
                                    pathToIcon = d.Split('=')[1];
                            }
                        }

                        ShortcutUI shortcut = new ShortcutUI();
                        shortcut.folder = this;
                        if (pathToIcon != "" && pathToIcon.Split((char)0x5C)[^1].Split('.')[^1] == "ico")
                        {
                            shortcut.InitUrlShortcut(iconSize, tableToIcon, textHeight, 
                                pathToIcon, Url, fileName, shortcutBorderThickness);
                        }
                        else
                        {
                            shortcut.InitUrlShortcut(iconSize, tableToIcon, textHeight,
                                System.Drawing.Icon.ExtractAssociatedIcon(pathToIcon), Url, fileName, shortcutBorderThickness);
                        }
                        Table.Children.Add(shortcut);
                        break;
                    }
                }
            }
        }

        private void Background_OnDragOver(object sender, DragEventArgs e)
        {
            /*var point = e.GetPosition(Table);
            point.X %= countColumn;
            point.Y %= countRows;
            System.Diagnostics.Trace.WriteLine(point);*/
        }
    }
}