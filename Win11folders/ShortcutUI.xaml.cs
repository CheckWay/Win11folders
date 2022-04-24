using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Interop;
using System.Windows.Media;
using Image = System.Windows.Controls.Image;


namespace Win11folders;

enum ShortcutType
{
    Lnk,
    Url
}

public partial class ShortcutUI : UserControl
{
    private string path;
    private ShortcutType shortcutType;
    public Folder folder { get; set; }

    public ShortcutUI()
    {
        InitializeComponent();
    }

    public void InitProgramShortcut(int iconSize, int tableToIcon, int textHeight, string pathToIcon,
        string pathToProgram, string programName, int borderThickness)
    {
        InitShortcut(iconSize, textHeight, tableToIcon, programName, borderThickness);
        ProgramIcon.Source = new BitmapImage(new Uri(pathToIcon));
        shortcutType = ShortcutType.Lnk;
        path = pathToProgram;
    }
    
    public void InitProgramShortcut(int iconSize, int tableToIcon, int textHeight, Icon icon,
        string pathToProgram, string programName, int borderThickness)
    {
        InitShortcut(iconSize, textHeight, tableToIcon, programName, borderThickness);
        Icon iconToShow = new Icon(icon, iconSize, iconSize);
        ProgramIcon.Source = Imaging.CreateBitmapSourceFromHIcon(iconToShow.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        shortcutType = ShortcutType.Lnk;
        path = pathToProgram;
    }

    public void InitUrlShortcut(int iconSize, int tableToIcon, int textHeight, string pathToIcon, string url,
        string urlName, int borderThickness)
    {
        InitShortcut(iconSize, textHeight, tableToIcon, urlName, borderThickness);
        ProgramIcon.Source = new BitmapImage(new Uri(pathToIcon));
        shortcutType = ShortcutType.Url;
        path = url;
    }
    
    public void InitUrlShortcut(int iconSize, int tableToIcon, int textHeight, Icon icon, string url, 
        string urlName, int borderThickness)
    {
        InitShortcut(iconSize, textHeight, tableToIcon, urlName, borderThickness);
        Icon iconToShow = new Icon(icon, iconSize, iconSize);
        ProgramIcon.Source = Imaging.CreateBitmapSourceFromHIcon(iconToShow.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());        
        shortcutType = ShortcutType.Url;
        path = url;
    }

    private void InitShortcut(int iconSize, int textHeight, int tableToIcon, string shortcutName, int borderThickness)
    {
        ResizeImage(ref ProgramIcon, iconSize);
        BorderThickness = new Thickness(borderThickness);
        Border.CornerRadius = new CornerRadius(5);
        Shortcut.Height = iconSize + textHeight + borderThickness * 4;
        Shortcut.Width = iconSize + tableToIcon * 2 + + borderThickness * 4;
        ProgramName.Text = shortcutName;
        ProgramName.Margin = new Thickness(borderThickness * 2, iconSize + borderThickness * 4, borderThickness * 2, 0);
        ProgramIcon.Margin = new Thickness(tableToIcon, borderThickness * 2, tableToIcon, textHeight);
    }
    
    private void ResizeImage(ref Image image, int imageSize)
    {
        image.Height = imageSize;
        image.Width = imageSize;
    }

    private void ShortcutUI_OnDrop(object sender, DragEventArgs e)
    {
        string[] file = (string[]) e.Data.GetData(DataFormats.FileDrop);
        ProgramIcon.Source = new BitmapImage(new Uri(file[0]));
        Border.BorderThickness = new Thickness(0);
    }

    private void ShortcutUI_OnDragOver(object sender, DragEventArgs e)
    {
        /*string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);
        var extension = System.IO.Path.GetExtension(files[0]);*/
        Border.BorderThickness = new Thickness(3);
    }

    private void ShortcutUI_OnDragLeave(object sender, DragEventArgs e)
    {
        Border.BorderThickness = new Thickness(0);
    }

    private void ShortcutUI_OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        Border.BorderThickness = new Thickness(0);
        switch (shortcutType)
        {
            case ShortcutType.Lnk:
                Process.Start(path);
                break;
            case ShortcutType.Url:
                switch (path.Split(':')[0])
                {
                    case "https":
                    {
                        Process.Start(path);
                        break;
                    }
                    case "steam":
                    {
                        Process.Start(@"C:\Program Files (x86)\Steam\steam.exe", path);
                        break;
                    }
                    case "com.epicgames.launcher":
                    {
                        Process.Start(@"C:\Program Files (x86)\Epic Games\Launcher\Portal\Binaries\Win64\EpicGamesLauncher.exe", path);
                        break;
                    }
                }
                break;
        }    
    }

    private void ShortcutUI_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        Border.BorderThickness = new Thickness(3);
    }

    private void ShortcutUI_OnMouseLeave(object sender, MouseEventArgs e)
    {
        Border.BorderThickness = new Thickness(0);
    }
}