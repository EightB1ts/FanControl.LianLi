using FanControl.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FanControl.LianLiPlugin
{
    public class LianLiPlugin : IPlugin, IDisposable
    {
        public string Name => "Lian Li";
        private Boolean m_DisposedValue;
        private LianLi.Devices _devices;
        private bool _initialized;
        private bool enableARGB = true;

        public void Close()
        {

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

        protected virtual void Dispose(Boolean disposing)
        {
            if (!m_DisposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                Close();
                m_DisposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~LianLiPlugin()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
