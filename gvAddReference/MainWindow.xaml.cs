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
using System.Windows.Forms;
using System.IO;

namespace gvAddReference
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Boolean filesChanged = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            String filePath = filePathBox.Text;
            String pagePath = pagePathBox.Text;
            String htmlTag = "";

            // Set the html tag string based on the type selected
            if (jsRadioButton.IsChecked.Value)
            {
                htmlTag = "<script type=\"text/javascript\" src=\"" + filePath + "\" ></script>";
            }
            else
            {
                htmlTag = "<link rel=\"stylesheet\" type=\"text/css\" href=\"" + filePath + "\" />";
            }

            // For each .aspx or html file in the selected pagePath, or the single file selected, insert the tag
            // Read the file as one string.
            //try
            //{
                statusText.Text = "Checking script file...";
                if(!File.Exists(filePath))
                {
                    // File does not exist error message
                }
                else if (Directory.Exists(filePath))
                {
                    // Pick a file not a directory for the script message
                }

                statusText.Text = "Looking for pages...";
                // Check for aspx or html pages in the directory, or the file
                System.Text.RegularExpressions.Regex fileExtension = new System.Text.RegularExpressions.Regex("^.+(.aspx|.ASPX|.html|.HTML)$");
                if (!directoryCheckBox.IsChecked.Value)
                {
                    if (File.Exists(pagePath))
                    {
                        if (fileExtension.IsMatch(pagePath))
                        {
                            statusText.Text = "Adding script to page...";
                            addTagToPage(htmlTag, pagePath);
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
                        processDirectory(pagePath, htmlTag, fileExtension);

                        // Need to fix the tag to use a different relative url for subdir's
                        foreach (string dir in Directory.GetDirectories(pagePath))
                        {
                            processDirectory(dir, htmlTag, fileExtension);
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

        private void processDirectory(String pagePath, String htmlTag, System.Text.RegularExpressions.Regex fileExtension)
        {
            string[] fileEntries = Directory.GetFiles(pagePath);
            foreach (string fileName in fileEntries)
            {
                if (fileExtension.IsMatch(fileName))
                {
                    addTagToPage(htmlTag, fileName);
                }
            }
        }

        // Adds the script tag in the string to the page just before the closing head tag
        private void addTagToPage(String scriptTag, String fileName)
        {
            // Read the file for manipulation
            System.IO.StreamReader readFile = new System.IO.StreamReader(fileName);
            var allLines = File.ReadAllLines(fileName).ToList();
            readFile.Close();

            int index = allLines.FindIndex(i => i.Contains("</head>"));
            allLines.Insert(index, scriptTag);
            File.WriteAllLines(fileName, allLines.ToArray());
            filesChanged = true;
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
    }
}
