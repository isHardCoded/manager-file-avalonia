using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fileManager.Models;

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace fileManager.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _currentPath;

        [ObservableProperty]
        private FileItem _selectedItem;

        [ObservableProperty]
        private string _fileContent;

        public ObservableCollection<FileItem> Items { get; } = new();

        public MainWindowViewModel()
        {
            CurrentPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            LoadDirectory();
        }

        [RelayCommand]
        private void LoadDirectory()
        {
            try
            {
                Items.Clear();

                var directories = Directory.GetDirectories(CurrentPath);

                Debug.WriteLine("Папки на рабочем столе: ");
                foreach (var dir in directories)
                {
                    var info = new DirectoryInfo(dir);
                    Debug.WriteLine(info.Name);

                    Items.Add(new FileItem
                    {
                        Name = info.Name,
                        FullPath = info.FullName,
                        Size = 0,
                        Created = info.CreationTime,
                        IsDirectory = true,
                    });
                }

                var files = Directory.GetFiles(CurrentPath);

                Debug.WriteLine("Файлы на рабочем столе: ");
                foreach (var file in files)
                {
                    var info = new FileInfo(file);
                    Debug.WriteLine(info.Name);

                    Items.Add(new FileItem
                    {
                        Name = info.Name,
                        FullPath = info.FullName,
                        Size = info.Length,
                        Created = info.CreationTime,
                        IsDirectory = false,
                    });
                }
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        [RelayCommand]
        private void CreateFile()
        {
            try
            {
                string path = Path.Combine(CurrentPath, "NewFile.txt");
                using (File.Create(path)) { }
                LoadDirectory();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        [RelayCommand]
        private void CreateFolder()
        {
            try
            {
                string path = Path.Combine(CurrentPath, "NewFolder");
                Directory.CreateDirectory(path);
                LoadDirectory();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedItem == null) return;

            try
            {
                if (SelectedItem.IsDirectory)
                {
                    if (Directory.Exists(SelectedItem.FullPath))
                        Directory.Delete(SelectedItem.FullPath, true);
                }
                else
                {
                    if (File.Exists(SelectedItem.FullPath))
                        File.Delete(SelectedItem.FullPath);
                }

                SelectedItem = null;
                LoadDirectory();
            }
            catch (IOException ex)
            {
                Debug.WriteLine("Файл используется другим процессом: " + ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine("Нет доступа: " + ex.Message);
            }
        }

        [RelayCommand]
        private void OpenFile()
        {
            if (SelectedItem == null || SelectedItem.IsDirectory) return;

            try
            {
                FileContent = File.ReadAllText(SelectedItem.FullPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        [RelayCommand]
        private void SaveFile()
        {
            if (SelectedItem == null || SelectedItem.IsDirectory) return;

            try
            {
                File.WriteAllText(SelectedItem.FullPath, FileContent);
                LoadDirectory();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
