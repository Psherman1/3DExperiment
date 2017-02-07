using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3DExperiment.Presentation;
using System.Windows;
using GalaSoft.MvvmLight.Command;

namespace _3DExperiment
{
	/// <summary>
	/// 
	/// </summary>
	public class MainWindowContext : ObservableObject
	{
		/// <summary>
		/// 
		/// </summary>
		public MainWindowContext()
		{
			
		}

		private RelayCommand _exitCommand;
		public RelayCommand ExitCommand
		{
			get { return _exitCommand ?? (_exitCommand = new RelayCommand(Exit)); }
		}

		private RelayCommand _settingsCommand;
		public RelayCommand SettingsCommand
		{
			get { return _settingsCommand ?? (_settingsCommand = new RelayCommand(ShowSettings)); }
		}

		private RelayCommand _launchCommand;
		public RelayCommand LaunchCommand
		{
			get { return _launchCommand ?? (_launchCommand = new RelayCommand(Launch3D)); }
		}

		/// <summary>
		/// 
		/// </summary>
		void Exit()
		{
			Application.Current.MainWindow.Close();
		}

		/// <summary>
		/// 
		/// </summary>
		void ShowSettings()
		{
			//TODO need a way to swap- xml views, maybe use a content presenter
		}

		/// <summary>
		/// 
		/// </summary>
		void Launch3D()
		{
			//TODO need to make a direct3D object and get rendering up and running...
			using (var window = new RenderWindow())
				window.Run();
		}
	}
}
