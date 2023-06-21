using System;
using System.Collections.Generic;
using System.Linq;

namespace LianLi
{
    internal class HID
    {

        public static List<HIDDevice> Locate(int[] VENDOR_IDS, int[] PRODUCT_IDS)
        {

            var devices = new List<HIDDevice>();
            var locatedDevices = HidSharp.DeviceList.Local.GetHidDevices().ToArray();

            foreach (HidSharp.HidDevice device in locatedDevices)
            {

                var _vid = 0;
                var _pid = 0;
                int.TryParse(GetIdentifierPart("vid_", device.DevicePath), System.Globalization.NumberStyles.HexNumber, null, out _vid);
                int.TryParse(GetIdentifierPart("pid_", device.DevicePath), System.Globalization.NumberStyles.HexNumber, null, out _pid);

                if (VENDOR_IDS.Contains(_vid) && PRODUCT_IDS.Contains(_pid))
                {
                    devices.Add(new HIDDevice(
                        _vid,
                        _pid,
                        device
                        ));
                }
            }

            return devices;

        }

        private static string GetIdentifierPart(string identifier, string DeviceId)
        {
            var vidIndex = DeviceId.IndexOf(identifier, StringComparison.Ordinal);
            var startingAtVid = DeviceId.Substring(vidIndex + 4);
            return startingAtVid.Substring(0, 4);
        }

    }

    internal class HIDDevice
    {
        public int _pid;
        public int _vid;
        private HidSharp.HidDevice _device;
        private HidSharp.HidStream _stream;
        private HidSharp.Reports.ReportDescriptor _reportDescriptor;

        public HIDDevice(int VID, int PID, HidSharp.HidDevice Device)
        {
            _vid = VID;
            _pid = PID;            
            _device = Device;

            if(_device.TryOpen(out _stream))
            {
                _reportDescriptor = _device.GetReportDescriptor();
            }
        }

        public byte[] ReadInputReport(byte reportId, int bufferLength)
        {
            byte[] buffer = new byte[bufferLength];
            buffer[0] = reportId;
            _stream.GetInputReport(buffer);

            return buffer;
        }

        public void Write(byte[] buffer)
        {
            if (!_stream.CanWrite) { return; }
            _stream.Write(buffer);
        }

        public void Dispose()
        {
            if (_stream != null) { _stream.Dispose(); }
        }

    }
}
