using System;
using System.Windows;
using System.ComponentModel;
using System.Reflection;
using System.Net;
using System.IO;
using System.Xml;

namespace PKHL.ProjectSweeper
{
    /// <summary>
    /// Interaction logic for AboutBox.xaml
    /// </summary>
    public partial class AboutBox : Window
    {
        private string AppVersion = null;
        private Assembly TheAssembly = null;
        private readonly string NotListed = LocalizationProvider.GetLocalizedValue<string>("ABOUT_RSLT_Txt1");
        private readonly string NewVersion = LocalizationProvider.GetLocalizedValue<string>("ABOUT_RSLT_Txt2");
        private readonly string Up2Date = LocalizationProvider.GetLocalizedValue<string>("ABOUT_RSLT_Txt3");
        private readonly string NoServer = LocalizationProvider.GetLocalizedValue<string>("ABOUT_RSLT_Txt4");

        public AboutBox(Assembly a)
        {
            InitializeComponent();

            TheAssembly = a;
            this.Title = LocalizationProvider.GetLocalizedValue<string>("ABOUT_Title");
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format("Version {0}.{1}.{2}.{3}", MajorVersion, MinorVersion, Build, Revision);
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;
            AppVersion = Build;
            this.textBoxDescription.Text = LocalizationProvider.GetLocalizedValue<string>("ABOUT_Desc_Txt1"); //Checking for updates....
            setLogo();
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = TheAssembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(TheAssembly.CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return TheAssembly.GetName().Version.ToString();
            }
        }

        public string MajorVersion
        {
            get
            {
                return TheAssembly.GetName().Version.Major.ToString();
            }
        }

        public string MinorVersion
        {
            get
            {
                return TheAssembly.GetName().Version.Minor.ToString();
            }
        }

        public string Build
        {
            get
            {
                return TheAssembly.GetName().Version.Build.ToString();
            }
        }

        public string Revision
        {
            get
            {
                return TheAssembly.GetName().Version.Revision.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = TheAssembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = TheAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = TheAssembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = TheAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void setLogo()
        {
            Stream s = TheAssembly.GetManifestResourceStream("PKHL.ProjectSweeper.Resources.pkh logo vertical.jpg");
            System.Windows.Media.Imaging.BitmapImage img = new System.Windows.Media.Imaging.BitmapImage();
            img.BeginInit();
            img.StreamSource = s;
            img.EndInit();
            logoImage.Source = img;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            TestButton.Visibility = Visibility.Visible;
            this.Title += " - Debug Mode";
#endif
        }
    }
}
