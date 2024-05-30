using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace MiniErp.UI.FingerScanner
{
    public class FingerSensor
    {
        SerialPort _serialPort;
        public bool IsAdding { get; set; }
        public bool AddResult { get; set; }
        public int Index { get; set; }
        private FingerSensor(String com_port)
        {
            _serialPort = new SerialPort(com_port, 9600);
            _serialPort.Open();
        }

        private FingerSensor() { }

        ~FingerSensor()
        {
            _serialPort.Close();
        }

        public static FingerSensor FindSensor()
        {
            FingerSensor sensor = null;
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
                {
                    var portnames = SerialPort.GetPortNames();
                    var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());

                    var portList = portnames.Select(n => n + " - " + ports.FirstOrDefault(s => s.Contains(n))).ToList();

                    var port = ports.FirstOrDefault(x => x.Contains("CP210"));
                    if (string.IsNullOrEmpty(port))
                        return new FingerSensor();

                    var portName = portnames.FirstOrDefault(x => port.Contains(x));
                    sensor = new FingerSensor(portName);
                }
            }
            catch (Exception ex)
            {
                return new FingerSensor();
            }
            return sensor;
        }

        public bool CheckSensor()
        {
            _serialPort.WriteLine("CHECK");
            string receivedData = _serialPort.ReadLine();
            if (receivedData.Remove(receivedData.Length - 1) == "1")
                return true;
            return false;
        }

        public bool AddFinger(int index, int timeout_s)
        {
            IsAdding = true;
            String sendData = "ADDFI" + index.ToString().PadLeft(3) + timeout_s.ToString().PadLeft(3);
            _serialPort.WriteLine(sendData);
            string receivedData = _serialPort.ReadLine();
            if (receivedData.Remove(receivedData.Length - 1) == "0")
            {
                IsAdding = false;
                return true;
            }
            IsAdding = false;
            return false;
        }

        public int CheckFinger(int timeout_s)
        {
            String sendData = "FINDF" + timeout_s.ToString().PadLeft(3);
            _serialPort.WriteLine(sendData);
            string receivedData = _serialPort.ReadLine();
            int id = -1;
            int.TryParse(receivedData.Remove(receivedData.Length - 1), out id);
            return id;
        }

        public bool IsValid => _serialPort != null;
    }
}
