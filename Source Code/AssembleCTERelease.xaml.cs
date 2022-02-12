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
//using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO;
using System.Diagnostics;
using System.Threading;
using WpfAnimatedGif;

namespace ParabellumDeveloperTools
{
	/// <summary>
	/// Interaction logic for AssembleCTERelease.xaml
	/// </summary>
	public partial class AssembleCTERelease : Window
	{
		public AssembleCTERelease()
		{
			InitializeComponent();
		}
		public bool RemoveCopies = true;
		public bool RemovePsdFiles = true;
		public bool RemoveMorghsBackups = true;
		public bool CreateModFolder = true;
		public bool CreateModArchive = true;
		/// <summary>
		/// Assembling CTE mod release. Async.
		/// </summary>
		private async void ButtonAssembleEventClick(object sender, RoutedEventArgs e)
		{
			DirectoryInfo dir = new DirectoryInfo(TextBoxModPath.Text);
			if (!dir.Exists) MessageBox.Show("Source directory does not exist or could not be found: " + TextBoxModPath.Text);
			else
			{
			LoadingIcon1.Opacity = 1;
			var LoadingIcon1Controller = ImageBehavior.GetAnimationController(LoadingIcon1);
			LoadingIcon1Controller.Play();
			LabelAssemblingState.Content = "Assembling...";
			string ModPath = TextBoxModPath.Text;
			string ModOriginalPath = TextBoxModPath.Text;
			do
			{
				ModPath = ModPath.Remove(ModPath.Length - 1, 1); //Clear mod folder name in path, whatever it is
			} while (!ModPath.EndsWith(@"\"));
			ModPath = ModPath.Insert(ModPath.Length, @"Parabellum CTE v" + TextBoxVersionNumber.Text); //Create new mod folder name containing version number
			if (Directory.Exists(ModPath)) {
				LabelAssemblingState.Content = "Assembling: Deleting old mod folder...";
				Directory.Delete(ModPath, true); } //Delete mod folder with created name, if it is already exists
			LabelAssemblingState.Content = "Assembling: Copying mod folder...";
			await Task.Run(() => DirectoryCopy(ModOriginalPath, ModPath, true)); //Copy mod folder with created name
			if (File.Exists(ModPath + @"\reference.brf")) File.Delete(ModPath + @"\reference.brf"); //Delete reference.brf if it is exists
			LabelAssemblingState.Content = "Assembling: Removing file copies...";
			if (RemoveCopies) await Task.Run(() => RemoveFiles(ModPath, "*копия*"));
			LabelAssemblingState.Content = "Assembling: Removing .psd files...";
			if (RemovePsdFiles) await Task.Run(() => RemoveFiles(ModPath, "*.psd"));
			LabelAssemblingState.Content = "Assembling: Removing Morgh's backups...";
			if (RemoveMorghsBackups) await Task.Run(() => RemoveFiles(ModPath, "*_*-*-*_*h*m*s.txt"));
			if (File.Exists(ModPath + ".7z"))
			{
				LabelAssemblingState.Content = "Assembling: Removing old mod archive...";
				await Task.Run(() => File.Delete(ModPath + ".7z"));
			} //Delete mod archive if it is exists
			LabelAssemblingState.Content = "Assembling: Creating mod 7z archive...";
			if (CreateModArchive) await Task.Run(() => Create7zArchiveFromFolder(ModPath + ".7z", ModPath));
			LabelAssemblingState.Content = "Assembling: Removing mod folder...";
			if (!CreateModFolder) await Task.Run(() => Directory.Delete(ModPath, true));
			LabelAssemblingState.Content = "Assembling: Done.";
			LoadingIcon1Controller.GotoFrame(0);
			LoadingIcon1Controller.Pause();
			LoadingIcon1.Opacity = 0;
			}
		}

		private void CheckBoxRemoveCopiesEventChecked(object sender, RoutedEventArgs e)
		{
			RemoveCopies = true;
		}

		private void CheckBoxRemoveCopiesEventUnchecked(object sender, RoutedEventArgs e)
		{
			RemoveCopies = false;
		}
		private void CheckBoxRemovePsdFilesChecked(object sender, RoutedEventArgs e)
		{
			RemovePsdFiles = true;
		}
		private void CheckBoxRemovePsdFilesUnchecked(object sender, RoutedEventArgs e)
		{
			RemovePsdFiles = false;
		}
		private void CheckBoxRemoveMorghsBackupsChecked(object sender, RoutedEventArgs e)
		{
			RemoveMorghsBackups = true;
		}
		private void CheckBoxRemoveMorghsBackupsUnchecked(object sender, RoutedEventArgs e)
		{
			RemoveMorghsBackups = false;
		}
		private void CheckBoxCreateModFolderChecked(object sender, RoutedEventArgs e)
		{
			CreateModFolder = true;
		}
		private void CheckBoxCreateModFolderUnchecked(object sender, RoutedEventArgs e)
		{
			CreateModFolder = false;
		}
		private void CheckBoxCreateModArchiveChecked(object sender, RoutedEventArgs e)
		{
			CreateModArchive = true;
		}
		private void CheckBoxCreateModArchiveUnchecked(object sender, RoutedEventArgs e)
		{
			CreateModArchive = false;
		}

		private void ButtonBrowseEventClick(object sender, RoutedEventArgs e)
		{
			CommonOpenFileDialog FolderSelectDialog1 = new CommonOpenFileDialog
			{
				Title = "Select mod folder",
				IsFolderPicker = true,
				InitialDirectory = TextBoxModPath.Text,
				AddToMostRecentlyUsedList = false,
				AllowNonFileSystemItems = false,
				DefaultDirectory = TextBoxModPath.Text,
				EnsureFileExists = true,
				EnsurePathExists = true,
				EnsureReadOnly = false,
				EnsureValidNames = true,
				Multiselect = false,
				ShowPlacesList = true,
			};
			if (FolderSelectDialog1.ShowDialog() == CommonFileDialogResult.Ok)
			{
				var Folder = FolderSelectDialog1.FileName;
				TextBoxModPath.Text = Folder.ToString();
			}
		}
		private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
		{
			// Get the subdirectories for the specified directory.
			DirectoryInfo dir = new DirectoryInfo(sourceDirName);

			if (!dir.Exists)
			{
				throw new DirectoryNotFoundException(
					"Method DirectoryCopy - Source directory does not exist or could not be found: "
					+ sourceDirName);
			}

			DirectoryInfo[] dirs = dir.GetDirectories();
			// If the destination directory doesn't exist, create it.
			if (!Directory.Exists(destDirName))
			{
				Directory.CreateDirectory(destDirName);
			}

			// Get the files in the directory and copy them to the new location.
			FileInfo[] files = dir.GetFiles();
			foreach (FileInfo file in files)
			{
				string temppath = Path.Combine(destDirName, file.Name);
				file.CopyTo(temppath, false);
			}

			// If copying subdirectories, copy them and their contents to new location.
			if (copySubDirs)
			{
				foreach (DirectoryInfo subdir in dirs)
				{
					string temppath = Path.Combine(destDirName, subdir.Name);
					DirectoryCopy(subdir.FullName, temppath, copySubDirs);
				}
			}
		}
		public static void Create7zArchiveFromFolder(string ArchivePath, string FolderPath)
		{
			ProcessStartInfo StartInfo7za = new ProcessStartInfo();
			StartInfo7za.CreateNoWindow = true;
			StartInfo7za.UseShellExecute = false;
			StartInfo7za.FileName = "7za.exe";
			StartInfo7za.WindowStyle = ProcessWindowStyle.Hidden;
			StartInfo7za.Arguments = "a \"" + ArchivePath + "\" " + "\"" + FolderPath + @"\" + "\"";
			try
			{
				// Start the process with the info we specified.
				// Call WaitForExit and then the using statement will close.
				using (Process exeProcess = Process.Start(StartInfo7za))
				{
					exeProcess.WaitForExit();
				}
			}
			catch
			{
				// Log error.
			}
		}
		public static void RemoveFiles(string Path, string SearchPattern)
		{
			string[] Files = Directory.GetFiles(Path, SearchPattern, SearchOption.AllDirectories);
			foreach (var File_ in Files)
			{
				File.Delete(File_);
			}
		}
		private void AssembleCTEReleaseOnClose(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			this.Visibility = Visibility.Hidden;
		}
	}
}
