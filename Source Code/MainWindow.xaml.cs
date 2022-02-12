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
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media.Animation;

namespace ParabellumDeveloperTools
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}
		/// <summary>
		/// "About" Window click event.
		/// </summary>
		private void ButtonAboutEventClick(object sender, RoutedEventArgs e)
		{
			MessageBox.Show("Parabellum Developing Tools.\nBulid 5.\nMade for M&B: Warband Parabellum mod.\nMade by Eldar, 2019.\n.NET Framework 4.7.2.\n7-Zip Copyright (C) 1999-2019 Igor Pavlov.");
		}

		private void ButtonAssembleCTEReleaseEventClick(object sender, RoutedEventArgs e)
		{
			WindowAssembleCTERelease.Show();
		}
		public ParabellumDeveloperTools.AssembleCTERelease WindowAssembleCTERelease = new ParabellumDeveloperTools.AssembleCTERelease();

		private void MainWindowOnClose(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Application.Current.Shutdown();
		}
	}
}