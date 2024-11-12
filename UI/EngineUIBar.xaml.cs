using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using ThedyxEngine.Engine;
using ThedyxEngine.Util;

namespace ThedyxEngine.UI
{
    public partial class EngineUIBar : ContentView
    {
        // Callback to update the UI
        public Action? UpdateUI;
        public Action<EngineObject>? DeleteSelected;
        public Action? EngineModeChanged;

        public EngineUIBar()
        {
            InitializeComponent();
            Loaded += (sender, args) =>
            {
                SetStoppedMode();
            };
        }

        private void SetStoppedMode()
        {
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

        public void Update()
        {
            long currentTime = Engine.Engine.GetSimulationTime();
            TimeSpan time = TimeSpan.FromMilliseconds(currentTime);
            TimeLabel.Text = time.ToString(@"mm\:ss\:ff");
        }

        private void SetRunningMode()
        {
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

        private void SetPausedMode()
        {
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

        private async Task SaveFile()
        {
            // Placeholder for file saving logic
            string filename = "saved_file.ten";
            FileManager.SaveToFile(filename);
            await Application.Current.MainPage.DisplayAlert("File Saved", $"File saved as {filename}", "OK");
        }

        private async Task OpenFile()
        {
            // Placeholder for file loading logic
            string filename = "opened_file.ten";
            FileManager.LoadFromFile(filename);
            UpdateUI?.Invoke();
            await Application.Current.MainPage.DisplayAlert("File Opened", $"File opened: {filename}", "OK");
        }

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            await SaveFile();
        }

        private async void OnOpenButtonClicked(object sender, EventArgs e)
        {
            await OpenFile();
        }

        private async void OnClearButtonClicked(object sender, EventArgs e)
        {
            bool result = await Application.Current.MainPage.DisplayAlert("Confirm Clear", "Are you sure you want to clear all data?", "Yes", "No");
            if (result)
            {
                Engine.Engine.ClearSimulation();
                DeleteSelected?.Invoke(null);
            }
        }

        private async void OnAddButtonClicked(object sender, EventArgs e)
        {
            string action = await Application.Current.MainPage.DisplayActionSheet("Add Item", "Cancel", null, "Grainsquare", "Rectangle", "Circle");
            switch (action)
            {
                case "Grainsquare":
                    AddSquare();
                    break;
                case "Rectangle":
                    // Handle Rectangle selection if enabled
                    break;
                case "Circle":
                    // Handle Circle selection if enabled
                    break;
            }
        }

        private void AddSquare()
        {
            ///TODO: Logic to add a GrainSquare
            UpdateUI?.Invoke();
        }

        private void OnStartButtonClicked(object sender, EventArgs e)
        {
            Engine.Engine.Start();
            EngineModeChanged?.Invoke();
            SetRunningMode();
        }

        private void OnStopButtonClicked(object sender, EventArgs e)
        {
            Engine.Engine.Stop();
            EngineModeChanged?.Invoke();
            SetStoppedMode();
        }

        private void OnPauseButtonClicked(object sender, EventArgs e)
        {
            Engine.Engine.Pause();
            EngineModeChanged?.Invoke();
            SetPausedMode();
        }

        private async void OnResetButtonClicked(object sender, EventArgs e)
        {
            bool result = await Application.Current.MainPage.DisplayAlert("Confirm Reset", "Are you sure you want to reset simulation data?", "Yes", "No");
            if (result)
            {
                Engine.Engine.ResetSimulation();
            }
        }
    }
}