using Microsoft.Maui.Controls;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using ThedyxEngine.Engine;
using ThedyxEngine.Util;
using CommunityToolkit.Maui.Views;
using ThedyxEngine.Engine.Managers;
using CommunityToolkit.Maui.Storage;
using LukeMauiFilePicker;
using Newtonsoft.Json;

namespace ThedyxEngine.UI
{
    public partial class EngineUIBar : ContentView
    {
        // Callback to update the UI
        public Action? UpdateUI;
        public Action<EngineObject>? DeleteSelected;
        public Action? EngineModeChanged;
        public MainPage? MainPage;
        public EngineUIBar() {
            InitializeComponent();
            Loaded += (sender, args) => {
                SetStoppedMode();
            };
        }

        private void SetStoppedMode() {
            // Set engine controls
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            PauseButton.IsEnabled = false;

            // Set UI controls
            AddButton.IsEnabled = true;
            SaveButton.IsEnabled = true;
            OpenButton.IsEnabled = true;
            ClearButton.IsEnabled = true;
            ResetButton.IsEnabled = true;
        }

        public void Update() {
            long currentTime = Engine.Engine.GetSimulationTime();
            TimeSpan time = TimeSpan.FromMilliseconds(currentTime);

            if (time.Days > 0) {
                // More than one day: show days, hours, and minutes
                TimeLabel.Text = time.ToString(@"dd\:hh\:mm");
            } 
            else if (time.Hours > 0) {
                // More than one hour but less than a day: show hours, minutes, and seconds
                TimeLabel.Text = time.ToString(@"hh\:mm\:ss");
            } 
            else {
                // Less than one hour: show minutes, seconds, and centiseconds
                TimeLabel.Text = time.ToString(@"mm\:ss\:ff");
            }
        }


        private void SetRunningMode() {
            // Set engine controls
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            PauseButton.IsEnabled = true;

            // Set UI controls
            AddButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
            OpenButton.IsEnabled = false;
            ClearButton.IsEnabled = false;
            ResetButton.IsEnabled = false;
        }

        private void SetPausedMode() {
            // Set engine controls
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = true;
            PauseButton.IsEnabled = false;

            // Set UI controls
            AddButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
            OpenButton.IsEnabled = false;
            ClearButton.IsEnabled = false;
            ResetButton.IsEnabled = true;
        }
        

        private void OnSaveButtonClicked(object sender, EventArgs e) {
            string jsonOutput = FileManager.GetObjectsJsonRepresentation();
            byte[] bytes = Encoding.UTF8.GetBytes(jsonOutput);
            using var memoryStream = new MemoryStream(bytes);

            var saveResult =  FileSaver.Default.SaveAsync("simulation.tdx", memoryStream);
        }


        private async void OnOpenButtonClicked(object sender, EventArgs e) {
            var customFileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>> {
                { DevicePlatform.iOS, new[] { ".tdx" } },
                { DevicePlatform.MacCatalyst, new[] { "public.data" } },
                { DevicePlatform.WinUI, new[] { ".tdx" } },
                { DevicePlatform.Android, new[] { ".tdx" } }
            });

            var options = new PickOptions {
                PickerTitle = "Select a .tdx File",
                FileTypes = customFileTypes
            };

            var file = await FilePicker.Default.PickAsync(options);  // Await the task

            if (file != null) {
                try {
                    using (var stream = await file.OpenReadAsync())  // Read the file as a stream
                    using (var reader = new StreamReader(stream)) {
                        string fileContent = await reader.ReadToEndAsync();  // Read content as a string
                        FileManager.LoadFromContent(fileContent);  // Pass content to LoadFromContent
                    }
                }
                catch (Exception ex) {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Failed to read the file: {ex.Message}", "OK");
                }
            }
            Engine.Engine.ResetSimulation();
        }


        private async void OnClearButtonClicked(object sender, EventArgs e) {
            bool result = await Application.Current.MainPage.DisplayAlert("Confirm Clear", "Are you sure you want to clear all data?", "Yes", "No");
            if (result) {
                Engine.Engine.ClearSimulation();
                DeleteSelected?.Invoke(null);
            }
        }

        private async void OnAddButtonClicked(object sender, EventArgs e) {
            string action = await Application.Current.MainPage.DisplayActionSheet("Add Item", "Cancel", null, "Grain Object", "Solid Object", "State Object");
            switch (action) {
                case "Grain Object":
                    AddObject(ObjectType.GrainSquare);
                    break;
                case "Solid Object":
                    AddObject(ObjectType.Rectangle);
                    break;
                case "State Object":
                    AddObject(ObjectType.StateRectangle);
                    break;
            }
        }

        private void AddObject(ObjectType objectType) {
            var createPopup = new CreateObjectPopup(objectType);
            this.MainPage?.ShowPopup(createPopup);
            if (UpdateUI != null) createPopup.OnObjectCreated += UpdateAll;
        }
        
        private void UpdateAll() {
            UpdateUI?.Invoke();
        }

        private void OnStartButtonClicked(object sender, EventArgs e) {
            Engine.Engine.Start();
            EngineModeChanged?.Invoke();
            SetRunningMode();
        }
        
        private void OnStopButtonClicked(object sender, EventArgs e) {
            Engine.Engine.Stop();
            EngineModeChanged?.Invoke();
            SetStoppedMode();
        }

        private void OnPauseButtonClicked(object sender, EventArgs e) {
            Engine.Engine.Pause();
            EngineModeChanged?.Invoke();
            SetPausedMode();
        }
    
        private void OnTemperatureModeButtonClicked(object sender, EventArgs e) {
            Engine.Engine.ShowTemperature = !Engine.Engine.ShowTemperature;
            // change color of button
            if (Engine.Engine.ShowTemperature) {
                ModeButton.BackgroundColor = Colors.Olive;
            } else {
                ModeButton.BackgroundColor = Colors.DarkGray;
            }
            UpdateUI?.Invoke();
        }
        
        private void OnSettingsButtonClicked(object sender, EventArgs e) {
            var settingsPopup = new SettingsPopup();
            this.MainPage?.ShowPopup(settingsPopup);
            UpdateUI?.Invoke();
        }
        
        private void OnGridButtonClicked(object sender, EventArgs e) {
            Engine.Engine.ShowGrid = !Engine.Engine.ShowGrid;
            // change color of button
            if (Engine.Engine.ShowGrid) {
                GridButton.BackgroundColor = Colors.Peru;
            } else {
                GridButton.BackgroundColor = Colors.DarkGray;
            }
            UpdateUI?.Invoke();
        }
        
        private void OnColorModeButtonClicked(object sender, EventArgs e) {
            Engine.Engine.ShowColor = !Engine.Engine.ShowColor;
            if (Engine.Engine.ShowColor) {
                ColorModeButton.BackgroundColor = Colors.Salmon;
                ColorModeButton.Text = "Color";
            }else {
                ColorModeButton.BackgroundColor = Colors.DarkGray;
                ColorModeButton.Text = "Temp";
            }
            UpdateUI?.Invoke();
        }
        
        private async void OnResetButtonClicked(object sender, EventArgs e) {
            bool result = await Application.Current.MainPage.DisplayAlert("Confirm Reset", "Are you sure you want to reset simulation data?", "Yes", "No");
            if (result) {
                Engine.Engine.ResetSimulation();
            }
        }
    }
}
