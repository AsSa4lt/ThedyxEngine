using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using ThedyxEngine.Engine;
using ThedyxEngine.Engine.Managers;

namespace ThedyxEngine.UI;

/**
 * MaterialsPopup is a popup that allows the user to change the properties of the materials.
 */
public partial class MaterialsPopup : Popup {
    /** Current selected material */
    private Material? _currentSelectedMaterial;
    /** Callback to reopen the material popup, because MAUI is awful */
    public Action<Material> ReopenMaterialPopup;
    
    /**
     * Constructor for the MaterialsPopup class.
     */
    public MaterialsPopup() {
        InitializeComponent();
        // we need to update the list of materials
        ListMaterials.OnSelectedMaterialChanged += OnSelectedMaterialChanged;
        // select the first material if there is any
        if(MaterialManager.Materials.Count > 0)
            ListMaterials.SelectMaterial(MaterialManager.Materials[0]);
    }
    
    /**
     * This method is called when the user selects a material from the list
     * If fills the entries with the properties of the selected material
     * \param material The selected material
     */
    private void OnSelectedMaterialChanged(Material material) {
        _currentSelectedMaterial = material;
        // we need to update the properties of the selected material
        Name.Text = material.Name;
        SolidSpecificHeatCapacity.Text = material.SolidSpecificHeatCapacity.ToString();
        LiquidSpecificHeatCapacity.Text = material.LiquidSpecificHeatCapacity.ToString();
        GasSpecificHeatCapacity.Text = material.GasSpecificHeatCapacity.ToString();
        SolidDensity.Text = material.SolidDensity.ToString();
        LiquidDensity.Text = material.LiquidDensity.ToString();
        GasDensity.Text = material.GasDensity.ToString();
        SolidEmissivity.Text = material.SolidEmissivity.ToString();
        LiquidEmissivity.Text = material.LiquidEmissivity.ToString();
        GasEmissivity.Text = material.GasEmissivity.ToString();
        SolidThermalConductivity.Text = material.SolidThermalConductivity.ToString();
        LiquidThermalConductivity.Text = material.LiquidThermalConductivity.ToString();
        GasThermalConductivity.Text = material.GasThermalConductivity.ToString();
        MeltingTemperature.Text = material.MeltingTemperature.ToString();
        BoilingTemperature.Text = material.BoilingTemperature.ToString();
        MeltingEnergy.Text = material.MeltingEnergy.ToString();
        BoilingEnergy.Text = material.BoilingEnergy.ToString();
        LiquidConvectiveHeatTransferCoefficient.Text = material.LiquidConvectiveHeatTransferCoefficient.ToString();
        GasConvectiveHeatTransferCoefficient.Text = material.GasConvectiveHeatTransferCoefficient.ToString();
        ColorR.Text = material.MaterialColor.GetByteRed().ToString();
        ColorG.Text = material.MaterialColor.GetByteGreen().ToString();
        ColorB.Text = material.MaterialColor.GetByteBlue().ToString();
        ColorA.Text = material.MaterialColor.GetByteAlpha().ToString();
    }

    /**
     * This method is called when the user changes the name of the material
     * Or when we add a new material
     */
    private void Update() {
        ListMaterials.Update();
    }
    
    /**
     * This method is called when the user clicks the add material button
     * It creates a new material and adds it to the list
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnAddMaterialClicked(object sender, EventArgs e) {
        // create a new material
        Material material = new Material();
        // create a new available name
        string name = "Material";
        int count = 1;
        while (!MaterialManager.IsNameAvailable(name + count)) {
            count++;
        }
        material.Name = name + count;
        // add the material to the list
        MaterialManager.Materials.Add(material);
        // select the material
        ListMaterials = new MaterialsList();
        ListMaterials.SelectMaterial(material);
        // update the list
        Update();
        ReOpenMaterialPopup(material);
    }
    
    /**
     * This method is called to force material selection
     * \param material The material to select
     */
    public void SelectMaterial(Material material) {
        ListMaterials.SelectMaterial(material);
    }
    
    /**
     * This method is called when we need to reopen Popup because of the bug in MAUI
     * \param material The material to select
     */
    private void ReOpenMaterialPopup(Material material) {
        this.Close();
       ReopenMaterialPopup?.Invoke(material);
    }
    
    /**
     * This method is called when the user finishes editing the name of the material
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnNameCompleted(object sender, EventArgs e) {
        // if name is the same, we don't need to update, show a message
        if (_currentSelectedMaterial == null || _currentSelectedMaterial.Name == Name.Text) return;
        // check if the name is already in use
        if(!MaterialManager.IsNameAvailable(Name.Text))
            Application.Current.MainPage?.DisplayAlert("Error", "Name already in use", "Ok");
        else 
            _currentSelectedMaterial.Name = Name.Text;
        Update();
    }
    
    /**
     * This method is called when the user clicks the delete button
     * It deletes the selected material
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnDeleteButtonClicked(object sender, EventArgs e) {
        // if the material is null, we don't need to delete
        if (_currentSelectedMaterial == null) return;
        // show a confirmation message
        Application.Current.MainPage?.DisplayAlert("Delete", "Are you sure you want to delete this material?", "Yes", "No").ContinueWith(task => {
            if (task.Result) {
                // remove the material from the list
                MaterialManager.Materials.Remove(_currentSelectedMaterial);
                // select the first material
                _currentSelectedMaterial = null;
                ListMaterials.Update();
                if (MaterialManager.Materials.Count > 0)
                    ListMaterials.SelectMaterial(MaterialManager.Materials[0]);
                // update the list
                Update();
            }
        });
        if (MaterialManager.Materials.Count == 0)
            ReOpenMaterialPopup(null);
        else
            ReOpenMaterialPopup(MaterialManager.Materials[0]);
    }

    /**
     * This method is called when the user finishes editing the solid specific heat capacity
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnSolidSpecificHeatCapacityCompleted(object sender, EventArgs e) {
        // try to parse the value to double, if cant, show a message
        if (_currentSelectedMaterial == null ) return;
        if (!double.TryParse(SolidSpecificHeatCapacity.Text, out double value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }
        _currentSelectedMaterial.SolidSpecificHeatCapacity = value;
    }
    
    /**
     * This method is called when the user finishes editing the liquid specific heat capacity
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnLiquidSpecificHeatCapacityCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!double.TryParse(LiquidSpecificHeatCapacity.Text, out double value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }
        _currentSelectedMaterial.LiquidSpecificHeatCapacity = value;
    }
    
    /**
     * This method is called when the user finishes editing the gas specific heat capacity
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnGasSpecificHeatCapacityCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!double.TryParse(GasSpecificHeatCapacity.Text, out double value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }
        _currentSelectedMaterial.GasSpecificHeatCapacity = value;
    }
    
    /**
     * This method is called when the user finishes editing the solid density
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnSolidDensityCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!double.TryParse(SolidDensity.Text, out double value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }
        _currentSelectedMaterial.SolidDensity = value;
    }
    
    private void OnLiquidDensityCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!double.TryParse(LiquidDensity.Text, out double value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }
        _currentSelectedMaterial.LiquidDensity = value;
    }
    
    /**
     * This method is called when the user finishes editing the gas density
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnGasDensityCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!double.TryParse(GasDensity.Text, out double value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }
        _currentSelectedMaterial.GasDensity = value;
    }
    
    /**
     * This method is called when the user finishes editing the solid emissivity
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnSolidEmissivityCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!double.TryParse(SolidEmissivity.Text, out double value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }
        _currentSelectedMaterial.SolidEmissivity = value;
    }
    
    /**
     * This method is called when the user finishes editing the liquid emissivity
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnLiquidEmissivityCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!double.TryParse(LiquidEmissivity.Text, out double value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }
        _currentSelectedMaterial.LiquidEmissivity = value;
    }
    
    /**
     * This method is called when the user finishes editing the gas emissivity
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnGasEmissivityCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!double.TryParse(GasEmissivity.Text, out double value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }
        _currentSelectedMaterial.GasEmissivity = value;
    }
    
    /**
     * This method is called when the user finishes editing the solid thermal conductivity
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnSolidThermalConductivityCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!double.TryParse(SolidThermalConductivity.Text, out double value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }
        _currentSelectedMaterial.SolidThermalConductivity = value;
    }
    
    /**
     * This method is called when the user finishes editing the liquid thermal conductivity
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnLiquidThermalConductivityCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!double.TryParse(LiquidThermalConductivity.Text, out double value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }
        _currentSelectedMaterial.LiquidThermalConductivity = value;
    }
    
    /**
     * This method is called when the user finishes editing the gas thermal conductivity
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnGasThermalConductivityCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!double.TryParse(GasThermalConductivity.Text, out double value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }
        _currentSelectedMaterial.GasThermalConductivity = value;
    }

    /**
     * This method is called when the user finishes editing the melting temperature
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnMeltingTemperatureCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!double.TryParse(MeltingTemperature.Text, out double value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }
        _currentSelectedMaterial.MeltingTemperature = value;
    }
    
    /**
     * This method is called when the user finishes editing the boiling temperature
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnBoilingTemperatureCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!double.TryParse(BoilingTemperature.Text, out double value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }
        _currentSelectedMaterial.BoilingTemperature = value;
    }
    
    /**
     * This method is called when the user finishes editing the melting energy
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnMeltingEnergyCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!double.TryParse(MeltingEnergy.Text, out double value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }
        _currentSelectedMaterial.MeltingEnergy = value;
    }
    
    /**
     * This method is called when the user finishes editing the boiling energy
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnBoilingEnergyCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!double.TryParse(BoilingEnergy.Text, out double value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }
        _currentSelectedMaterial.BoilingEnergy = value;
    }
    
    /**
     * This method is called when the user finishes editing the red color
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnRedColorCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!byte.TryParse(ColorR.Text, out byte value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }

        Color color = new Color(value, 
            _currentSelectedMaterial.MaterialColor.GetByteGreen(),
            _currentSelectedMaterial.MaterialColor.GetByteBlue(),
            _currentSelectedMaterial.MaterialColor.GetByteAlpha());
    }
    
    /**
     * This method is called when the user finishes editing the green color
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnGreenColorCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!byte.TryParse(ColorG.Text, out byte value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }

        Color color = new Color(_currentSelectedMaterial.MaterialColor.GetByteRed(), 
            value,
            _currentSelectedMaterial.MaterialColor.GetByteBlue(),
            _currentSelectedMaterial.MaterialColor.GetByteAlpha());
    }
    
    /**
     * This method is called when the user finishes editing the blue color
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnBlueColorCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!byte.TryParse(ColorB.Text, out byte value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }

        Color color = new Color(_currentSelectedMaterial.MaterialColor.GetByteRed(),
            _currentSelectedMaterial.MaterialColor.GetByteGreen(), value,
            _currentSelectedMaterial.MaterialColor.GetByteAlpha());
    }
    
    /**
     * This method is called when the user finishes editing the alpha color
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnAlphaColorCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!byte.TryParse(ColorA.Text, out byte value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }

        Color color = new Color(_currentSelectedMaterial.MaterialColor.GetByteRed(),
            _currentSelectedMaterial.MaterialColor.GetByteGreen(), 
            _currentSelectedMaterial.MaterialColor.GetByteBlue(),
            value);
    }
    
    /**
     * This method is called when the user finishes editing the solid convective heat transfer coefficient
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnLiquidConvectiveHeatTransferCoefficientCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!double.TryParse(LiquidConvectiveHeatTransferCoefficient.Text, out double value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }
        _currentSelectedMaterial.LiquidConvectiveHeatTransferCoefficient = value;
    }
    
    /**
     * This method is called when the user finishes editing the gas convective heat transfer coefficient
     * \param sender The object that sent the event
     * \param e The event arguments
     */
    private void OnGasConvectiveHeatTransferCoefficientCompleted(object sender, EventArgs e) {
        if (_currentSelectedMaterial == null ) return;
        if (!double.TryParse(GasConvectiveHeatTransferCoefficient.Text, out double value)) {
            Application.Current.MainPage?.DisplayAlert("Error", "Invalid value", "Ok");
            return;
        }
        _currentSelectedMaterial.GasConvectiveHeatTransferCoefficient = value;
    }
    
}