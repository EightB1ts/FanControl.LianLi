using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FanControl.Plugins;

namespace FanControl.LianLiPlugin
{
    public class ControlSensors : IPluginControlSensor
    {
        private Controllers.Controllers _controllers;
        private readonly int _controllerIndex;
        private readonly int _channelIndex;
        private float? _val;

        public ControlSensors(Controllers.Controllers controllers, int controllerIndex, int channelIndex) {
            _controllers = controllers;
            _controllerIndex = controllerIndex;
            _channelIndex = channelIndex;
        }

        public string Identifier => $"LianLi/Control/{(int)_controllerIndex + 1}:{(int)_channelIndex + 1}";
        public float? Value { get; private set; }
        public string Name => $"Uni Controller #{(int)_controllerIndex + 1} Ch. {(int)_channelIndex + 1}";
        public string Origin => $"LianLi";

        public string Id => "Controller_"
            + _controllerIndex.ToString()
            + " _Channel_"
            + _channelIndex.ToString();

        public void Update() => Value = _val;

        public void Set(float val)
        {
            if(_val != val)
            {
                _controllers.SetFanSpeed(_controllerIndex, _channelIndex, val);
                _val = val;
            }
        }

        public void Reset()
        {
           
        }

    }

    public class Sensors: IPluginSensor
    {

        private Controllers.Controllers _controllers;
        private readonly int _controllerIndex;
        private readonly int _channelIndex;

        public Sensors(Controllers.Controllers controllers, int controllerIndex, int channelIndex)
        {
            _controllers = controllers;
            _controllerIndex = controllerIndex;
            _channelIndex = channelIndex;
        }

        public string Identifier => $"LianLi/Sensor/{(int)_controllerIndex + 1}:{(int)_channelIndex + 1}";
        public float? Value { get; private set; }

        public string Name => $"Uni Controller #{(int)_controllerIndex + 1} Ch. {(int)_channelIndex + 1}";

        public string Origin => $"LianLi";

        public string Id => "Controller_"
            + _controllerIndex.ToString()
            + " _Channel_"
            + _channelIndex.ToString();

        public void Update() => Value = _controllers.GetFanSpeed(_controllerIndex, _channelIndex);

    }
}
