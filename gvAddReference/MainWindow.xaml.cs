﻿using System;
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
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using NLog;

namespace gvAddReference
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Boolean filesChanged = false;
        List<String> changeLog = new List<String>();
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            String filePath = filePathBox.Text;
            String pagePath = pagePathBox.Text;

            // For each .aspx or html file in the selected pagePath, or the single file selected, insert the tag
            // Read the file as one string.
            //try
            //{
                statusText.Text = "Checking script file...";
                if(!File.Exists(filePath))
                {
                    // File does not exist error message
                    return;
                }
                else if (Directory.Exists(filePath))
                {
                    // Pick a file not a directory for the script message
                    return;
                }

                statusText.Text = "Looking for pages...";
                // Check for aspx or html pages in the directory, or the file
                Regex fileExtension = new Regex("^.+(.aspx|.ASPX|.html|.HTML)$");
                if (!directoryCheckBox.IsChecked.Value)
                {
                    if (File.Exists(pagePath))
                    {
                        if (fileExtension.IsMatch(pagePath))
                        {
                            statusText.Text = "Adding script to page...";
                            addTagToPage(filePath, pagePath);
                        }
                    }
                }
                else 
                {
                    // Use just the directory path
                    pagePath = pagePath.Substring(0, pagePath.LastIndexOf('\\'));
                    if (Directory.Exists(pagePath))
                    {
                        statusText.Text = "Looking for files in directory...";
                        // Call add tag to page for each page file
                        processDirectory(pagePath, filePath, fileExtension);

                        // Need to fix the tag to use a different relative url for subdir's
                        foreach (string dir in Directory.GetDirectories(pagePath))
                        {
                            processDirectory(dir, filePath, fileExtension);
                        }
                    }
                    else
                    {
                        // Not found error message
                        return;
                    }
                }

                // Based on if filesChanged, give the user a message
                if (filesChanged)
                    statusText.Text = "References added successfully!";
                else statusText.Text = "No changes were made to any files";
            //}
            //catch (Exception ex)
            //{
                //Check for file not found exception and give message
            //    Console.Write(ex.Message);
            //}
            //finally
            //{

            //}
        }

        private String getScriptTag(String filePath)
        {
            String htmlTag;
            // Set the html tag string based on the type selected
            if (jsRadioButton.IsChecked.Value)
            {
                htmlTag = "<script type=\"text/javascript\" src=\"" + filePath + "\" ></script>";
            }
            else
            {
                htmlTag = "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + filePath + "\" />";
            }
            return htmlTag;
        }

        private void processDirectory(String pagePath, String filePath, Regex fileExtension)
        {
            string[] fileEntries = Directory.GetFiles(pagePath);
            foreach (string fileName in fileEntries)
            {
                if (fileExtension.IsMatch(fileName))
                {
                    addTagToPage(filePath, fileName);
                }
            }
        }

        // Adds the script tag in the string to the page just before the closing head tag
        private void addTagToPage(String filePath, String fileName)
        {
            String scriptTag;
            // Read the file for manipulation
            System.IO.StreamReader readFile = new System.IO.StreamReader(fileName);
            var allLines = File.ReadAllLines(fileName).ToList();
            readFile.Close();

            // Buffer for holding the relative path
            StringBuilder str = new StringBuilder(260);
            // Make the path we add in the script a relative path to the page
            // Function imported from dll at the bottom of this class
            bool madeRelative = PathRelativePathTo(str, fileName, FileAttributes.Normal, filePath, FileAttributes.Normal);
            if (madeRelative)
                scriptTag = getScriptTag(str.ToString());
            else
            {
                // Uhh, problem getting the relative path
                // just use the script fileName for the tag
                scriptTag = getScriptTag(fileName);
            }
            
            int index = allLines.FindIndex(i => i.Contains("</head>"));
            allLines.Insert(index, scriptTag);
            File.WriteAllLines(fileName, allLines.ToArray());
            filesChanged = true;
            logger.Info("File changed: " + fileName + " | Script reference added: " + filePath + " | Relative reference: " + str.ToString() +
                 " | HTML Tag: " + scriptTag);
        }

        private void filePathButton_Click(object sender, RoutedEventArgs e)
        {
            // Show a file select dialog for .js and .css files
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
 
            ofd.Filter = "Script files (*.js, *.css)|*.js*;*.css|All Files|*.*";
 
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filePathBox.Text = ofd.FileName;
            }

        }

        private void pageBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            // Show a file select dialog for .aspx and .html files
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = false;
            ofd.CheckFileExists = false;
            ofd.CheckPathExists = true;

            ofd.Filter = "Aspx + HTML Pages (*.aspx, *.html)|*.aspx*;*.htm;*.html|All Files|*.*";
 
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                pagePathBox.Text = ofd.FileName;
            }
        }

        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        static extern bool PathRelativePathTo(
             [Out] StringBuilder pszPath,
             [In] string pszFrom,
             [In] FileAttributes dwAttrFrom,
             [In] string pszTo,
             [In] FileAttributes dwAttrTo
        );
    }
}
