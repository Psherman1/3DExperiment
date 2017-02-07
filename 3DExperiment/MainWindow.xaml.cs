using System;
using System.Collections.Generic;
using System.Linq;
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

namespace _3DExperiment
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			//set the window dimensions to the screen dimensions
			Width = SystemParameters.PrimaryScreenWidth / 2;
			Height = SystemParameters.PrimaryScreenHeight / 2;
			Left = 0;
			Top = 0;
			DataContext = new MainWindowContext();
		}
	}
}
