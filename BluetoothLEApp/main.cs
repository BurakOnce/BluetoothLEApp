using System;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Essentials;
using Plugin.BluetoothLE;

namespace BluetoothLEApp
{
    public partial class MainPage : ContentPage
    {
        private IAdapter _adapter;
        private IDevice _device;
        private ICharacteristic _characteristic;

        public MainPage()
        {
            InitializeComponent();

            _adapter = CrossBleAdapter.Current;
        }

        /* Method to scan for Bluetooth devices and connect */
        private async void ScanAndConnect()
        {
            try
            {
                // Scan for Bluetooth devices
                var scanResult = await _adapter.Scan().FirstOrDefaultAsync();
                if (scanResult != null)
                {
                    // Device found, attempt to connect
                    _device = scanResult.Device;
                    await _device.ConnectWait();

                    // Find a specific service and characteristic
                    var service = await _device.GetServiceAsync(Guid.Parse("0000180D-0000-1000-8000-00805F9B34FB"));
                    if (service != null)
                    {
                        _characteristic = await service.GetCharacteristicAsync(Guid.Parse("00002AB4-0000-1000-8000-00805F9B34FB"));
                        if (_characteristic != null)
                        {
                            // Enable notifications for the characteristic
                            await _characteristic.EnableNotificationsAsync();
                            _characteristic.ValueUpdated += Characteristic_ValueUpdated;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle errors
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        /* Event handler for handling value updates from the characteristic */
        private void Characteristic_ValueUpdated(object sender, Plugin.BluetoothLE.ValueChangedEventArgs e)
        {
            // Process data received from the characteristic here
            var value = e.Characteristic.Value;
            // Take action to use the data
        }

        private void ScanButton_Clicked(object sender, EventArgs e)
        {
            ScanAndConnect();
        }
    }
}
