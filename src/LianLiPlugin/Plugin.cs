using FanControl.Plugins;
using System;
using System.Linq;

namespace FanControl.LianLiPlugin
{
    public class LianLiPlugin : IPlugin
    {
        public string Name => "Lian Li";
        private Boolean m_DisposedValue;
        private LianLi.Devices _devices;
        private bool _initialized;
        private bool enableARGB = false;

        public void Close()
        {
            // Close all Lian Li Devices
            _devices.Dispose();
        }

        public void Initialize()
        {
            _devices = new LianLi.Devices(enableARGB);
            _initialized = _devices.FanControllers_Located();
        }

        public void Load(IPluginSensorsContainer _container)
        {
            if(_initialized)
            {
                /** Load Fan Controllers **/
                // Each Controller consists of 4 Channels - Configure a sensor for each  
                var controlSensors = new ControlSensors[_devices.FanControllers_Count() * 4];
                for (int i = 0; i < controlSensors.Count(); i++)
                {
                    var controllerIndex = (int)i / 4;
                    var channelIndex = (int)i % 4;
                    _container.ControlSensors.Add(new ControlSensors(_devices, controllerIndex, channelIndex));
                    _container.FanSensors.Add(new Sensors(_devices, controllerIndex, channelIndex));
                }

            }
        }

    }
}
