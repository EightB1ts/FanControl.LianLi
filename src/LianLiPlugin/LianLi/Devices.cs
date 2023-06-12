using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LianLi
{
    public class Devices
    {

        private List<FanController> _fancontrollers = new List<FanController>{ };

        public Devices(bool enableARGB) {

            // Scan for controllers
            int[] VENDOR_IDS = { 0x0cf2 };
            int[] PRODUCT_IDS = { 0x7750, 0xa100, 0xa101, 0xa102, 0xa103, 0xa104, 0xa105 };

            var Devices = HID.Locate(VENDOR_IDS, PRODUCT_IDS);

            foreach (var device in Devices)
            {
                _fancontrollers.Add(new FanController(device, enableARGB));
            }

        }

        // Dispose Fan Controllers
        public void Dispose()
        {
            for (int i = 0; i < _fancontrollers.Count(); i++)
            {
                _fancontrollers[i].Dispose();
            }
        }

        public bool FanControllers_Located()
        {
            if (_fancontrollers.Count == 0) { return false; }
            return true;
        }

        public int FanControllers_Count()
        {
            return _fancontrollers.Count;
        }

        public float FanControllers_GetSpeed(int fancontroller_index, int fancontroller_channel)
        {
            return _fancontrollers[fancontroller_index].GetSpeed(fancontroller_channel);
        }

        public void FanControllers_SetSpeed(int fancontroller_index, int fancontroller_channel, int speed)
        {
            _fancontrollers[fancontroller_index].SetSpeed(fancontroller_channel, speed);
        }
       
    }

    internal class FanController
    {
        private enum Type {
            SL,
            SLV2,
            SLI,
            AL,
            ALV2,
            Unknown
        }

        private Type _type;
        private HIDDevice _device;
        public float _speed;

        public FanController(HIDDevice device, bool enableARGB)
        {
            _device = device;
            switch (device._pid)
            {
                case 0xa100:
                case 0x7750: 
                    _type = Type.SL;
                    break;
                case 0xa101:
                    _type = Type.AL;
                    break;
                case 0xa102:
                    _type = Type.SLI;
                    break;
                case 0xa103:
                case 0xa105:
                    _type = Type.SLV2;
                    break;
                case 0xa104:
                    _type = Type.ALV2;
                    break;
                default:
                    _type = Type.Unknown;
                    break;
            }

            if(enableARGB){ SetARGB(); }
            for (int i = 0; i < 4; i++) { DisableRPMSync(i); }
        }


        public void SetSpeed(int fancontroller_channel, int speed)
        {
            var speed_800_1900 = (byte)((800 + (11 * speed)) / 19);
            var speed_250_2000 = (byte)((250 + (17.5 * speed)) / 20);
            var speed_200_2100 = (byte)((200 + (19 * speed)) / 21);

            switch (_type)
            {
                case Type.SL:
                    _device.Write(new byte[] { 224, (byte)(32 + fancontroller_channel), 0, speed_800_1900 });
                    break;
                case Type.SLV2:
                    _device.Write(new byte[] { 224, (byte)(32 + fancontroller_channel), 0, speed_250_2000 });
                    break;
                case Type.AL:
                    _device.Write(new byte[] { 224, (byte)(32 + fancontroller_channel), 0, speed_800_1900 });
                    break;
                case Type.ALV2:
                    _device.Write(new byte[] { 224, (byte)(32 + fancontroller_channel), 0, speed_250_2000 });
                    break;
                case Type.SLI:
                    _device.Write(new byte[] { 224, (byte)(32 + fancontroller_channel), 0, speed_200_2100 });
                    break;
            }
        }

        public float GetSpeed(int fancontroller_channel)
        {
           

            byte[] buffer = new byte[65];
            // Same for all of them. Switch because this may change in the future.
            switch (_type)
            {
                case Type.SL:
                    buffer = _device.ReadInputReport(224, 65);
                    break;
                case Type.SLV2:
                    buffer = _device.ReadInputReport(224, 65);
                    break;
                case Type.AL:
                    buffer = _device.ReadInputReport(224, 65);
                    break;
                case Type.ALV2:
                    buffer = _device.ReadInputReport(224, 65);
                    break;
                case Type.SLI:
                    buffer = _device.ReadInputReport(224, 65);
                    break;
            }

            return (float)BitConverter.ToUInt16(buffer.Skip(1 + fancontroller_channel * 2).Take(2).Reverse().ToArray(), 0);
        }

        public void Dispose()
        {
            _device.Dispose();
        }

        private void DisableRPMSync(int fancontroller_channel)
        {
            byte channelByte = (byte)((2 * fancontroller_channel) * 16);
            if(fancontroller_channel == 0) { channelByte = (byte)(16); }

            switch (_type)
            {
                case Type.SL:
                    _device.Write(new byte[] { 224, 16, 49, channelByte });
                    break;
                case Type.SLV2:
                    _device.Write(new byte[] { 224, 16, 98, channelByte });
                    break;
                case Type.AL:
                    _device.Write(new byte[] { 224, 16, 66, channelByte });
                    break;
                case Type.ALV2:
                    _device.Write(new byte[] { 224, 16, 98, channelByte });
                    break;
                case Type.SLI:
                    _device.Write(new byte[] { 224, 16, 98, channelByte });
                    break;
            }
        }

        private void SetARGB()
        {
            switch(_type)
            {
                case Type.SL:
                    _device.Write(new byte[] { 224, 16, 48, 1, 0, 0, 0 });
                    break;
                case Type.SLV2:
                    _device.Write(new byte[] { 224, 16, 97, 1, 0, 0, 0 });
                    break;
                case Type.AL:
                    _device.Write(new byte[] { 224, 16, 65, 1, 0, 0, 0 });
                    break;
                case Type.ALV2:
                    _device.Write(new byte[] { 224, 16, 97, 1, 0, 0, 0 });
                    break;
                case Type.SLI:
                    _device.Write(new byte[] { 224, 16, 97, 1, 0, 0, 0 });
                    break;
            }
        }

        

    }
}
