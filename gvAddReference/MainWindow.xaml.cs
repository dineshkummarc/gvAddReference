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
            logger.Info("gvAddReference started at: " + DateTime.Now.ToString());
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            String filePath = filePathBox.Text;
            String pagePath = pagePathBox.Text;

            logger.Debug("Starting to add references...");
            // For each .aspx or html file in the selected pagePath, or the single file selected, insert the tag
            // Read the file as one string.
            //try
            //{
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

                // Check for aspx or html pages in the directory, or the file
                Regex fileExtension = new Regex("^.+(.aspx|.ASPX|.html|.HTML)$");
                if (!directoryCheckBox.IsChecked.Value)
                {
                    if (File.Exists(pagePath))
                    {
                        if (fileExtension.IsMatch(pagePath))
                        {
                            addTagToPage(filePath, pagePath);
                        }
                    }
                }
                else 
                {
                    // Use just the directory path and process all directories and files
                    pagePath = pagePath.Substring(0, pagePath.LastIndexOf('\\'));
                    if (Directory.Exists(pagePath))
                    {
                        // Call add tag to page for each page file
                        processDirectory(pagePath, filePath, fileExtension);

                        // Need to fix the tag to use a different relative url for subdir's
                        foreach (string dir in Directory.GetDirectories(pagePath))
                        {
                            // Go 3 folders deep looking for files
                            // this is dumb, change this
                            processDirectory(dir, filePath, fileExtension);
                            foreach (string deeperDir in Directory.GetDirectories(dir))
                            {
                                processDirectory(deeperDir, filePath, fileExtension);
                                foreach (string deepestDir in Directory.GetDirectories(deeperDir))
                                {
                                    processDirectory(deepestDir, filePath, fileExtension);
                                }
                            }
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
                    statusText.Text = "References added successfully! See ChangeLog.txt";
                else statusText.Text = "No changes were made to any files. See Log.txt";
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
                htmlTag = "        <script type=\"text/javascript\" src=\"" + filePath + "\" ></script>";
            }
            else
            {
                htmlTag = "        <link rel=\"stylesheet\" type=\"text/css\" href=\"" + filePath + "\" />";
            }
            // Local paths use backslash, web paths use forward slash so convert
            return htmlTag.Replace("\\", "/");
        }

        // Calls addTagToPage on every file in the directory that matches the regex
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
        private void addTagToPage(String scriptPath, String fileName)
        {

            String scriptTag;
            // Read the file for manipulation
            System.IO.StreamReader readFile = new System.IO.StreamReader(fileName);
            var allLines = File.ReadAllLines(fileName).ToList();
            readFile.Close();

            // Output buffer for holding the relative path
            StringBuilder str = new StringBuilder(260);

            // Make the path we add in the script a relative path to the page
            // Function imported from dll at the bottom of this class
            bool madeRelative = PathRelativePathTo(str, fileName, FileAttributes.Normal, scriptPath, FileAttributes.Normal);
            if (madeRelative)
                scriptTag = getScriptTag(str.ToString());
            else
            {
                // Uhh, problem getting the relative path
                // just use the script fileName for the tag
                scriptTag = getScriptTag(scriptPath);
            }
            
            int index = allLines.FindIndex(i => i.ToUpper().Contains("</HEAD>"));
            if (index == -1)
            {
                // No closing head tag, try just before the closing body tag
                index = allLines.FindIndex(i => i.ToUpper().Contains("</BODY>"));
                if (index == -1)
                {
                    //No head or body tag, error message, log error
                    logger.Debug("No head or body tag found in file " + fileName);
                    logger.Info("File: " + fileName + " | Page has no head or body tags | No changes were made");
                    return;
                }
            }

            if (File.Exists(fileName + ".vb"))
            {
                // Read the file's codebehind file to check if it inherits from the Base Page
                System.IO.StreamReader codeFile = new System.IO.StreamReader(fileName + ".vb");
                var codeLines = File.ReadAllLines(fileName + ".vb").ToList();
                codeFile.Close();

                // If this page inherits from the base page don't add the script here, we can add it there
                // Also don't add the reference if it already exists in the file
                int line = 0;
                if ((line = (codeLines.FindIndex(i => i.ToUpper().Contains("INHERITS BASEPAGE")
                        || i.ToUpper().Contains(System.IO.Path.GetFileName(fileName).ToUpper())))) > 0)
                {
                    logger.Debug("File " + fileName + " inherits from the BasePage or already has the analytics script block, no changes were made");
                    logger.Info("File: " + fileName + " | On Line number: " + line + " | Page inherits from BasePage | No changes were made");
                    return;
                }
            }

            //Insert the script tag just before the closing head tag
            allLines.Insert(index, scriptTag);

            // Write the file back, write what we did to the change log
            File.WriteAllLines(fileName, allLines.ToArray());
            filesChanged = true;
            logger.Info("File changed: " + fileName + " | HTML Tag: " + scriptTag);
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

        // Imported function for creating relative paths between two files
        // This gives us a path to use in the script tag that will work on the server
        [DllImport("shlwapi.dll", CharSet = CharSet.Auto)]
        static extern bool PathRelativePathTo(
             [Out] StringBuilder pszPath,
             [In] string pszFrom,
             [In] FileAttributes dwAttrFrom,
             [In] string pszTo,
             [In] FileAttributes dwAttrTo
        );

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Should this close or clear the form? bad name
            gvMainWindow.Close();
        }
    }
}
