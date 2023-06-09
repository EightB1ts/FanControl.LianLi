using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using LibUsbDotNet;
using LibUsbDotNet.Main;

namespace FanControl.LianLiPlugin.Controllers
{
    public class Controllers
    {
        private bool enableARGB = true;
        public int deviceCount = 0;
        public List<Controller> controllers;

        public bool Locate()
        {
            int[] VENDOR_IDS = { 0x0cf2 };
            int[] PRODUCT_IDS = { 0xa100, 0xa103, 0x7750 };

            controllers = GetControllers(VENDOR_IDS, PRODUCT_IDS);

            if (controllers.Count > 0)
            {
                deviceCount = controllers.Count;
                return true;
            }

            return false;
        }

        public float GetFanSpeed(int controllerIndex, int channelIndex)
        {
            return controllers[controllerIndex].GetFanSpeed(channelIndex);
        }

        public void SetFanSpeed(int controllerIndex, int channelIndex, float speed)
        {
            controllers[controllerIndex].SetFanSpeed(channelIndex, speed);
        }

        private List<Controller> GetControllers(int[] VENDOR_IDS, int[] PRODUCT_IDS)
        {
            var foundControllers = new List<Controller>();

            UsbRegDeviceList allDevices = UsbDevice.AllDevices;
            foreach (UsbRegistry device in allDevices)
            {
                if (VENDOR_IDS.Contains(device.Vid) && PRODUCT_IDS.Contains(device.Pid))
                {
                    foundControllers.Add(new Controller(device, enableARGB));
                }
            }

            return foundControllers;
        }

    }

    public class Controller
    {

        public Controller(UsbRegistry deviceReg, bool enableARGB)
        {
            DeviceReg = deviceReg;
            EnableARGB = enableARGB;

            // SL Device
            if (deviceReg.Pid == 0xa100 || deviceReg.Pid == 0xa103 || deviceReg.Pid == 0xa7750) { DeviceProduct = Product.SL; }
            else { DeviceProduct = Product.Unknown; }
            EnableARGB = enableARGB;

            // ToDo: Add support for AL Devices
        }

        enum Product
        {
            SL,
            AL,
            Unknown
        }
     
        private UsbRegistry DeviceReg { get; set; }
        private Product DeviceProduct { get; set; }

        private static UsbDevice Device;

        private bool DisabledPWM;

        private bool EnableARGB;

        public float GetFanSpeed(int channelIndex)
        {

            if (DeviceProduct == Product.SL)
            {
                
                byte[] data = new byte[24];
                IntPtr resBuffer = SendConfig(0xc0, 0x81, 0xd800, data);

                if(resBuffer != IntPtr.Zero)
                {
                    Marshal.Copy(resBuffer, data, 0, data.Length);
                    return (float)BitConverter.ToUInt16(data.Skip(channelIndex*2).Take(2).ToArray(), 0);
                }

            }

            return 0.0f;
        }

        private bool Connected()
        {
            if(Device != null && Device.IsOpen) { return true; }
            if(DeviceReg.Open(out Device)) { return true; }
            return false;
        }

        public void SetFanSpeed(int channelIndex, float speed)
        {

            ushort channel_addr;
            ushort channel_commit_addr;
            byte disable_pwm_offset;

            // Convert speed percentage into UInt16, outputs as little endian.
            byte[] speedBytes = BitConverter.GetBytes((short)(800 + 1100 * speed / 100));

            if (DeviceProduct == Product.SL)
            {
                // Set offsets based off the channel index
                switch (channelIndex)
                {
                    case 0:
                        disable_pwm_offset = 0x10;
                        (channel_addr, channel_commit_addr) = (0xd8a0, 0xd890);
                        break;
                    case 1:
                        disable_pwm_offset = 0x20;
                        (channel_addr, channel_commit_addr) = (0xd8a2, 0xd891);
                        break;
                    case 2:
                        disable_pwm_offset = 0x40;
                        (channel_addr, channel_commit_addr) = (0xd8a4, 0xd892);
                        break;
                    case 3:
                        disable_pwm_offset = 0x80;
                        (channel_addr, channel_commit_addr) = (0xd8a6, 0xd893);
                        break;
                    default:
                        disable_pwm_offset = 0x10;
                        (channel_addr, channel_commit_addr) = (0xd8a0, 0xd890);
                        break;
                }

                // Check if PWM has been disabled yet or if EnableARGB is enabled
                if(!DisabledPWM)
                {
                    // Make sure PWM is disabled.
                    SendConfig(0x40, 0x80, 0xe020, new byte[] { 0x0, 0x31, disable_pwm_offset, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x01 });
                    DisabledPWM = true;
                }

                if(EnableARGB)
                {
                    SendConfig(0x40, 0x80, 0xe021, new byte[] { 0x30, 0x01 });
                    SendCommit(0x40, 0x80, 0xe02f);
                    EnableARGB = false;
                }
                
                // Send Fan Speeds & Commit Changes
                SendConfig(0x40, 0x80, channel_addr, speedBytes);
                SendCommit(0x40, 0x80, channel_commit_addr);
            }
        }

        private IntPtr ReserveMemory(byte[] buffer)
        {
            IntPtr resbuffer = Marshal.AllocHGlobal(buffer.Length);
            Marshal.Copy(buffer, 0, resbuffer, buffer.Length);

            return resbuffer;
        }

        private IntPtr SendConfig(byte requestType, byte request, ushort index, byte[] dataBuffer)
        {

            if(!Connected()) { return IntPtr.Zero; }
            bool success = false;
            var resBuffer = ReserveMemory(dataBuffer);

            if (DeviceProduct == Product.SL)
            {
                UsbSetupPacket setup = new UsbSetupPacket()
                {
                    RequestType = requestType,
                    Request = request,
                    Value = 0x0,
                    Index = (short)index
                };

                
                success = Device.ControlTransfer(ref setup, resBuffer, dataBuffer.Length, out int _);
            }

            Device.Close();
            Device = null;
            return resBuffer;
        }

        private void SendCommit(byte requestType, byte request, ushort index)
        {
            if (DeviceProduct == Product.SL)
            {
                SendConfig(requestType, request, index, new byte[] { 0x01 });
            }
        }

    }


}
